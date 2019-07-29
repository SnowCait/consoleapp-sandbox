using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApi.User32;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient(new Http2Handler());
        static async Task Main(string[] args)
        {
            LoadConfig();
            //await HttpRequestAsync();
            //Centering();

            Console.ReadLine();
        }

        private static void LoadConfig()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configBuilder.AddJsonFile(@"Config.json");
            var config = configBuilder.Build();
            var mode = config["Mode"];
            Console.WriteLine($"Mode: {mode}");
        }

        private static async Task HttpRequestAsync()
        {
            // Internet access
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://example.com/"))
            {
                requestMessage.Version = new Version(2, 0);
                var response = await httpClient.SendAsync(requestMessage);
                Console.WriteLine($"HTTP/{response.Version}");
            }
        }

        private static void Centering()
        {
            // ペイントの中央揃え
            using (var process = Process.GetProcessesByName("mspaint").FirstOrDefault())
            {
                if (process == null)
                {
                    Console.WriteLine($"No process.");
                    return;
                }
                var windowHandle = process.MainWindowHandle;

                var clientPoint = new NetCoreEx.Geometry.Point(0, 0);
                User32Methods.GetClientRect(windowHandle, out var clientRect);
                User32Methods.ClientToScreen(windowHandle, ref clientPoint);
                Console.WriteLine($"clientRect: {clientRect.ToString()}");
                Console.WriteLine($"clientPoint: {clientPoint.ToString()}");

                var screen = Screen.PrimaryScreen;
                Console.WriteLine($"PrimaryScreen: {screen.Bounds.ToString()}");
                // 小数点以下は切り捨て
                var x = (screen.Bounds.Width - clientRect.Width) / 2;
                var y = (screen.Bounds.Height - clientRect.Height) / 2;
                Console.WriteLine($"Move to ({x}, {y})");

                // Move Window
                User32Methods.MoveWindow(windowHandle, x, y, clientRect.Width, clientRect.Height, true);
            }
        }
    }

    class Http2Handler : WinHttpHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Version = new Version(2, 0);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
