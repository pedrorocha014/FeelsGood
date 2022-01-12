using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimalSelector.Data;
using AnimalSelector.AsyncDataService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            _messageBus.PublishAnimalsRequest(imageRequest);

            return Ok();
        }
    }
}