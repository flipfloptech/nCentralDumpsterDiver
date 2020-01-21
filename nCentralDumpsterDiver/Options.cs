using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
namespace nCentralDumpsterDiver
{
    class Options
    {
        [Option('u', "url", Required = true, HelpText = "URLs to be Processed", Separator = ',')]
        public IEnumerable<string> InputUrls { get; set; }


        [Option('i', "id", Required = false, HelpText = "Customer IDs to try processing, will be excluded from bruteforce",Separator=',')]
        public IEnumerable<int> CustomerID { get; set; }

        [Option('b', "bruteforce", Default = false, Required = false, HelpText = "Enable Customer ID BruteForce")]
        public bool BruteForceEnabled { get; set; }

        [Option("min", Default = 100, Required = false, HelpText = "Minimum Customer ID to try for bruteforce.")]
        public int CustomerIDMinimum { get; set; }
        [Option("max", Default = 200, Required = false, HelpText = "Maximum Customer ID to try for bruteforce.")]
        public int CustomerIDMaximum { get; set; }
    }
}
