using Dirigera.Lib;
using Dirigera.Lib.Extensions;

namespace Dirigera.Cli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 1)
            {
                var dirigera = await DirigeraManager.Discover(args[0]);
                Console.WriteLine($"IpAddress: {dirigera.IpAddress}");
                Console.WriteLine($"Authenticated?: {await dirigera.IsAuthenticated()}");

                await dirigera.Refresh();
                Console.Write(dirigera.Hub?.ToJson());
            }
            else
            {
                Console.WriteLine("No auth token specified, press ENTER to start authentication...");
                Console.ReadLine();
                await AuthenticateAutomatically();
            }
        }

        static async Task AuthenticateManually()
        {
            var hub = await DirigeraManager.Discover();
            await hub.StartAuthentication();
            Console.WriteLine("Press the Action-button on the DIRIGERA hub and then press ENTER...");
            Console.ReadLine();
            var authToken = await hub.FinishAuthentication();
            Console.WriteLine($"Sucessfully authenticated with the DIRIGERA hub. The authentication token is: {authToken}");
        }

        static async Task AuthenticateAutomatically()
        {
            var hub = await DirigeraManager.Discover();
            Console.WriteLine("Starting authentication process. Press the Action-button on the DIRIGERA hub now.");
            var authToken = await hub.Authenticate();
            Console.WriteLine($"Sucessfully authenticated with the DIRIGERA hub. The authentication token is: {authToken}");
        }
    }
}
