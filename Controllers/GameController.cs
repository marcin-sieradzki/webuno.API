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
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;
        public GameController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        /// <summary>Get game</summary>
        /// <param name="key">Game key</param>
        [HttpGet("{key}")]
        [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Game>> GetGameAsync([FromRoute] string key)
        {
            try
            {
                var game = await _gameRepository.GetGameAsync(key);
                return Ok(game);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        /// <summary>Start game</summary>
        /// <param name="playerName">Player name</param>
        [HttpPost("{playerName}/{connectionId}")]
        [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Game>> StartGameAsync([FromRoute]string playerName, [FromRoute] string hostConnectionId)
        {
            try
            {
                var game = await _gameRepository.StartGameAsync(playerName, hostConnectionId);
                return Ok(game);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

    }
}
