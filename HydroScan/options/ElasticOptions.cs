using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroScan.options
{
    public class ElasticOptions
    {
        public string ApiKey { get; set; } = "";
        public string Url { get; set; } = "";
        public string Fingerprint { get; set; } = "";

        public string CloudId { get; set; } = "";
    }
}
