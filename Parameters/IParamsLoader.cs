using System;
using System.Collections.Generic;
using System.Text;

namespace QManagerOracle.Parameters
{
    public interface IParamsLoader
    {
        string DirFile { get; set; }
        string DirControl { get; set; }
        string FileUpload { get; set; }
        string FileControl { get; set; }

    }
}
