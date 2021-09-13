using QManagerOracle.Parameters;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace QManagerOracle
{
    public class SQLLdr : ParamsDB , IDisposable
    {
        public delegate void Debug(string result);
        public event Debug DebugEventParams;
        int ProcessID = 0;
        public SQLLdr()
        {
            DebugEventParams += SQLLdr_DebugEventParams;
        }

        private void SQLLdr_DebugEventParams(string result)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[Debug] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(result);
        }

        public SQLLdr(ParamsDB @params) : base()
        {
            this.IPAdress = @params.IPAdress ?? throw new ArgumentNullException(nameof(@params.IPAdress));
            this.PassDB = @params.PassDB ?? throw new ArgumentNullException(nameof(@params.PassDB));
            this.PathClient = @params.PathClient ?? throw new ArgumentNullException(nameof(@params.PathClient));
            if (@params.Port > 1) this.Port = @params.Port;
            this.Service = @params.Service ?? throw new ArgumentNullException(nameof(@params.Service));
            this.UserDB = @params.UserDB ?? throw new ArgumentNullException(nameof(@params.UserDB));
        }

        void Execute(ParamsLoader loader, ParamsDB paramsDB)
        {
            string Credentials, ComandoStartNewWindow, FileNameSystem;
            
            if (Environment.OSVersion.Platform == PlatformID.Unix) FileNameSystem = "/bin/bash"; else FileNameSystem = "cmd.exe";
            if (paramsDB != null)
            {
                Credentials = paramsDB.GetCredentials();
                CriarNovaJanela = paramsDB.CriarNovaJanela;
            }
            else
            {
                Credentials = GetCredentials();
            }
            if (CriarNovaJanela && Environment.OSVersion.Platform == PlatformID.Win32NT) ComandoStartNewWindow = "start "; else ComandoStartNewWindow = "";

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
                ProcessID = process.Id;
                string[] log = loader.FileUpload.Split(new string[] { ".txt", ".csv", @"\", "." }, StringSplitOptions.RemoveEmptyEntries);
                string NomeSemExtensao = log[log.Length - 1];
                string ProcPar = $@"{ComandoStartNewWindow}{PathClient}SQLLDR.exe {Credentials} control={loader.FileControl} log=.\LOG\{NomeSemExtensao}.log bad=.\BAD\{NomeSemExtensao}.bad data={loader.FileUpload}";
                //process.StandardInput.WriteLine("@echo on");
                process.StandardInput.WriteLine(ProcPar);
                process.StandardInput.Flush();
                process.StandardInput.Close();
                if (loader.Debug)
                {
                    DebugEventParams.Invoke(ProcPar + Environment.NewLine);
                }
                while (!process.StandardOutput.EndOfStream)
                {
                    DebugEventParams.Invoke(process.StandardOutput.ReadLine() + Environment.NewLine);
                }
                
                process.WaitForExit();
            }
        }
        public void KillProcess()
        {
            if (ProcessID != 0)
            {
                using (Process process = Process.GetProcessById(ProcessID))
                {
                    process.Kill();
                };
            }
        }
                
        public async Task ExecuteAsync(ParamsLoader loader, ParamsDB paramsDB = null)
        {
            try
            {
                await Task.Run(() => Execute(loader, paramsDB));
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
        public Task TaskExecute(ParamsLoader loader, ParamsDB paramsDB = null)
        {
            try
            {
                return new Task(() =>
                {
                    Execute(loader, paramsDB);
                }
                ); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void Dispose()
        {
            KillProcess();
            GC.SuppressFinalize(this);
        }
    }
}
