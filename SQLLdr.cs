using QManagerOracle.Parameters;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace QManagerOracle
{
    public class SQLLdr : ParamsDB
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

        string Execute(ParamsLoader loader, ParamsDB paramsDB)
        {
            string Credentials, ComandoStartNewWindow, FileNameSystem, output;
            if (CriarNovaJanela && Environment.OSVersion.Platform != PlatformID.Unix) ComandoStartNewWindow = "start "; else ComandoStartNewWindow = "";
            if (Environment.OSVersion.Platform == PlatformID.Unix) FileNameSystem = "/bin/bash"; else FileNameSystem = "cmd.exe";
            if (paramsDB != null) Credentials = paramsDB.GetCredentials(); else Credentials = GetCredentials();

            if (!Directory.Exists(loader.DirWorkControl))
            {
                Directory.CreateDirectory(loader.DirWorkControl);
                throw new Exception($@"Insert control file in path:\n{loader.DirWorkControl}");
            }
            var LogDir = $"{loader.DirWorkControl}\\LOG";
            var BadDir = $"{loader.DirWorkControl}\\BAD";
            if (!Directory.Exists(LogDir) || !Directory.Exists(BadDir))
            {
                Directory.CreateDirectory(LogDir);
                Directory.CreateDirectory(BadDir);
            }
            using (Process process = new Process())
            {

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = loader.DirWorkControl;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.FileName = FileNameSystem;

                process.StartInfo.CreateNoWindow = !CriarNovaJanela;

                process.Start();

                string[] log = loader.FileUpload.Split(new string[] { ".txt", ".csv", @"\", "." }, StringSplitOptions.RemoveEmptyEntries);
                string NomeSemExtensao = log[log.Length - 1];
                process.StandardInput.WriteLine($@"{ComandoStartNewWindow}{PathClient}SQLLDR.exe {Credentials} control={loader.FileControl} log=.\LOG\{NomeSemExtensao}.log bad=.\BAD\{NomeSemExtensao}.bad data={loader.FileUpload}");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            return output;
        }

        public async Task<string> ExecuteAsync(ParamsLoader loader, ParamsDB paramsDB = null)
        {
            try
            {
               return await Task.Run(() => Execute(loader, paramsDB));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void ExecuteNonAsync(ParamsLoader loader, ParamsDB paramsDB = null)
        {
            try
            {
                Task.Run(() => Execute(loader, paramsDB));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
