using System.Diagnostics;
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
        private static HttpClient _httpClient;

        private static DiscoveryDocumentResponse _discoveryDocument;

        private static bool _serviceIsUp;

        private readonly string _remoteServiceBaseUrl;

        public NetworkService()
        {
            _httpClient ??= new HttpClient();

            _remoteServiceBaseUrl = "http://localhost:5100/api/v1/Monitoring";
        }

        public async Task SendMinerData(BaseMinerModel data)
        {
            if (!_serviceIsUp)
            {
                Debug.WriteLine("Trying to start networking service...");
                await Setup();
            }

            var uri = API.Miner.AddMinerData(_remoteServiceBaseUrl);

            var jsonString = JsonConvert.SerializeObject(data, Formatting.None);

            var json = new StringContent(JsonConvert.SerializeObject(data).ToLower(), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, json);

            response.EnsureSuccessStatusCode();

            Debug.WriteLine("Data sent successfully");
        }

        public async Task Setup()
        {
            _discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://localhost:5000",
                Policy =
                {
                    ValidateIssuerName = false,
                    RequireHttps = false
                }
            });

            if (_discoveryDocument.IsError)
            {
                Debug.WriteLine(_discoveryDocument.Error);
                _serviceIsUp = false;
                return;
            }

            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "miner.api"
            });

            if (tokenResponse.IsError)
            {
                Debug.WriteLine(tokenResponse.Error);
                _serviceIsUp = false;
                return;
            }

            _httpClient.SetBearerToken(tokenResponse.AccessToken);
            _serviceIsUp = true;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
