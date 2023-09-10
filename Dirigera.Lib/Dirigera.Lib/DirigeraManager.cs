using Dirigera.Lib.Api;
using Dirigera.Lib.Constants;
using Dirigera.Lib.Devices;
using Dirigera.Models;
using System.Drawing;
using Zeroconf;

namespace Dirigera.Lib
{
    public class DirigeraManager
    {
        private string? _pkceCode;
        private string? _code;

        private readonly ApiClient _apiClient;

        public Hub? Hub { get; set; }
        public List<Light> Lights { get; set; } = new();
        public List<Room> Rooms { get; set; } = new();

        public string IpAddress { get; }

        /// <summary>
        /// Create a <see cref="DirigeraManager"/> object using an IP address and an optional authentication token.
        /// You may also use <see cref="Discover(string?, int)"/> if you don't know the IP address of your hub.
        /// </summary>
        /// <param name="ipAddress">The IP address of your hub.</param>
        /// <param name="authToken">An optional authentication token if you have authenticated with the hub previously.</param>
        public DirigeraManager(string ipAddress, string? authToken = null)
        {
            IpAddress = ipAddress;
            _apiClient = new ApiClient(ipAddress, authToken);
        }

        public async Task Refresh()
        {
            await GetHubDetails();
            await GetDevices();
            await GetRooms();
        }


        private async Task GetHubDetails()
        {
            var dto = await _apiClient.GetHubDetails();
            if (dto is not null) Hub = new Hub(this, dto);
        }

        private async Task GetDevices()
        {
            var devices = await _apiClient.GetDevices();
            if (devices is null) return;

            var lights = devices
                .Where(x => x.DeviceType == DeviceType.LIGHT)
                .Select(x => new Light(this, x))
                .ToList();

             Lights = lights;
        }

        private async Task GetRooms()
        {
            var rooms = await _apiClient.GetRooms();
            if (rooms is null) return;

            Rooms = rooms
                .Select(x => new Room(x.Id, x.Name))
                .Where(x => x != null)
                .ToList();
        }

        public async Task SetLightState(Light light, bool state)
        {
            await _apiClient.PatchAttributes(light.Id, new Dictionary<string, object>()
            {
                { "isOn", state }
            });
        }

        public async Task SetLightDimmer(Light light, int dimmer)
        {
            await _apiClient.PatchAttributes(light.Id, new Dictionary<string, object>()
            {
                { "lightLevel", dimmer }
            });
        }

        public async Task SetLightColorTemperature(Light light, int colorTemperature)
        {
            await _apiClient.PatchAttributes(light.Id, new Dictionary<string, object>()
            {
                { "colorTemperature", colorTemperature }
            });
        }

        public async Task SetLightColor(Light light, double hue, double saturation)
        {
            await _apiClient.PatchAttributes(light.Id, new Dictionary<string, object>()
            {
                { "colorHue", hue },
                { "colorSaturation", saturation }
            });
        }

        public async Task SetLightColor(Light light, Color color)
        {
            await SetLightColor(light, color.GetHue(), color.GetSaturation());
        }

        public async Task SetLightState(Room room, bool state)
        {
            await _apiClient.PatchAttributesRoom(room.Id, new Dictionary<string, object>()
            {
                { "isOn", state }
            });
        }

        public async Task SetLightDimmer(Room room, int dimmer)
        {
            await _apiClient.PatchAttributesRoom(room.Id, new Dictionary<string, object>()
            {
                { "lightLevel", dimmer }
            });
        }

        public async Task SetLightColorTemperature(Room room, int colorTemperature)
        {
            await _apiClient.PatchAttributesRoom(room.Id, new Dictionary<string, object>()
            {
                { "colorTemperature", colorTemperature }
            });
        }

        public async Task SetLightColor(Room room, double hue, double saturation)
        {
            await _apiClient.PatchAttributesRoom(room.Id, new Dictionary<string, object>()
            {
                { "colorHue", hue },
                { "colorSaturation", saturation }
            });
        }

        public async Task SetLightColor(Room room, Color color)
        {
            await SetLightColor(room, color.GetHue(), color.GetSaturation());
        }


        /// <summary>
        /// Create a <see cref="DirigeraManager"/> object by automatically discovering your IKEA DIRIGERA hub on your network.
        /// </summary>
        /// <param name="authToken">An optional authentication token if you have authenticated with the hub previously.</param>
        /// <param name="timeout">The amount of time (in milliseconds) allowed for discovery.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async static Task<DirigeraManager> Discover(string? authToken = null, int timeout = 5000)
        {
            var results = await ZeroconfResolver.ResolveAsync("_ihsp._tcp.local.", TimeSpan.FromMilliseconds(timeout));

            if (results == null || !results.Any()) throw new Exception("No IKEA DIRIGERA hub found.");

            return new DirigeraManager(results[0].IPAddress, authToken);
        }

        public async Task<bool> IsAuthenticated()
        {
            return _apiClient.HasAuthenticationToken && await _apiClient.CheckAuthToken();
        }


        /// <summary>
        /// Start hub authentication process.
        /// The Action-button on the hub must be pressed AFTER calling this method,
        /// call <see cref="FinishAuthentication"/> after pressing the Action-button to receive the auth token.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StartAuthentication()
        {
            _pkceCode = ApiClient.GeneratePkceCodeVerifier();
            _code = await _apiClient.SendChallenge(_pkceCode);

            //if (_code is null) throw new Exception("Could not start authentication.");
            if (_code is null) return false;

            return true;
        }

        /// <summary>
        /// Finish hub authentication process.
        /// <see cref="StartAuthentication"/> must be called first.
        /// </summary>
        /// <returns>The authentication token. This token should be saved in your application for later use.</returns>
        public async Task<string?> FinishAuthentication()
        {
            if (_pkceCode is null || _code is null) throw new Exception($"{nameof(StartAuthentication)} must be called first.");

            var token = await _apiClient.GetToken(_code, _pkceCode);

            //if (token is null) throw new Exception($"Could not finish authentication. Make sure to press the Action-button on the hub after calling {nameof(StartAuthentication)}.");
            if (token is null) return null;

            _apiClient.AuthToken = token;
            return token;
        }

        /// <summary>
        /// Start hub authentication process with a timeout.
        /// The Action-button on the hub must be pressed within the timeout period.
        /// </summary>
        /// <returns></returns>
        public async Task<string?> Authenticate(int timeout = 60000, CancellationToken? cancellationToken = null)
        {
            try
            {
                using var ctsTimeout = new CancellationTokenSource(timeout);
                using var cts = cancellationToken is null ?
                    ctsTimeout : CancellationTokenSource.CreateLinkedTokenSource(ctsTimeout.Token, cancellationToken.Value);

                if (!await StartAuthentication()) return null;

                while (!cts.IsCancellationRequested)
                {
                    var authToken = await FinishAuthentication();
                    if (authToken is not null) return authToken;

                    await Task.Delay(1000, cts.Token);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
