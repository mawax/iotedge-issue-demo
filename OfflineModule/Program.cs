using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace OfflineModule
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Init().Wait();
        }

        static async Task Init()
        {
            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");

            // Set breakpoint here, disconnect internet access manually, then resume
           var sw = new Stopwatch();
            for (var i = 0; i < 100; i++)
            {
                var obj = new { timestamp = DateTime.Now, test = true };
                var asJson = JsonSerializer.Serialize(obj);
                var twin = new TwinCollection(asJson);

                sw.Start();
                Console.WriteLine($"[{DateTime.Now:o}] Updating reported properties...");
                await ioTHubModuleClient.UpdateReportedPropertiesAsync(twin);
                Console.WriteLine($"[{DateTime.Now:o}] Updated reported properties. Duration: {sw.ElapsedMilliseconds}ms");
                sw.Reset();

                await Task.Delay(1000);
            }
        }
    }
}
