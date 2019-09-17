using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeFrames;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CodeFramesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private IMemoryCache _cache;

        public GameController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        [HttpGet]
        public ActionResult<Game> GetState()
        {
            var gameState = (Game)_cache.Get("CodeFrames");
            return gameState;
        }

        [HttpPost]
        [Route("guess/{id}")]
        public ActionResult<Game> PostGuess([FromRoute]int id)
        {
            var gameState = (Game)_cache.Get("CodeFrames");
            gameState.Guess(id);
            return gameState;
        }

        [HttpPost]
        [Route("pass")]
        public ActionResult<Game> PostPass()
        {
            var gameState = (Game)_cache.Get("CodeFrames");
            gameState.Pass();
            return gameState;
        }

        [HttpPost]
        [Route("reset")]
        public ActionResult<Game> PostNewGame()
        {
            var gameState = (Game)_cache.Get("CodeFrames");
            gameState.Reset();
            return gameState;
        }
    }
}
