using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MinerFusionConsole.Models.Miners;
using MinerFusionConsole.Models.Miners.PhoenixMiner;
using Newtonsoft.Json;

namespace MinerFusionConsole.Services.MinerServices
{
    public class PhoenixService : IMinerService
    {
        private TcpClient _tcpClient;

        private readonly BaseMinerModel _model;

        private readonly string _minerIpAddress;

        private readonly int _minerPort;

        private readonly string _password;

        public PhoenixService(string minerId, string userId, string minerName, string minerIpAddress, int minerPort = 3333, string password = "")
        {
            _model = new BaseMinerModel(minerId, userId, minerName);
            _tcpClient = new TcpClient();
            _minerIpAddress = minerIpAddress;
            _minerPort = minerPort;
            _password = password;
        }

        public async Task<BaseMinerModel> GetMinerStatus()
        {
            _model.FlushModelFields();

            try
            {
                UnpackMinerStatus(await DownloadMinerStatus());
            }
            catch (Exception)
            { // ignored
            } 


            return _model;
        }

        private async Task<IImmutableList<string>> DownloadMinerStatus()
        {
            await TryConnect(_minerIpAddress, _minerPort);
            if (!_tcpClient.Connected)
                return null;


            var query = "{\"id\":0,\"jsonrpc\":\"2.0\",\"method\":\"miner_getstat1\",\"psw\":\"" + _password + "\"}\n";
            var networkStream = _tcpClient.GetStream();


            if (!networkStream.CanWrite || !networkStream.CanRead) return null;

            byte[] sendBytes = Encoding.ASCII.GetBytes(query);
            await networkStream.WriteAsync(sendBytes, 0, sendBytes.Length);

            byte[] bytes = new byte[_tcpClient.ReceiveBufferSize];
            await networkStream.ReadAsync(bytes, 0, (int)_tcpClient.ReceiveBufferSize);

            string returnData = Encoding.UTF8.GetString(bytes);

            return JsonConvert.DeserializeObject<PhoenixMinerModel>(returnData).Result.ToImmutableList();
        }

        private void UnpackMinerStatus(IImmutableList<string> minerData)
        {
            var start = Stopwatch.StartNew();
            if (minerData == null)
            {
                _model.MinerAlive = false;
                return;
            }

            _model.FlushModelFields();

            var minerStats = minerData.ElementAt(2).Split(';');
            var gpuTempsAndFans = minerData.ElementAt(6).Split(';');

            _model.MinerVersion = minerData.ElementAt(0);
            _model.UpTime = int.Parse(minerData.ElementAt(1));
            _model.TotalHashRate = double.Parse(minerStats.ElementAt(0)) / 1000;
            _model.AcceptedShares = int.Parse(minerStats.ElementAt(1));
            _model.RejectedShares = int.Parse(minerStats.ElementAt(2));
            _model.PerGpuHashRate = Array.ConvertAll(minerData.ElementAt(3).Split(';'), double.Parse).ToList().Select(d => d / 1000).ToList();
            _model.MineServer = minerData.ElementAt(7);
            _model.MinerAlive = true;

            for (int i = 0; i < gpuTempsAndFans.Length; i++)
            {
                if (i % 2 == 0)
                    _model.PerGpuTemperatures.Add(int.Parse(gpuTempsAndFans[i]));
                else
                    _model.PerGpuFanSpeed.Add(int.Parse(gpuTempsAndFans[i]));
            }

            Debug.WriteLine($"{start.ElapsedMilliseconds}ms");
            start.Stop();
        }

        public void Dispose()
        {
            _tcpClient?.Dispose();
        }

        private async Task TryConnect(string minerIpAddress, int minerPort)
        {
            if (!_tcpClient.Connected)
            {
                _tcpClient.Close();
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(minerIpAddress, minerPort);
            }
        }
    }
}
