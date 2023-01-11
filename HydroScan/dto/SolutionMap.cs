using DotNetProjectParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroScan.dto
{
    public class SolutionMap
    {
        public string Id { get; set; }
        public  string SolutionName { get; set; }
        public List<ProjectInfo> Projects { get; set; }
    
        public SolutionMap(string solutionName, List<Project> projects) 
        {
            SolutionName = solutionName;
            Id = solutionName.Split("\\").TakeLast(1).First();

            Projects = projects.Select(p =>
            {
                var dependencies = p?.Items.Where(x => x.ItemType == "PackageReference")
                                .Select(i => new ProjectDependency
                                {
                                    Id = i.ItemName,
                                    ItemName = i.ItemName,
                                    ItemType = i.ItemType,
                                    Include = i.Include,
                                    ResolvedIncludePath = i.ResolvedIncludePath,
                                    CopyToOutputDirectory = i.CopyToOutputDirectory,
                                    Version = i.Version,
                                    ProjectName = p.Name,
                                    SolutionName = solutionName
                                }).ToList();

                return new ProjectInfo
                {
                    Id = p?.Name ?? $"Id Missing-{DateTime.UtcNow}",
                    Name = p?.Name ?? $"Name missing-{DateTime.UtcNow}",
                    AssemblyName = p?.AssemblyName ?? $"AssemblyNameMissing",
                    FullPath = p?.FullPath ?? $"FullPath missing",
                    DirectoryPath = p?.DirectoryPath ?? $"DirectoryPath missing-{DateTime.UtcNow}",
                    OutputType = p?.OutputType ?? $"OutputType missing-{DateTime.UtcNow}",
                    TargetExtension = p?.TargetExtension ?? $"TargetExtension missing-{DateTime.UtcNow}",
                    TargetFramework = p?.TargetFramework ?? $"TargetFrameowkr missing-{DateTime.UtcNow}",
                    DependencyIdList = dependencies?.Select(x => x.Id).ToList() ?? new List<string>(),
                    Dependencies = dependencies ?? new List<ProjectDependency>(),
                    SolutionName = Id
                };
            }).ToList();
        }
    }
}
