using System;

namespace QManagerOracle
{
    public class ParamsDB
    {
        public ParamsDB()
        {
        }

        public ParamsDB(string iPAdress, int port, string service, string userDB, string passDB, string pathClient)
        {
            IPAdress = iPAdress ?? throw new ArgumentNullException(nameof(iPAdress));
            Port = port;
            Service = service ?? throw new ArgumentNullException(nameof(service));
            UserDB = userDB ?? throw new ArgumentNullException(nameof(userDB));
            PassDB = passDB ?? throw new ArgumentNullException(nameof(passDB));
            PathClient = pathClient ?? throw new ArgumentNullException(nameof(pathClient));
        }

        public string IPAdress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 1521;
        public string Service { get; set; } = "XE";
        public string UserDB { get; set; } = "SYSTEM";
        public string PassDB { get; set; } = "";
        public string PathClient { get; set; } = "";

        protected string GetCredentials()
        {
            return $"{UserDB}/{PassDB}@{IPAdress}:{Port}/{Service}";
        }
    }
}
