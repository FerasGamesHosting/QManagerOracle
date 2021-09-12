using QManagerOracle.Parameters;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace QManagerOracle
{
    public class SQLPlus : ParamsDB
    {
        public delegate void Debug(string result);
        public event Debug DebugEventParams;
        int ProcessID = 0;
        public SQLPlus()
        {
            DebugEventParams += SQLPlus_DebugEventParams;
        }

        private void SQLPlus_DebugEventParams(string result)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[Debug] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(result);
        }

        public SQLPlus(ParamsDB @params) : base()
        {
            this.IPAdress = @params.IPAdress ?? throw new ArgumentNullException(nameof(@params.IPAdress));
            this.PassDB = @params.PassDB ?? throw new ArgumentNullException(nameof(@params.PassDB));
            this.PathClient = @params.PathClient ?? throw new ArgumentNullException(nameof(@params.PathClient));
            if (@params.Port > 1) this.Port = @params.Port;
            this.Service = @params.Service ?? throw new ArgumentNullException(nameof(@params.Service));
            this.UserDB = @params.UserDB ?? throw new ArgumentNullException(nameof(@params.UserDB));
        }
        void Execute(ParamsScript script, ParamsDB paramsDB)
        {
            string Credentials, ComandoStartNewWindow, FileNameSystem;
            if (CriarNovaJanela && Environment.OSVersion.Platform == PlatformID.Win32NT) ComandoStartNewWindow = "start "; else ComandoStartNewWindow = "";
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
            if (!Directory.Exists(script.ScriptDir))
            {
                Directory.CreateDirectory(script.ScriptDir);
                throw new Exception($@"Insert script file in path:\n{script.ScriptDir}");
            }
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = script.ScriptDir;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.FileName = FileNameSystem;

                process.StartInfo.CreateNoWindow = !CriarNovaJanela;

                process.Start();
                ProcessID = process.Id;
                string ProcPar = $@"{ComandoStartNewWindow}{PathClient}sqlplus.exe {Credentials} @{script.ScriptName} {script.Parameters}";
                process.StandardInput.WriteLine(ProcPar);
                process.StandardInput.Flush();
                process.StandardInput.Close();

                if (script.Debug)
                {
                    DebugEventParams.Invoke(ProcPar + Environment.NewLine);
                }
                DebugEventParams.Invoke(process.StandardOutput.ReadToEnd());
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
        string ExecuteS(ParamsScript script, ParamsDB paramsDB)
        {
            Execute(script, paramsDB);
            return string.Empty;
        }
        [Obsolete("Será retirado na proxima release, utilize o evento DebugEventParams para acompanhar em tempo real.")]
        public async Task<string> ExecuteAsync(ParamsScript script, ParamsDB paramsDB = null)
        {
            try
            {
                return await Task.Run(() => ExecuteS(script, paramsDB));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void ExecuteNonAsync(ParamsScript script, ParamsDB paramsDB = null)
        {
            try
            {
                Task.Run(() => Execute(script, paramsDB));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public Task TaskExecute(ParamsScript script, ParamsDB paramsDB = null)
        {
            try
            {
                Task task = new Task(() =>
                {
                    Execute(script, paramsDB);
                }
                );
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
