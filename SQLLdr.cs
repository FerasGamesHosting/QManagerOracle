using QManagerOracle.Parameters;
using System;
using System.Diagnostics;
using System.IO;

namespace QManagerOracle
{
    public class SQLLdr : ParamsDB, IParamsLoader
    {
        public SQLLdr()
        {
        }

        public SQLLdr(ParamsDB @params)
        {
            this.IPAdress = @params.IPAdress ?? throw new ArgumentNullException(nameof(@params.IPAdress));
            this.PassDB = @params.PassDB ?? throw new ArgumentNullException(nameof(@params.PassDB));
            this.PathClient = @params.PathClient ?? throw new ArgumentNullException(nameof(@params.PathClient));
            if (@params.Port > 1) this.Port = @params.Port;
            this.Service = @params.Service ?? throw new ArgumentNullException(nameof(@params.Service));
            this.UserDB = @params.UserDB ?? throw new ArgumentNullException(nameof(@params.UserDB));
        }

        public string DirFile { get; set; }
        public string DirControl { get; set; }
        public string FileUpload { get; set; }
        public string FileControl { get ; set ; }

        void Execute(ParamsLoader loader)
        {
            if (!Directory.Exists(loader.DirControl))
            {
                Directory.CreateDirectory(loader.DirControl);
                throw new Exception(@$"Insert control file in path:\n{loader.DirControl}");
            }
            
            using Process process = new Process();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = "cmd.exe";

            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string[] vs = { ".txt", ".csv", @"\", "." };
            string[] log = loader.FileUpload.Split(vs, StringSplitOptions.RemoveEmptyEntries);
            process.StandardInput.WriteLine(@$"start {0}SQLLDR.exe {GetCredentials()} control={loader.FileControl} log=.\LOG\{log[log.Length - 1]}.log bad=.\BAD\{log[log.Length - 1]}.bad data={loader.FileUpload}");
            process.StandardInput.Flush();
            process.StandardInput.Close();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            try
            {
                File.Delete(loader.FileControl);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
