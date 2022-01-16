using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnimalSelector.AsyncDataService{
    public class HttpClientService : IHttpClientService
    {
        public async Task<string> GetSingleCat()
        {
            string responseString = "";

            using(var httpClient = new HttpClient()){
                using(var response = await httpClient.GetAsync("https://api.thecatapi.com/v1/images/search")){
                    responseString = await response.Content.ReadAsStringAsync();
                }
            }

            return responseString;
        }

        public async Task<string> GetSingleDog()
        {
            string responseString = "";

            using(var httpClient = new HttpClient()){
                using(var response = await httpClient.GetAsync("https://dog.ceo/api/breeds/image/random")){
                    responseString = await response.Content.ReadAsStringAsync();
                }
            }

            return responseString;
        }
    }
}