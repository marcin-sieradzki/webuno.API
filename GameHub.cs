using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Webuno.API.Models;
using Webuno.API.Services;

namespace Webuno.API
{
    public class GameHub : Hub, IGameHub
    {
        private readonly IGameRepository _gameRepository;
        public GameHub(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }
        public async Task<Game> StartGame(string hostName)

        {
            try
            {
                var game = await _gameRepository.StartGameAsync(hostName);
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Key.ToString());
                return game;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task JoinGame(Guid gameKey, string playerName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameKey.ToString());
            await Clients.OthersInGroup(gameKey.ToString()).SendAsync("PlayerJoined", playerName);
        }
        public async Task SendMessage(string message, string playerName, Guid gameKey)
        {
            await Clients.Group(gameKey.ToString()).SendAsync("MessageReceived", new { message, playerName, gameKey });
        }
    }
}
