using BikeDataProject.App.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BikeDataProject.App.API
{
    public class APIHandler
    {
        HttpClient client;

        public APIHandler()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/");
        }

        public async Task<bool> SendTracks(Track track)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "Track/StoreTrack") 
            { 
                Content = new StringContent(JsonConvert.SerializeObject(track), Encoding.UTF8, "application/json")
            };

            var response =  await client.SendAsync(requestMessage);

            return response.IsSuccessStatusCode;
        }
    }
}
