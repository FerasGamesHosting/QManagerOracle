using QManagerOracle.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace QManagerOracle
{
    public class SQLPlus : ParamsDB
    {
        public SQLPlus()
        {
        }
        public SQLPlus(ParamsDB @params)
        {
            this.IPAdress = @params.IPAdress ?? throw new ArgumentNullException(nameof(@params.IPAdress));
            this.PassDB = @params.PassDB ?? throw new ArgumentNullException(nameof(@params.PassDB));
            this.PathClient = @params.PathClient ?? throw new ArgumentNullException(nameof(@params.PathClient));
            if (@params.Port > 1) this.Port = @params.Port;
            this.Service = @params.Service ?? throw new ArgumentNullException(nameof(@params.Service));
            this.UserDB = @params.UserDB ?? throw new ArgumentNullException(nameof(@params.UserDB));
        }

        string Execute(ParamsScript script, ParamsDB paramsDB)
        {
            string Credentials, ComandoStartNewWindow, FileNameSystem, output;
            if (CriarNovaJanela && Environment.OSVersion.Platform != PlatformID.Unix) ComandoStartNewWindow = "start "; else ComandoStartNewWindow = "";
            if (Environment.OSVersion.Platform == PlatformID.Unix) FileNameSystem = "/bin/bash"; else FileNameSystem = "cmd.exe";

            if (paramsDB != null) Credentials = paramsDB.GetCredentials(); else Credentials = GetCredentials();
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

                process.StandardInput.WriteLine($@"{ComandoStartNewWindow}{PathClient}sqlplus.exe {Credentials} @{script.ScriptName} {script.Parameters}");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            return output;
        }

        public async Task<string> ExecuteAsync(ParamsScript script, ParamsDB paramsDB = null)
        {
            try
            {
               return await Task.Run(() => Execute(script, paramsDB));
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
    }
}
