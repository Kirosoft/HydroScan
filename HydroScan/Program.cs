using DotNetProjectParser;
using System.CommandLine;
using System.Reflection;
using System.Linq;
using Serilog;
using Microsoft.Extensions.Configuration;
using HydroScan.options;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using static System.Net.Mime.MediaTypeNames;
using HydroScan.dto;

class Program
{
    private static ScanOptions _scanOptions = new ScanOptions();
    private static ElasticOptions _elasticOptions = new ElasticOptions();
    private static ElasticsearchClient? _client;
 
    static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/data.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("Starting HydroScan");

        var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false);

        IConfiguration config = builder.Build();
        _scanOptions = config.GetSection("ScanOptions").Get<ScanOptions>() ?? new ScanOptions();
        _elasticOptions = config.GetSection("ElasticOptions").Get<ElasticOptions>() ?? new ElasticOptions();
        var settings = new ElasticsearchClientSettings(_elasticOptions.CloudId, new ApiKey(_elasticOptions.ApiKey));
                                 //.CertificateFingerprint(_elasticOptions.Fingerprint)
                                //.Authentication(new ApiKey(_elasticOptions.ApiKey));
        _client = new ElasticsearchClient(settings);

        var pathOption = new Option<string?>(
            name: "--path",
            description: "start path",
            getDefaultValue: () => ".");

        var searchAllDirectoriesOption = new Option<bool>(
            name: "--searchAllDirectories",
            description: "search all directories or just the top level",
            getDefaultValue: () => true);

        var rootCommand = new RootCommand("HydroScan");
        rootCommand.AddOption(pathOption);
        rootCommand.AddOption(searchAllDirectoriesOption);

        rootCommand.SetHandler((path, searchAllDirectoriesOption) =>
        {
            ScanTheSolution(path!, searchAllDirectoriesOption);
        }, pathOption, searchAllDirectoriesOption);

        return await rootCommand.InvokeAsync(args);
    }

    static void ScanTheSolution(string path, bool searchAllDirectories)
    {
        var projectFilePaths = Directory.GetFiles(path, _scanOptions.ProjectFiles, searchAllDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        var solutionFilePaths = Directory.GetFiles(path, _scanOptions.SolutionFiles, searchAllDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        // find all projects in a solution
        var solutionRootPaths = solutionFilePaths.Select(x => String.Join("\\", x.Split("\\").Reverse().Skip(1).Reverse()))
                                .Distinct();  // sometimes multiple solutions in the same directory TODO: refine this logic

        // exclude any solution files starting with the path list in ExcludeDirectories 
        var filteredSolutionFilePaths = solutionRootPaths.Where(x => _scanOptions?.ExcludeSolutionDirectories
                                                            .Split(",")?.Any(y => !x.StartsWith(y, StringComparison.CurrentCultureIgnoreCase)) ?? true);

        if (filteredSolutionFilePaths != null)
        {
            // exclude any project files starting with the path list in ExcludeDirectories 
            var filteredProjectFilePaths = projectFilePaths?.Where(x => _scanOptions?.ExcludeProjectDirectories
                                                                .Split(",")?.Any(y => !x.StartsWith(y, StringComparison.CurrentCultureIgnoreCase)) ?? true)
                                                            .Where(x => _scanOptions?.ExcludeSolutionDirectories
                                                                .Split(",")?.Any(y => !x.StartsWith(y, StringComparison.CurrentCultureIgnoreCase)) ?? true)
                                                                .Distinct();

            if (filteredProjectFilePaths != null)
            {

                // Parse the project file into properties
                var projectFiles = filteredProjectFilePaths?.Select(projectFile => ProjectFactory.GetProject(new FileInfo(projectFile))).Where(p => p != null);

                if (projectFiles != null)
                {
                    var allSolutionsMaps = filteredSolutionFilePaths.Select(x => new SolutionMap(x, projectFiles
                                            .Where(y => y.FullPath.StartsWith(x, StringComparison.CurrentCultureIgnoreCase))
                                            //.Select(y => y.Split("\\").TakeLast(1).First()) // Takes just the project name and not the path
                                            .ToList()));

                    var response = _client?.IndexMany(allSolutionsMaps.Select(s => new SolutionInfo { SolutionName = s.SolutionName, ProjectNames = s.Projects.Select(p => p.Name).ToList()}), 
                                                                                $"{_elasticOptions.IndexName}_solutions");
                    response = _client?.IndexMany(allSolutionsMaps.SelectMany(s => s.Projects.Select(p => p)), $"{_elasticOptions.IndexName}_projects");
                    response = _client?.IndexMany(allSolutionsMaps.SelectMany(s => s.Projects.SelectMany(p => p.Dependencies.Select(d => d))), 
                                            $"{_elasticOptions.IndexName}_dependencies");

                    if (response == null || !response.IsValidResponse)
                    { 
                        throw new Exception("Elasticsearch response error. " + response?.DebugInformation);
                    }

                }
            }
        }

        Log.Information("Hydroscan finished");

    }
}