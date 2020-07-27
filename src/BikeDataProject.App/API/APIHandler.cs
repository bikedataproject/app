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

        /// <summary>
        /// Send tracks to the server (POST)
        /// </summary>
        /// <param name="track">A track containing a list of locations and the userId</param>
        /// <returns>True if the tracks are succesfully send to the server</returns>
        public async Task<bool> SendTracks(Track track)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, Constants.StoreTrackEndPoint) 
            { 
                Content = new StringContent(JsonConvert.SerializeObject(track), Encoding.UTF8, Constants.ApplicationJson)
            };

            var response =  await client.SendAsync(requestMessage);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Gets a userInfo object containing the imei and the userId
        /// </summary>
        /// <param name="userInfo">A userInfo object containing the imei property</param>
        /// <returns>A userInfo object with the userId property filled in</returns>
        public async Task<UserInfo> GetUserId(UserInfo userInfo) 
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, Constants.UserIdEndPoint)
            {
                Content = new StringContent(JsonConvert.SerializeObject(userInfo), Encoding.UTF8, Constants.ApplicationJson)
            };

            var response = await client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<UserInfo>(content);
        }

        public async Task<WorldStatistics> GetWorldStatisticsAsync() 
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, Constants.WorldStatisticsEndPoint);

            var response = await client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<WorldStatistics>(content);
        }
    }
}
