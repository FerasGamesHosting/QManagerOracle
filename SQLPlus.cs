using QManagerOracle.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace QManagerOracle
{
    public class SQLPlus : ParamsDB, IParamsScript
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

        public Dictionary<IParamsScript, Task> Tarefas = new Dictionary<IParamsScript, Task>();
        protected List<IParamsScript> Scripts = new List<IParamsScript>();

        public int ID { get; set; }
        public int IDNext { get; set; }
        public string ScriptName { get; set; }
        public string ScriptDir { get; set; }
        public string Parameters { get; set; }
        public bool Wait { get; set; }

        void Execute(IParamsScript script)
        {
            if (!Directory.Exists(script.ScriptDir))
            {
                Directory.CreateDirectory(script.ScriptDir);
                throw new Exception(@$"Insert script file in path:\n{script.ScriptDir}");
            }
            using Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = script.ScriptDir;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = "cmd.exe";
            //process.StartInfo.Arguments = "chcp 65001";

            process.StartInfo.CreateNoWindow = true;

            process.Start();

            process.StandardInput.WriteLine($@"start {PathClient}sqlplus.exe {GetCredentials()} @{script.ScriptName} {script.Parameters}");
            process.StandardInput.Flush();
            process.StandardInput.Close();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        [Obsolete("Em fase de construção")]
        public async void ExecuteAllAsync()
        {
            List<Task> TarefasWait = new List<Task>();
            List<Task> TarefasNoWait = new List<Task>();

            foreach (IParamsScript iScript in Scripts)
            {
                if (iScript.Wait)
                {
                    TarefasWait.Add(new Task(() => { Execute(iScript); }));
                }
                else
                {
                    TarefasNoWait.Add(new Task(() => { Execute(iScript); }));
                }

            }
            TarefasWait.ForEach((t) => { t.Start(); });
            TarefasNoWait.ForEach((t) => { t.Start(); });
            //Parallel.ForEach<Task>(Tarefas, (t) => { t.Start()});
            await Task.WhenAll(TarefasWait.ToArray());
        }
        public async void ExecuteAsync(IParamsScript script)
        {
            await Task.Run(() => Execute(script));
        }

        [Obsolete("Em fase de construção")]
        public void Add(IParamsScript script)
        {
            Scripts.Add(
                new ParamsScript()
                {
                    ID = script.ID,
                    IDNext = script.IDNext,
                    ScriptDir = script.ScriptDir,
                    ScriptName = script.ScriptName,
                    Parameters = script.Parameters,
                    Wait = script.Wait
                });
        }
        [Obsolete("Em fase de construção")]
        public void Clear()
        {
            Scripts.Clear();
        }
    }
}
