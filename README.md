# MinerFusion - Console Client

This is miner monitoring console desktop client. Use it to monitor your crypto mining rigs on [MinerFusion](https://minerfusion.com). Runs on Windows and Linux and MacOS.

## Requirements

- [.NET 5.0 Runtime](https://dotnet.microsoft.com/download)

### Supported miners

Support for more miners is coming shortly.

- Claymore
- Phoenix
- NBMiner
- LolMiner
- T-Rex
- XMRig

### How to use

#### Windows

- Download and install .NET Core 3.1 Runtime
- Download MinerFusion - Console client
- Run program for it to create `miners.json` and `access_key.txt` files or create them yourself in main program directory.
- Paste your unique access key to `access_key.txt` file and save it.
- For program to work you must add at least one valid miner entry in `miners.json` file. Use template below to add new miner entries.

#### Linux / MacOS

- Follow these [instructions](https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-manager-ubuntu-2004) for Linux and these [instructions](https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-3.1.300-macos-x64-installer) for MacOS to download and install .NET Core 3.1 Runtime for your OS
- Download MinerFusion - Console client, unzip and copy to target destination
- Being in console clinet folder execute: `chmod +x MinerFusionConsole`
- Run `./MinerFusionConsole` and it will create `miners.json` and `access_key.txt`
- Paste your unique access key to `access_key.txt` file and save it.
- For program to work you must add at least one valid miner entry in `miners.json` file. Use template below to add new miner entries.

### Adding miner entries

To create new miner entry in `miners.json` file create new JSON object inside JSON array and add key-pair values that will allow client to get data from your miner. To add another miner entry just repeat the same process remembering to add comma after previous miner entry. If you are unsure how entries should look like look at the example below.

... or just use this [template](https://gist.github.com/kitohe/ab07d185ec6b91ea3b0bca410771fa10)

Here are key-value pairs that you should use to correcly add new miner entry:

| Key           | Value                                                      | Optional          |
| ------------- |:----------------------------------------------------------:|:-----------------:|
| MinerType     | *<p>Claymore</p><p>Phoenix</p><p>NBMiner</p><p>LolMiner</p><p>TRex</p><p>XMRig</p>* | no                |
| MinerName     | *string*                                                   | no                |
| MinerIpAddress| *string*                                                   | no                |
| MinerPassword | *string*                                                   | yes               |
| MinerPort     | *int*                                                      | no *(yes if port = 3333)*             |


Example of `miners.json` file with two different miners:
```json
[
  {
    "MinerType": "Claymore",
    "MinerName": "claymore_miner",
    "MinerIpAddress": "192.168.1.199",
    "MinerPassword": "",
    "MinerPort": 3333
  },
  {
    "MinerType": "Phoenix",
    "MinerName": "phoenix_miner",
    "MinerIpAddress": "192.168.1.200",
    "MinerPassword": "",
    "MinerPort": 3333
  }
]
```

*Notes:*

- If you somehow lose you `miners.json` file and you want to reconnect to your already existing miner data on website,
just go to [monitoring](https://minerfusion.com/Monitoring) tab and click details and copy `minerId` from url and paste it into your `minerId` field in `miners.json` file
- LolMiner does not support monitoring GPUs temperatures and fan speeds
- TRex Miner does not support monitoring shares per GPU
