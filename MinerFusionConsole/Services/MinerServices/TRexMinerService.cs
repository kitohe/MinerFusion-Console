using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using MinerFusionConsole.BuildingBlocks;
using MinerFusionConsole.Models.Miners;
using MinerFusionConsole.Models.Miners.TRexMiner;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinerFusionConsole.Services.MinerServices
{
    public class TRexMinerService : IMinerService
    {
        private readonly HttpClient _httpClient;

        private readonly string _uri;

        private readonly BaseMinerModel _model;

        public TRexMinerService(string minerId, string userId, string minerName, string minerIpAddress,
            int minerPort = 4067)
        {
            _httpClient = new HttpClient();

            _uri = new UriBuilder("http", minerIpAddress, minerPort, "summary").Uri.ToString();
            _model = new BaseMinerModel(minerId, userId, minerName);
        }

        public async Task<BaseMinerModel> GetMinerStatus()
        {
            _model.FlushModelFields();

            UnpackMinerStatus(await DownloadMinerStatus());

            return _model;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private async Task<object> DownloadMinerStatus()
        {
            try
            {
                var message = await _httpClient.GetStringAsync(_uri);
                return JsonUtils.ExtractJsonObject(message);
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"Exception: {e.Message}");
                return null;
            }
        }

        private void UnpackMinerStatus(object minerStatus)
        {
            if (minerStatus == null) return;

            JObject jsonObject;

            try
            {
                jsonObject = (JObject) minerStatus;
            }
            catch (InvalidCastException)
            {
                _model.MinerAlive = false;
                return;
            }

            var data = JsonConvert.DeserializeObject<TRexMinerModel>(jsonObject.ToString());

            _model.MinerAlive = true;
            _model.TotalHashRate = data.TotalHashRate;
            _model.AcceptedShares = data.AcceptedShares;

            foreach (var gpu in data.Devices)
            {
                _model.PerGpuHashRate.Add(gpu.HashRate);
                _model.PerGpuFanSpeed.Add(gpu.FanSpeed);
                _model.PerGpuTemperatures.Add(gpu.Temperature);
                _model.PerGpuShares.Add(0); // per gpu shares are not supported in TRex
            }
        }

        private string DummyMinerResponse()
        {
            return
                "{\"accepted_count\":6,\"active_pool\":{\"difficulty\":5,\"ping\":97,\"retries\":0,\"url\":\"stratum+tcp://...\",\"user\":\"...\"},\"algorithm\":\"x16r\",\"api\":\"1.2\",\"cuda\":\"9.10\",\"description\":\"T-Rex NVIDIA GPU miner\",\"difficulty\":31968.245093004043,\"gpu_total\":1,\"gpus\":[{\"device_id\":0,\"fan_speed\":66,\"gpu_id\":0,\"hashrate\":4529054,\"hashrate_day\":5023728,\"hashrate_hour\":0,\"hashrate_minute\":4671930,\"intensity\":21.5,\"name\":\"GeForce GTX 1050\",\"temperature\":80,\"vendor\":\"Gigabyte\",\"disabled\":true,\"disabled_at_temperature\":77}],\"hashrate\":4529054,\"hashrate_day\":5023728,\"hashrate_hour\":0,\"hashrate_minute\":4671930,\"name\":\"t-rex\",\"os\":\"linux\",\"rejected_count\":0,\"solved_count\":0,\"ts\":1537095257,\"uptime\":108,\"version\":\"0.6.5\",\"updates\":{\"url\":\"https://fileurl\",\"md5sum\":\"md5...\",\"version\":\"0.8.0\",\"notes\":\"short update info\",\"notes_full\":\"full update info\",\"download_status\":{\"downloaded_bytes\":1775165,\"total_bytes\":5245345,\"last_error\":\"\",\"time_elapsed_sec\":2.887111,\"update_in_progress\":true,\"update_state\":\"downloading\",\"url\":\"https://fileurl\"}}}";
        }
    }
}
