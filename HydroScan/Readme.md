## Hydroscan ##


Run with:

Hydroscan <--path C:\Users\mark\source\repos> <--searchAllDirectories>

Override the execution path with:

--path C:\Users\mark\source\repos (or will default to . if omitted)

--searchAllDirectories false    (default to true and will scan all sub directories)


Hydroscan will scan all c# solutions and projects below the specified directory by defaut. The project xml is parsed to find all the project dependencies.
The dependencies are written to an elastic search index. Which can then be viewed with ElasticSearch graph:

Config options:

"ScanOptions": {
    // Solutions starting with the paths listed here will be filtered out
    "ExcludeSolutionDirectories": "C:\\Users\\mark\\source\\repos\\HydroScan",
    // Projects starting with the paths listed here will be filtered out
    "ExcludeProjectDirectories": "C:\\Users\\mark\\source\\repos\\caas-onsite-agent\\OPC_XML_DA\\",
    // Project files to search for
    "ProjectFiles": "*.csproj"
  },
  "ElasticOptions": {
    "ApiKey": "<apikey>",
    "CloudId": "<cloudid>",
    "Url": "https://<url>.elastic-cloud.com:9200",
    "Fingerprint": "<fingerprint>",
    "IndexName":  "Hydroscan"
  },

