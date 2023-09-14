using Dirigera.Lib.Api.Dto;
using Dirigera.Lib.Api.Dto.Base;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Dirigera.Lib.Api
{
    internal class ApiClient
    {
        private const string PKCE_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";

        private readonly string _ip;
        private string _apiBaseUrl;
        private string? _authToken;

        public string? AuthToken
        {
            get => _authToken; set
            {
                _authToken = value;
                if (value is not null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
                }
            }
        }

        private HttpClient _httpClient;

        public ApiClient(string ip, string? authToken = null)
        {
            _ip = ip;
            _apiBaseUrl = $"https://{_ip}:8443/v1";

            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }
            };

            _httpClient = new HttpClient(handler);
            AuthToken = authToken;
        }


        public bool HasAuthenticationToken => AuthToken != null;


        public async Task<bool> CheckAuthToken()
        {
            string url = $"{_apiBaseUrl}/hub/status";
            var res = await _httpClient.GetAsync(url);
            return res.StatusCode == HttpStatusCode.OK;
        }

        public async Task<HubDto?> GetHubDetails()
        {
            string url = $"{_apiBaseUrl}/hub/status";
            var res = await _httpClient.GetFromJsonAsync<HubDto>(url);
            return res;
        }

        public async Task<List<DeviceDto>?> GetDevices()
        {
            string url = $"{_apiBaseUrl}/devices";
            var res = await _httpClient.GetFromJsonAsync<List<DeviceDto>>(url);
            return res;
        }

        public async Task<List<RoomDto>?> GetRooms()
        {
            string url = $"{_apiBaseUrl}/rooms";
            var res = await _httpClient.GetFromJsonAsync<List<RoomDto>>(url);
            return res;
        }

        public async Task<T> GetDevice<T>(string id)
        {
            string url = $"{_apiBaseUrl}/devices/{id}";
            var res = await _httpClient.GetFromJsonAsync<T>(url);
            return res;
        }

        public async Task<HttpResponseMessage> PatchAttributes(string deviceId, Dictionary<string, object> attributes)
        {
            string url = $"{_apiBaseUrl}/devices/{deviceId}";

            var data = new List<object>()
            {
                new {
                    Attributes = attributes
                }
            };

            var res = await _httpClient.PatchAsJsonAsync(url, data);
            return res;
        }

        public async Task<HttpResponseMessage> PatchAttributesRoom(string roomId, Dictionary<string, object> attributes, string deviceType)
        {
            string url = $"{_apiBaseUrl}/devices/room/{roomId}?deviceType={deviceType}";

            var data = new List<object>()
            {
                new {
                    Attributes = attributes
                }
            };

            var res = await _httpClient.PatchAsJsonAsync(url, data);
            return res;
        }

        public async Task<string?> SendChallenge(string codeVerifier)
        {
            string authUrl = $"{_apiBaseUrl}/oauth/authorize";

            var query =
                $"?audience=homesmart.local&response_type=code&code_challenge={GeneratePkceCodeChallenge(codeVerifier)}&code_challenge_method=S256";

            var response = await _httpClient.GetAsync(authUrl + query);
            if (!response.IsSuccessStatusCode) return null;

            var res = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(res);
            return document?.RootElement.GetProperty("code").GetString();
        }

        public async Task<string?> GetToken(string code, string codeVerifier)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("name", System.Net.Dns.GetHostName()),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code_verifier", codeVerifier)
            });

            string tokenUrl = $"{_apiBaseUrl}/oauth/token";

            var response = await _httpClient.PostAsync(tokenUrl, content);
            if (!response.IsSuccessStatusCode) return null;

            var res = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(res);
            return document?.RootElement.GetProperty("access_token").GetString();
        }




        public static string GeneratePkceCodeVerifier(int length = 128)
        {
            if (length < 43 || length > 128)
            {
                throw new ArgumentException("PKCE Code Verifier length must be between 43 and 128 characters.");
            }
            byte[] randomBytes = RandomNumberGenerator.GetBytes(length);

            var sb = new StringBuilder(length);
            for (int i = 0; i < length; ++i)
            {
                var index = randomBytes[i] % PKCE_ALPHABET.Length;
                sb.Append(PKCE_ALPHABET[index]);
            }

            return sb.ToString();
        }

        public static string GeneratePkceCodeChallenge(string codeVerifier)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));

            // Convert to base64url, without padding
            string base64 = Convert.ToBase64String(bytes);
            string base64Url = base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');

            return base64Url;
        }
    }
}