using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroScan.options
{
    public class ScanOptions
    {
        public string ExcludeSolutionDirectories { get; set; } = "";
        public string ExcludeProjectDirectories { get; set; } = "";
        public string ProjectFiles { get; set; } = "*.csproj";
        public string SolutionFiles { get; set; } = "*.sln";
    }
}
