using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webuno.API.Extensions;
using Webuno.API.Models;
namespace Webuno.API.Services
{
    public class GameRepository : IGameRepository
    {
        private readonly WebunoDbContext _dbContext;
        private readonly ICardsRepository _cardsRepository;
        private const string CardsType = "card";
        public GameRepository(ICardsRepository cardsRepository, WebunoDbContext dbContext)
        {
            _cardsRepository = cardsRepository;
            _dbContext = dbContext;
        }

        public async Task<Game> GetGameAsync(string key)
        {
            var game = await _dbContext.Games.FirstOrDefaultAsync(game => game.Key == key);
            return game;
        }

        public async Task<Game> StartGameAsync(string playerName, string hostConnectionId)
        {
            try
            {
                Player host = await BuildHost(playerName, hostConnectionId);
                Game newGame = await BuildGame(host);

                await SaveGame(newGame).ConfigureAwait(false);

                return await GetGameAsync(newGame.Key.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private async Task SaveGame(Game newGame)
        {
            await _dbContext.Games.AddAsync(newGame);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task<Game> BuildGame(Player host)
        {
            var initialCard = await GetRandomCard();
            var newGame = new Game { Key = Guid.NewGuid().ToString(), Players = new List<Player> { host }, CardsPlayed = new List<CardDto> { initialCard }, CurrentPlayerTurn = host.Name, IsStarted = false };
            return newGame;
        }

        private async Task<Player> BuildHost(string playerName, string hostConnectionId)
        {
            var hostCards = await DrawPlayerStartingCards();
            var host = new Player { Name = playerName, Key = Guid.NewGuid(), PlayerCards = hostCards, IsHost = true, ConnectionId = hostConnectionId, TurnIndex = 1 };
            return host;
        }

        private async Task<CardDto> GetRandomCard()
        {
            var cards = await _cardsRepository.GetCardsAsync();
            var cardDtos = cards.Select(_ => _.ToDto()).ToList();
            Random rnd = new Random();
            var randomCard = cardDtos[rnd.Next(cardDtos.Count)];
            return randomCard;
        }

        private async Task<PlayerCardDto> DrawRandomCard(string playerName, Game game)
        {
            var randomCard = await GetRandomCard();
            var player = game.Players.FirstOrDefault(_ => _.Name == playerName);
            player.PlayerCards.Add(randomCard.ToPlayerDto());
            UpdateGameCurrentPlayerTurn(playerName, game);
            await UpdateGameAsync(game);
            return randomCard.ToPlayerDto();
        }

        private static void UpdateGameCurrentPlayerTurn(string playerName, Game game)
        {
            var playerTurnIndex = game.Players.FirstOrDefault(_ => _.Name == playerName).TurnIndex;
            game.CurrentPlayerTurn = game.Players.Count > 1 ? game.Players.FirstOrDefault(_ => _.TurnIndex == playerTurnIndex + 1).Name : playerName;
        }

        private async Task<List<PlayerCardDto>> DrawPlayerStartingCards()
        {
            var cards = await _cardsRepository.GetCardsAsync();
            Random rnd = new Random();
            return cards.Select(_ => _.ToPlayerDto()).OrderBy(x => rnd.Next()).Take(7).ToList();

        }

        public async Task<Game> JoinGameAsync(string playerName, Game game, string connectionId)
        {
            Player newPlayer = await BuildJoiningPlayer(playerName, game, connectionId);

            game.Players.Add(newPlayer);

            await UpdateGameAsync(game);

            return await GetGameAsync(game.Key);
        }

        private async Task<Player> BuildJoiningPlayer(string playerName, Game game, string connectionId)
        {
            var highestTurnIndex = game.Players?.Count != 0 ?  game.Players.Max(_ => _.TurnIndex) : 0;
            var newPlayer = new Player { Name = playerName, Key = Guid.NewGuid(), IsHost = false, PlayerCards = await DrawPlayerStartingCards(), TurnIndex = highestTurnIndex + 1, ConnectionId = connectionId };
            return newPlayer;
        }

        public async Task<Game> UpdateGameAsync(Game game)
        {
            _dbContext.Update(game);
            await _dbContext.SaveChangesAsync();
            return await GetGameAsync(game.Key);
        }
        public async Task<Game> PlayCardAsync(string gameKey, string playerName, string cardKey)
        {
            var card = await _cardsRepository.GetCardAsync(cardKey);
            if (card == null)
            {
                throw new Exception("Card does not exist");
            }

            CardDto cardDto = BuildCardDto(playerName, card);

            var game = await GetGameAsync(gameKey);

            if (game == null)
            {
                throw new Exception("Game does not exist");
            }

            UpdateGameCurrentPlayerTurn(playerName, game);

            game.CardsPlayed.Add(cardDto);
            var player = game.Players.FirstOrDefault(_ => _.Name == playerName);
            player.PlayerCards = player.PlayerCards.Where(_ => _.Key != cardKey).ToList();
            return await UpdateGameAsync(game);
        }

        private static CardDto BuildCardDto(string playerName, Card card)
        {
            return new CardDto()
            {
                Color = card.Color,
                Effect = card.Effect,
                Key = card.Key,
                PlayedBy = playerName,
                Symbol = card.Symbol,
                Type = card.Type
            };
        }
    }
}
