using QManagerOracle.Parameters;
using System;
using System.Diagnostics;
using System.IO;

namespace QManagerOracle
{
    public class SQLLdr : ParamsDB
    {
        public SQLLdr(ParamsDB @params)
        {
            this.IPAdress = @params.IPAdress ?? throw new ArgumentNullException(nameof(@params.IPAdress));
            this.PassDB = @params.PassDB ?? throw new ArgumentNullException(nameof(@params.PassDB));
            this.PathClient = @params.PathClient ?? throw new ArgumentNullException(nameof(@params.PathClient));
            if (@params.Port > 1) this.Port = @params.Port;
            this.Service = @params.Service ?? throw new ArgumentNullException(nameof(@params.Service));
            this.UserDB = @params.UserDB ?? throw new ArgumentNullException(nameof(@params.UserDB));
        }

        public string Execute(string scriptDir, string CTL, string arquivoUpload)
        {
            if (!Directory.Exists(@$"{scriptDir}\CTL")) 
            { 
                Directory.CreateDirectory(@$"{scriptDir}\CTL");
                throw new Exception(@$"Insert control file in path:\n{scriptDir}\CTL");
            }
            string output = string.Empty;
            using Process process = new Process();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = scriptDir;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = "cmd.exe";

            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string[] vs = { ".txt", ".csv", @"\", "." };
            string[] log = arquivoUpload.Split(vs, StringSplitOptions.RemoveEmptyEntries);
            process.StandardInput.WriteLine(@$"start {0}SQLLDR.exe {GetCredentials()} control=.\CTL\{CTL} log=.\LOG\{log[log.Length - 1]}.log bad=.\BAD\{log[log.Length - 1]}.bad data={arquivoUpload}");
            process.StandardInput.Flush();
            process.StandardInput.Close();
            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            try
            {
                File.Delete(CTL);
            }
            catch (Exception)
            {
                throw;
            }



            return output;
        }
    }
}
