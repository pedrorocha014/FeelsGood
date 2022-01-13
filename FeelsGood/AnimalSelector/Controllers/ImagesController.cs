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

        public ImagesController(ILogger<ImagesController> logger, IMessageBusClient messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        [HttpPost("single")]
        public IActionResult CallSingleImage([FromBody] ImageRequestDto imageRequest)
        {
            string response = _messageBus.PublishAnimalsRequest(imageRequest);

            ImagesDto imagesDto = JsonConvert.DeserializeObject<ImagesDto>(response);

            return Ok(imagesDto);
        }
    }
}