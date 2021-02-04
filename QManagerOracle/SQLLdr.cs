using QManagerOracle.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
