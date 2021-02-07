using System;
using System.Collections.Generic;
using System.Text;

namespace QManagerOracle.Parameters
{
    public class ParamsLoader : IParamsLoader
    {
        public string DirFile { get; set; }
        public string DirControl { get; set; }
        public string FileUpload { get; set; }
        public string FileControl { get; set; }
    }
}
