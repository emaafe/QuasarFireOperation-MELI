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
    public class TopSecretController : ControllerBase
    {
        // POST: TopSecret
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public IActionResult Post([FromBody] dynamic jsonValue = null)
        {
            SatelliteService satService = new SatelliteService();
            dynamic response = satService.getTopSecretResponse(jsonValue);

            if (response == null)
                return NotFound();
            else
                return Ok(response);
        }
    }
}
