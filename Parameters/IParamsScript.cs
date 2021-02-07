namespace QManagerOracle.Parameters
{
    public interface IParamsScript 
    {
        public int ID { get; set; }
        public int IDNext { get; set; }
        public string ScriptName { get; set; }
        public string ScriptDir { get; set; }
        public string Parameters { get; set; }
        public bool Wait { get; set; }
    }
}
