using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using MinerFusionConsole.Infrastructure;
using MinerFusionConsole.Models.Miners;
using Newtonsoft.Json;

namespace MinerFusionConsole.Services
{
    public class NetworkService : INetworkService
    {
        private readonly HttpClient _httpClient;

        private DiscoveryDocumentResponse _discoveryDocument;

        private bool _serviceIsUp;

        private readonly string _remoteServiceBaseUrl;

        private DateTime _tokenExpTime;

        public NetworkService()
        {
            _httpClient ??= new HttpClient();

            _remoteServiceBaseUrl = "https://miner.api.minerfusion.com/api/v1/Monitoring";
        }

        public async Task<bool> SendMinerData(BaseMinerModel data)
        {
            if (!_serviceIsUp)
                await Setup();
            
            if (_tokenExpTime < DateTime.UtcNow)
                await Setup();

            var uri = API.Miner.AddMinerData(_remoteServiceBaseUrl);

            var json = new StringContent(JsonConvert.SerializeObject(data).ToLower(), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, json);

            return response.IsSuccessStatusCode;
        }

        public async Task Setup()
        {
            _discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://accounts.minerfusion.com",
                Policy =
                {
                    ValidateIssuerName = true,
                    RequireHttps = true
                }
            });

            if (_discoveryDocument.IsError)
            {
                _serviceIsUp = false;
                return;
            }

            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,
                
                ClientId = "client",
                ClientSecret = "we_minin",
                Scope = "miner.api"
            });

            if (tokenResponse.IsError)
            {
                _serviceIsUp = false;
                return;
            }

            _tokenExpTime = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
            _httpClient.SetBearerToken(tokenResponse.AccessToken);
            _serviceIsUp = true;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
