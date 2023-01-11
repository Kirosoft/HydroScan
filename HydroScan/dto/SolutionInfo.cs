using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroScan.dto
{
    public class SolutionInfo
    {
        public string SolutionName { get; set; } = "SolutionName";
        public List<string> ProjectNames { get; set; } = new List<string>();
    }
}
