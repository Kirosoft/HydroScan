using DotNetProjectParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HydroScan.dto
{
    public class ProjectDependency
    {
        public string Id { get; set; } = "DefaultId";
        public string ProjectName { get; set; } = "DefaultProjectName";
        /// <summary>
        /// Name of item
        /// </summary>
        public string ItemName { get; internal set; } = "ItemName";

        /// <summary>
        /// Type of item (Compile, Reference, EmbeddedResource etc)
        /// </summary>
        public string ItemType { get; internal set; } = "ItemType";

        /// <summary>
        /// The relative path of the item that is included 
        /// </summary>
        public string Include { get; internal set; } = "Include";

        /// <summary>
        /// The absolute path of the included item
        /// </summary>
        public string ResolvedIncludePath { get; internal set; } = "ResolvedIncludePath";

        /// <summary>
        /// Is the item be copied to output
        /// </summary>
        public string CopyToOutputDirectory { get; internal set; } = "CopyToOutputDirectory";

        /// <summary>
        /// The version of the item that is included 
        /// </summary>
        public string Version { get; internal set; } = "Version";

        public string SolutionName { get; set; } = "SolutionName";
    }
}
