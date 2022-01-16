using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimalSelector.Data;
using AnimalSelector.AsyncDataService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AnimalSelector.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IMessageBusClient _messageBus;
        private readonly IHttpClientService _httpClient;

        public ImagesController(ILogger<ImagesController> logger, IMessageBusClient messageBus, IHttpClientService httpClient)
        {
            _logger = logger;
            _messageBus = messageBus;
            _httpClient = httpClient;
        }

        [HttpPost("single")]
        public async Task<IActionResult> CallSingleImage([FromBody] ImageRequestDto imageRequest)
        {
            string responseString = "";
            ImagesDto imagesDto = new ImagesDto();

            switch (imageRequest.animal)
            {
                case "dog":
                    responseString = await _httpClient.GetSingleDog();
                    DogDto dogImageDto = JsonConvert.DeserializeObject<DogDto>(responseString);
                    imagesDto.link = dogImageDto.message;
                break;

                case "cat":
                    responseString = await _httpClient.GetSingleCat();
                    CatDto catImageDto = JsonConvert.DeserializeObject<CatDto>(responseString);
                    imagesDto.link = catImageDto.url;
                break;
            }

            return Ok(imagesDto);
        }
    }
}