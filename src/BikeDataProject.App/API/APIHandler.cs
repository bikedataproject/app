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
            client.BaseAddress = new Uri(Constants.BaseAPIUri);
        }

        public async Task<bool> SendTracks(Track track)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, Constants.StoreTrackEndPoint) 
            { 
                Content = new StringContent(JsonConvert.SerializeObject(track), Encoding.UTF8, Constants.ApplicationJson)
            };

            var response =  await client.SendAsync(requestMessage);

            return response.IsSuccessStatusCode;
        }
    }
}
