namespace QManagerOracle.Parameters
{
    public interface IParamsScript 
    {
        int ID { get; set; }
        int IDNext { get; set; }
        string ScriptName { get; set; }
        string ScriptDir { get; set; }
        string Parameters { get; set; }
        bool Wait { get; set; }
    }
}
