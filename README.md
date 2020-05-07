## MinerFusion - Console Client

This is miner monitoring console desktop client. Use it to monitor your crypto mining rigs on [MinerFusion](https://minerfusion.com). Runs on Windows and Linux.

### Requirements
- [.NET Core 3.1 Runtime](https://dotnet.microsoft.com/download)

### Supported miners:

Support for more miners is coming shortly.

- Claymore
- Phoenix

### How to use

- Download and install .NET Core 3.1 Runtime
- Run program for it to create `miners.json` and `access_key.txt` files or create them yourself in main program directory.
- Paste your unique access key to `access_key.txt` file and save it.
- For program to work you must add at least one valid miner entry in `miners.json` file. Use template below to add new miner entries.

### Adding miner entries

To create new miner entry in `miners.json` file create new JSON object inside JSON array and add key-pair values that will allow client to get data from your miner. To add another miner entry just repeat the same process remembering to add comma after previous miner entry. If you are unsure how entries should look like look at the example below.

Here are key-value pairs that you should use to correcly add new miner entry:

| Key           | Value                                      | Optional          |
| ------------- |:-------------------------------------------|:-----------------:|
| MinerType     | <p>Claymore</p></p>Phoenix</p>             | no                |
| MinerName     | *string*                                   | no                |
| MinerIpAddress| *string*                                   | no                |
| MinerPassword | *string*                                   | yes               |
| MinerPort     | *int*                                      | yes               |


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
