using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webuno.API.Models;
using Webuno.API.Services;

namespace Webuno.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ICardsRepository _cardsRepository;
        public CardsController(ICardsRepository cardsRepository)
        {
            _cardsRepository = cardsRepository;
        }
        /// <summary>Get cards</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Card>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Card>>> GetCardsAsync()
        {
            try
            {
                var game = await _cardsRepository.GetCardsAsync();
                return Ok(game);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
