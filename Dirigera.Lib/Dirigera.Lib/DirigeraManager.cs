using Dirigera.Lib.Api;
using Dirigera.Lib.Api.Dto.Base;
using Dirigera.Lib.Constants;
using Dirigera.Lib.Models;
using Dirigera.Lib.Models.Base;
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

        internal ApiClient ApiClient => _apiClient;

        public Hub? Hub { get; set; }
        public List<Room> Rooms { get; set; } = new();
        public List<Scene> Scenes { get; set; } = new();
        public List<Device> Devices { get; set; } = new();
        public List<Light> Lights { get; set; } = new();
        public List<Blind> Blinds { get; set; } = new();
        public List<Outlet> Outlets { get; set; } = new();
        public List<EnvironmentSensor> EnvironmentSensors { get; set; } = new();

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

        public async Task LoadAll()
        {
            await GetHubDetails();
            await GetDevices();
            await GetRooms();
            await GetScenes();
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

            Devices = devices
                .Select(x => CreateDeviceByType(x))
                .ToList();

            Lights = Devices.Where(x => x is Light).Select(x => (Light)x).ToList();
            Blinds = Devices.Where(x => x is Blind).Select(x => (Blind)x).ToList();
            Outlets = Devices.Where(x => x is Outlet).Select(x => (Outlet)x).ToList();
            EnvironmentSensors = Devices.Where(x => x is EnvironmentSensor).Select(x => (EnvironmentSensor)x).ToList();
        }

        private Device CreateDeviceByType(DeviceDto dto)
        {
            return dto.DeviceType switch
            {
                DeviceType.LIGHT => new Light(this, dto),
                DeviceType.BLINDS => new Blind(this, dto),
                DeviceType.OUTLET => new Outlet(this, dto),
                DeviceType.ENVIRONMENT_SENSORS => new EnvironmentSensor(this, dto),
                _ => new Device(this, dto),
            };
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

        private async Task GetScenes()
        {
            var scenes = await _apiClient.GetScenes();
            if (scenes is null) return;

            Scenes = scenes
                .Select(x => new Scene(this)
                {
                    Id = x.Id,
                    Name = x.Info?.Name,
                    Icon = x.Info?.Icon,
                    Type = x.Type,
                    CreatedAt = x.CreatedAt,
                    LastCompleted = x.LastCompleted,
                    LastTriggered = x.LastTriggered,
                    UndoAllowedDuration = x.UndoAllowedDuration
                })
                .Where(x => x != null)
                .ToList();
        }

        public async Task SetLightState(Light light, bool state)
        {
            if (light is null || light.Id is null) return;
            await _apiClient.PatchAttributes(light.Id, new Dictionary<string, object>()
            {
                { "isOn", state }
            });
        }

        public async Task SetLightDimmer(Light light, int dimmer)
        {
            if (light is null || light.Id is null) return;
            await _apiClient.PatchAttributes(light.Id, new Dictionary<string, object>()
            {
                { "lightLevel", dimmer }
            });
        }

        public async Task SetLightColorTemperature(Light light, int colorTemperature)
        {
            if (light is null || light.Id is null) return;
            await _apiClient.PatchAttributes(light.Id, new Dictionary<string, object>()
            {
                { "colorTemperature", colorTemperature }
            });
        }

        public async Task SetLightColor(Light light, double hue, double saturation)
        {
            if (light is null || light.Id is null) return;
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
            },
            DeviceType.LIGHT);
        }

        public async Task SetLightDimmer(Room room, int dimmer)
        {
            await _apiClient.PatchAttributesRoom(room.Id, new Dictionary<string, object>()
            {
                { "lightLevel", dimmer }
            },
            DeviceType.LIGHT);
        }

        public async Task SetLightColorTemperature(Room room, int colorTemperature)
        {
            await _apiClient.PatchAttributesRoom(room.Id, new Dictionary<string, object>()
            {
                { "colorTemperature", colorTemperature }
            },
            DeviceType.LIGHT);
        }

        public async Task SetLightColor(Room room, double hue, double saturation)
        {
            await _apiClient.PatchAttributesRoom(room.Id, new Dictionary<string, object>()
            {
                { "colorHue", hue },
                { "colorSaturation", saturation }
            },
            DeviceType.LIGHT);
        }

        public async Task SetLightColor(Room room, Color color)
        {
            await SetLightColor(room, color.GetHue(), color.GetSaturation());
        }

        public async Task SetBlind(Room room, int level)
        {
            await _apiClient.PatchAttributesRoom(room.Id, new Dictionary<string, object>()
            {
                { "blindsTargetLevel", level }
            },
            DeviceType.LIGHT);
        }

        public async Task SetBlind(Blind blind, int level)
        {
            if (blind is null || blind.Id is null) return;
            await _apiClient.PatchAttributes(blind.Id, new Dictionary<string, object>()
            {
                { "blindsTargetLevel", level }
            });
        }

        public async Task SetOutlet(Outlet outlet, bool state)
        {
            if (outlet is null || outlet.Id is null) return;
            await _apiClient.PatchAttributes(outlet.Id, new Dictionary<string, object>()
            {
                { "isOn", state }
            });
        }

        public async Task TriggerScene(Scene scene)
        {
            if (scene is null || scene.Id is null) return;
            await _apiClient.TriggerScene(scene.Id);
        }

        public async Task UndoScene(Scene scene)
        {
            if (scene is null || scene.Id is null) return;
            await _apiClient.UndoScene(scene.Id);
        }


        /// <summary>
        /// Create a <see cref="DirigeraManager"/> object by automatically discovering your IKEA DIRIGERA hub on your network.
        /// </summary>
        /// <param name="authToken">An optional authentication token if you have authenticated with the hub previously.</param>
        /// <param name="timeout">The amount of time (in milliseconds) allowed for discovery.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async static Task<DirigeraManager> Discover(string? authToken = null, int timeout = 1000)
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
