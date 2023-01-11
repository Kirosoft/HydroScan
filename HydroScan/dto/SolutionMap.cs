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
                    Id = p.Name,
                    Name = p.Name,
                    AssemblyName = p.AssemblyName,
                    FullPath = p.FullPath,
                    DirectoryPath = p.DirectoryPath,
                    OutputType = p.OutputType,
                    TargetExtension = p.TargetExtension,
                    TargetFramework = p.TargetFramework,
                    DependencyIdList = dependencies.Select(x => x.Id).ToList(),
                    Dependencies = dependencies,
                    SolutionName = Id
                };
            }).ToList();
        }
    }
}
