using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using MinerFusionConsole.BuildingBlocks;
using MinerFusionConsole.Models.Miners;
using Newtonsoft.Json.Linq;

namespace MinerFusionConsole.Services.MinerServices
{
    public class BMiner : IMinerService
    {
        private readonly HttpClient _httpClient;

        private readonly string _uri;

        private readonly BaseMinerModel _model;

        public BMiner(string minerId, string userId, string minerName, string minerIpAddress,
            int minerPort = 1880)
        {
            _httpClient = new HttpClient();

            _uri = new UriBuilder("http", minerIpAddress, minerPort).Uri.ToString();
            _model = new BaseMinerModel(minerId, userId, minerName);
        }

        public async Task<BaseMinerModel> GetMinerStatus()
        {
            _model.FlushModelFields();

            UnpackMinerStatus(await DownloadMinerStatus());

            return _model;
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
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
