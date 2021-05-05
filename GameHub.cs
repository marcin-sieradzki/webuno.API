using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public override async Task OnDisconnectedAsync(Exception exception)
        {
   
            await base.OnDisconnectedAsync(exception);
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
        public async Task<Game> StartGame(string hostName)

        {
            try
            {
                var game = await _gameRepository.StartGameAsync(hostName, Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Key.ToString());
                return game;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<Game> JoinGame(Guid gameKey, string playerName)
        {
            var game = await _gameRepository.GetGameAsync(gameKey.ToString());

            if (IsAnyPlayerInTheGameWithSameName(playerName, game))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, gameKey.ToString());
                await Clients.OthersInGroup(gameKey.ToString()).SendAsync("PlayerReconnected", playerName);
                return game;
            }

            var joinedGame = await _gameRepository.JoinGameAsync(playerName, game);

            await Groups.AddToGroupAsync(Context.ConnectionId, joinedGame.ToString());
            await Clients.OthersInGroup(joinedGame.ToString()).SendAsync("PlayerJoined", playerName);
            return joinedGame;
        }
        private static bool IsAnyPlayerInTheGameWithSameName(string playerName, Game game)
        {
            return game.Players.Any(player => player.Name == playerName);
        }
        public async Task SendMessage(string message, string playerName, Guid gameKey)
        {
            await Clients.Group(gameKey.ToString()).SendAsync("MessageReceived", new { message, playerName, gameKey });
        }

        public async Task<Game> DisconnectFromGame(Guid gameKey, string playerName)
        {
            var game = await _gameRepository.GetGameAsync(gameKey.ToString());
            var playerToDisconnect = game.Players.FirstOrDefault(player => player.Name == playerName);
            game.Players.Remove(playerToDisconnect);
            await _gameRepository.UpdateGameAsync(game);
            await Groups.RemoveFromGroupAsync(playerToDisconnect.ConnectionId, gameKey.ToString());
            return game;
        }

        public async Task<Game> PlayCard(string gameKey, string playerName, Card card)
        {
            var game = await _gameRepository.PlayCardAsync(gameKey, playerName, card.Key);
            await Clients.Group(gameKey).SendAsync("CardPlayed", new { gameKey, playerName, card });
            return game;
        }

    }
}
