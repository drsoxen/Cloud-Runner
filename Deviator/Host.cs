using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Deviator
{
    class User
    {
        string registered = "UNKNOWN";
        string lastonline = "UNKNOWN";
        string username = "UNKNOWN";
        string computername = "UNKNOWN";
        string addr = "UNKNOWN";
        string processorcount = "UNKNOWN";
        string totalmemory = "UNKNOWN";
        string osversion = "UNKNOWN";
        string osdomainname = "UNKNOWN";
        string[] drives;
        string[] environmentvars;
    }

    class Host
    {
        FtpWebRequest request;

        string server     = "";
        string username   = "";
        string password   = "";

        public void Initialize(string filename)
        {
            Log.STR("Initializing host (" + filename + ")");
            try
            {
                StreamReader sr = new StreamReader(File.Open(filename, FileMode.Open) as Stream);
                server = sr.ReadLine();
                username = sr.ReadLine();
                password = sr.ReadLine();
                Log.STR("Host server (" + server + ") Host user (" + username + ")");
                sr.Close();
            }
            catch (System.Exception ex)
            {
                Log.STR("Failed with message: \"" + ex.Message + "\"");
            }
        }

        public string DHCP()
        {
            return "205.211.50.10";
        }

        public string IP()
        {
            return "205.211.50.10";
        }

        public bool Upload(string src, string dest, int chunksize = 2048)
        {
            FileInfo inf = new FileInfo(src);
            string uri = "ftp://" + server + "/" + dest;
            Log.STR("Uploading file (" + src + ") to (" + dest + ")");
            try
            {
                Log.STR("Establishing FTP web request...");
                request = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            }
            catch (System.Exception ex)
            {
                Log.STR("Failed with message: \"" + ex.Message + "\"");
                return false;
            }
            Log.STR("Success");

            request.Credentials = new NetworkCredential(username, password);
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.ContentLength = inf.Length;

            FileStream fs;
            try
            {
                fs = File.Open(src, FileMode.Open);
            }
            catch (System.Exception ex)
            {
                Log.STR("Failed with message: \"" + ex.Message + "\"");
                return false;
            }
            

            byte[] buff = new byte[chunksize];
            int contentLen;
            
            Log.STR("Starting file upload. File size (" + fs.Length + ") chunk size (" + chunksize + ")");
            try
            {
                Stream strm = request.GetRequestStream();
                contentLen = fs.Read(buff, 0, chunksize);
                while (contentLen > 0)
                {
                    Log.STR("Writing file chunk");
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, chunksize);
                }
                strm.Close();
                fs.Close();
            }
            catch (System.Exception ex)
            {
                Log.STR("Failed with message: \"" + ex.Message + "\"");
                return false;
            }
            Log.STR("Success.");
            return true;
        }

        public byte[] Download(string file, int chunksize = 2048)
        {
            MemoryStream outputStream;
            try
            {
                outputStream = new MemoryStream();

                request = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + server + "/" + file));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(username, password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Log.STR("Failed with message: \"" + ex.Message + "\"");
                return null;
            }
            return outputStream.ToArray();
        }
    }
}
