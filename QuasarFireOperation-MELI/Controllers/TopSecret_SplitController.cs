using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuasarFireOperation_MELI.Services;

namespace QuasarFireOperation_MELI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TopSecret_SplitController : ControllerBase
    {
        private static string[] arrSatellite = { "kenobi", "skywalker", "sato" };

        // GET:TopSecret_Split/satellite_name
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public IActionResult Get([FromQuery] string satellite_name)
        {
            if (!arrSatellite.Contains(satellite_name))
                return NotFound();

            SatelliteService satService = new SatelliteService();
            dynamic response = satService.getTopSecretSplitResponse(satellite_name);

            if (response == null)
                return NotFound();
            else
                return Ok(response);
        }

        // POST: TopSecret_Split/{satellite_name}
        [Route("{satellite_name}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public IActionResult Post(string satellite_name, [FromBody] dynamic satData = null)
        {
            
            if (satData == null || !arrSatellite.Contains(satellite_name))
                return NotFound();

            SatelliteService satService = new SatelliteService();
            bool response = satService.getTopSecretSplitResponse(satellite_name, satData);
            if (response)
                return Ok();
            else
                return NotFound();
        }
    }
}
