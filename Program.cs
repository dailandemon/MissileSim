using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace MissileSimulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 Missile simulator started...");

            //handle to skip SSL certificate validation, not recommended
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var httpClient = new HttpClient(handler);


            int missileId = 11; // Update this ID if needed
            string apiUrl = $"http://localhost:5208/api/equipment/{missileId}/location"; // Update base URL if needed

            double centerLat = 27.994402;
            double centerLng = -81.760254;
            double radius = 1.0;

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTc1MDQ1NzcxOX0.KuNQBdoSfoDU9LCYGOPveaFzfH25BfPw_XNrdYBwvdU"); // Replace with your actual token

            while (true)
            {
                for (int angle = 0; angle < 360; angle += 10)
                {
                    double radians = angle * Math.PI / 180;
                    double lat = centerLat + radius * Math.Cos(radians);
                    double lng = centerLng + radius * Math.Sin(radians);

                    var payload = new
                    {
                        latitude = lat,
                        longitude = lng
                    };

                    var json = JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await httpClient.PutAsync(apiUrl, content);
                        Console.WriteLine($"[✓] Updated to ({lat:F4}, {lng:F4}) - Status: {response.StatusCode}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[!] Failed: {ex.Message}");

                         var inner = ex.InnerException;
                         while (inner != null)
                            {
                                Console.WriteLine($"    Inner: {inner.Message}");
                                inner = inner.InnerException;
                            }
                    }

                    await Task.Delay(1000); // 1 update/sec
                }
            }
        }
    }
}
