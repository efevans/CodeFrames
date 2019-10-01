using CodeFrames;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace CodeFramesAPI.Hubs
{
    public class GameHub : Hub
    {
        private IMemoryCache _cache;

        public GameHub(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public async Task SendNeedGameState()
        {
            await Clients.Caller.SendAsync("ReceiveGameUpdate", (Game)_cache.Get("CodeFrames"));
        }

        public async Task SendGuess(int id)
        {
            var gameState = (Game)_cache.Get("CodeFrames");
            gameState.Guess(id);
            await SendUpdate(gameState);
        }

        public async Task SendPass()
        {
            var gameState = (Game)_cache.Get("CodeFrames");
            gameState.Pass();
            await SendUpdate(gameState);
        }

        public async Task SendNewGame()
        {
            var gameState = (Game)_cache.Get("CodeFrames");
            gameState.Reset();
            await SendUpdate(gameState);
        }

        private async Task SendUpdate(Game game)
        {
            await Clients.All.SendAsync("ReceiveGameUpdate", game);
        }
    }
}
