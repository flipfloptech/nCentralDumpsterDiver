# SolarWinds n-Central Dumpster Diver

## Description / Explanation
This application utilizes the nCentral agent dot net libraries to simulate the agent registration and pull the agent/appliance configuration settings. This information can contain plain text active directory domain credentials. This was reported to SolarWinds PSIRT(psirt@solarwinds.com) on 10/10/2019. In most cases the agent download URL is not secured allowing anyone without authorization and known customer id to download the agent software. Once you have a customer id you can self register and pull the config. Application will test availability of customer id via agent download URL. If successful it will then pull the config. We do not attempt to just pull the config because timing out on the operation takes to long. Removing the initial check, could produce more results as the agent download could be being blocked where as agent communication would not be.
    
Harmony is only used to block the nCentral libraries from saving and creating a "config" directory that is not needed.
    
## Usage
    nCentralDumpsterDiver 1.0.0.0
    Copyright c  2020
    ERROR(S):
      Required option 'u, url' is missing.
      -u, --url           Required. URLs to be Processed
      -i, --id            Customer IDs to try processing, will be excluded from bruteforce
    
      -b, --bruteforce    (Default: false) Enable Customer ID BruteForce
      --min               (Default: 100) Minimum Customer ID to try for bruteforce.
      --max               (Default: 200) Maximum Customer ID to try for bruteforce.
      --help              Display this help screen.
      --version           Display version information.`
## Example
     C:\Storage\nCentralDumpsterDiver>nCentralDumpsterDiver.exe -u https://nable.localhost.localdomain/ -b
    [10:49:00 INF] Processing https://nable.localhost.localdomain/ started
    [10:49:00 INF] Starting bruteforce, this will exclude any previously specified customer id(s)
## Seeing is believing

### Search
[![](https://github.com/flipflopfpv/nCentralDumpsterDiver/blob/master/nCentralDumpsterDiver/screenshots/ShodanSearch.png?raw=true)](https://www.shodan.io/search?query=%22Set-Cookie%3A+ncentral_version%3D%22 "Shodan.io Results")

### Run
![](https://github.com/flipflopfpv/nCentralDumpsterDiver/blob/master/nCentralDumpsterDiver/screenshots/Running.png?raw=true)

### Review
![](https://github.com/flipflopfpv/nCentralDumpsterDiver/blob/master/nCentralDumpsterDiver/screenshots/Results.png?raw=true)
