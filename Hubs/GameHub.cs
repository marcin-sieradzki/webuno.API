using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webuno.API.Models;
using Webuno.API.Services;

namespace Webuno.API
{
    public class GameHub : Hub, IGameHub
    {
        private readonly IGameRepository _gameRepository;

        public GameHub(IGameRepository gameRepository, WebunoDbContext dbContext)
        {
            _gameRepository = gameRepository;
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
            try
            {
                var game = await _gameRepository.GetGameAsync(gameKey.ToString());

                if (IsAnyPlayerInTheGameWithSameName(playerName, game))
                {
                    await ReconnectPlayer(gameKey, playerName, game);
                    return game;
                }

                Game joinedGame = await JoinPlayerToGame(playerName, game);
                return joinedGame;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private async Task<Game> JoinPlayerToGame(string playerName, Game game)
        {
            var joinedGame = await _gameRepository.JoinGameAsync(playerName, game, Context.ConnectionId);
            Player player = GetPlayerByName(playerName, game);
            await Groups.AddToGroupAsync(Context.ConnectionId, joinedGame.ToString());
            await Clients.OthersInGroup(joinedGame.Key.ToString()).SendAsync("PlayerJoined", player);
            return joinedGame;
        }

        private async Task ReconnectPlayer(Guid gameKey, string playerName, Game game)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameKey.ToString());
            Player playerReconnecting = GetPlayerByName(playerName, game);
            playerReconnecting.ConnectionId = Context.ConnectionId;
            await _gameRepository.UpdateGameAsync(game);
            await Clients.OthersInGroup(gameKey.ToString()).SendAsync("PlayerReconnected", playerReconnecting);
        }

        private static Player GetPlayerByName(string playerName, Game game)
        {
            return game.Players.FirstOrDefault(_ => _.Name == playerName);
        }

        private static bool IsAnyPlayerInTheGameWithSameName(string playerName, Game game)
        {
            if (game.Players is null || game.Players.Count == 0)
            {
                return false;
            }
            return game.Players.Any(player => player.Name == playerName);
        }
        public async Task<object> SendMessage(string message, string playerName, Guid gameKey)
        {
            await Clients.Group(gameKey.ToString()).SendAsync("MessageReceived", new { message, playerName, gameKey });
            return new { message, playerName, gameKey };
        }

        public async Task<Game> DisconnectFromGame(Guid gameKey, string playerName)
        {
            var game = await _gameRepository.GetGameAsync(gameKey.ToString());
            var playerToDisconnect = game.Players.FirstOrDefault(player => player.Name == playerName);
            playerToDisconnect.ConnectionId = "";
            await _gameRepository.UpdateGameAsync(game);
            await Groups.RemoveFromGroupAsync(playerToDisconnect.ConnectionId, gameKey.ToString());
            return game;
        }

        public async Task<Game> PlayCard(string gameKey, string playerName, Card card)
        {
            try
            {
                var game = await _gameRepository.PlayCardAsync(gameKey, playerName, card);
                if (!string.IsNullOrEmpty(game.WinnerId))
                {
                    await Clients.Group(gameKey).SendAsync("GameWon", game.WinnerId);
                    return game;
                }
                await Clients.Group(gameKey).SendAsync("CardPlayed", new { playerName, card });
                return game;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Game> DrawRandomCard(string playerName, Game game)
        {
            try
            {
                var card = await _gameRepository.DrawRandomCard(playerName, game);
                var gameKey = game.Key;
                await Clients.Group(game.Key).SendAsync("CardDrew", new { gameKey, playerName, card });
                return game;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
