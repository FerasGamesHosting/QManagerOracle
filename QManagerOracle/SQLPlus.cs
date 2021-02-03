using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        struct Script
        {
            public int ID;
            public int IDNext;
            public string ScriptName;
            public string ScriptDir;
            public bool Wait;
        }

        readonly List<Script> Scripts = new List<Script>();
        void Execute(string scriptDir, string scriptFilename)
        {
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = scriptDir;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.FileName = "cmd.exe";
                //process.StartInfo.Arguments = "chcp 65001";

                process.StartInfo.CreateNoWindow = true;

                process.Start();

                process.StandardInput.WriteLine($@"start {PathClient}sqlplus.exe {GetCredentials()} @{scriptFilename}");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
        }

        public async void ExecuteAllAsync()
        {
            List<Task> TarefasWait = new List<Task>();
            List<Task> TarefasNoWait = new List<Task>();

            foreach (Script iScript in Scripts)
            {
                if (iScript.Wait)
                {
                    TarefasWait.Add(new Task(() => { Execute(iScript.ScriptDir, iScript.ScriptName); }));
                }
                else
                {
                    TarefasNoWait.Add(new Task(() => { Execute(iScript.ScriptDir, iScript.ScriptName); }));
                }
                
            }
            TarefasWait.ForEach((t) => { t.Start(); });
            TarefasNoWait.ForEach((t) => { t.Start(); });
            //Parallel.ForEach<Task>(Tarefas, (t) => { t.Start()});
            await Task.WhenAll(TarefasWait.ToArray());
        }


        //public static string ExecuteLOADER_S(string PathOra, string credentials, string scriptDir, string CTL, string arquivoUpload)
        //{
        //    string output = string.Empty;
        //    using (Process process = new Process())
        //    {
        //        process.StartInfo.UseShellExecute = false;
        //        process.StartInfo.WorkingDirectory = scriptDir;
        //        process.StartInfo.RedirectStandardOutput = true;
        //        process.StartInfo.RedirectStandardInput = true;
        //        process.StartInfo.FileName = "cmd.exe";

        //        process.StartInfo.CreateNoWindow = true;

        //        process.Start();

        //        string[] vs = { ".txt", ".csv", @"\", "." };
        //        string[] log = arquivoUpload.Split(vs, StringSplitOptions.RemoveEmptyEntries);
        //        process.StandardInput.WriteLine(string.Format(@"start {0}SQLLDR.exe {1} control={2} log=.\LOG\{3}.log bad=.\BAD\{3}.bad data={4}", PathOra, credentials, CTL, log[log.Length - 1], arquivoUpload));
        //        process.StandardInput.Flush();
        //        process.StandardInput.Close();
        //        output = process.StandardOutput.ReadToEnd();
        //        process.WaitForExit();
        //        try
        //        {
        //            File.Delete(CTL);
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }

        //    }

        //    return output;
        //}
        public async void ExecuteAsync(string scriptDir, string scriptFilename)
        {
            await Task.Run(() => Execute(scriptDir, scriptFilename));
        }
        

        public void Add(string scriptDir, string scriptFilename, bool wait = false, int id = 1)
        {
            Scripts.Add(
                new Script()
                {
                    ID = id,
                    ScriptDir = scriptDir,
                    ScriptName = scriptFilename,
                    Wait = wait
                });
        }
        public void Clear()
        {

        }
    }
}
