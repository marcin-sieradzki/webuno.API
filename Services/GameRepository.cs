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
                Game newGame = await BuildGame();
                Player host = await BuildHost(playerName, hostConnectionId, newGame);
                newGame.Players.Add(host);
                newGame.CurrentPlayerTurn = host.Name;

                 _dbContext.Games.Add(newGame);
                await _dbContext.SaveChangesAsync();

                return await GetGameAsync(newGame.Key.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private async Task<Game> BuildGame()
        {
            var deck = await _cardsRepository.GetCardsAsync();
            var deckDtos = deck.Select(_=>_.ToDto()).ToList();
            var newGame = new Game { Key = Guid.NewGuid().ToString(), CardsPlayed = new List<CardDto>(), IsStarted = false, Deck = deckDtos };
            var initialCard = DrawRandomCardFromDeck(newGame.Deck);
            newGame.CardsPlayed.Add(initialCard);
            return newGame;
        }

        private async Task<Player> BuildHost(string playerName, string hostConnectionId, Game game)
        {
            var hostCards = DrawPlayerStartingCards(game.Deck);
            var host = new Player { Name = playerName, Key = Guid.NewGuid(), PlayerCards = hostCards, IsHost = true, ConnectionId = hostConnectionId, TurnIndex = 1, SitIndex = 1 };
            return host;
        }

        private CardDto DrawRandomCardFromDeck(List<CardDto> deck)
        {
            Random rnd = new Random();
            var randomCard = deck[rnd.Next(deck.Count)];
            deck.Remove(randomCard);
            return randomCard;
        }

        public async Task<PlayerCardDto> DrawRandomCard(string playerName, Game game)
        {
            var randomCard = DrawRandomCardFromDeck(game.Deck).ToPlayerDto();
            var player = game.Players.FirstOrDefault(_ => _.Name == playerName);
            player.PlayerCards.Add(randomCard);
            UpdateGameCurrentPlayerTurn(playerName, game);
            await UpdateGameAsync(game);
            return randomCard;
        }
        public async Task<List<PlayerCardDto>> DrawRandomCards(string playerName, int amount, Game game)
        {
            var cards = game.Deck;
            var player = game.Players.FirstOrDefault(_ => _.Name == playerName);
            Random rnd = new Random();
            var drawnCards = cards.OrderBy(x => rnd.Next()).Take(amount).ToList();
            RemoveCardsFromDeck(game.Deck, drawnCards);
            await UpdateGameAsync(game);
            return drawnCards.Select(card => card.ToPlayerDto()).ToList();
        }

        private static void RemoveCardsFromDeck(List<CardDto> deck, List<CardDto> cardsToRemove)
        {
            cardsToRemove.ForEach(card => deck.Remove(card));
        }

        private static void UpdateGameCurrentPlayerTurn(string playerName, Game game)
        {
            var playerTurnIndex = game.Players.FirstOrDefault(_ => _.Name == playerName).TurnIndex;
            if (game.Players.Any(p => p.TurnIndex == playerTurnIndex + 1))
            {
                game.CurrentPlayerTurn = game.Players.FirstOrDefault(_ => _.TurnIndex == playerTurnIndex + 1).Name;
                return;
            }
            game.CurrentPlayerTurn = game.Players.FirstOrDefault(_ => _.TurnIndex == 1).Name;
        }

        private List<PlayerCardDto> DrawPlayerStartingCards(List<CardDto> deck)
        {
            Random rnd = new Random();
            var cards =  deck.OrderBy(x => rnd.Next()).Take(7).ToList();
            RemoveCardsFromDeck(deck, cards);
            return cards.Select(card => card.ToPlayerDto()).ToList();
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
            var highestTurnIndex = game.Players?.Count != 0 ? game.Players.Max(_ => _.TurnIndex) : 0;
            var highestSitIndex = game.Players?.Count != 0 ? game.Players.Max(_ => _.SitIndex) : 0;
            var newPlayer = new Player { Name = playerName, Key = Guid.NewGuid(), IsHost = false, PlayerCards =  DrawPlayerStartingCards(game.Deck), TurnIndex = highestTurnIndex + 1, ConnectionId = connectionId, SitIndex = highestSitIndex + 1 };
            return newPlayer;
        }

        public async Task<Game> UpdateGameAsync(Game game)
        {
            _dbContext.Update(game);
            await _dbContext.SaveChangesAsync();
            return await GetGameAsync(game.Key);
        }
        public async Task<Game> PlayCardAsync(string gameKey, string playerName, Card card)
        {
            try
            {
                var game = await GetGameAsync(gameKey);
                if (game == null)
                {
                    throw new Exception("Game does not exist");
                }

                CheckIFCardCanBePlayed(card, game);

                CardDto cardDto = BuildCardDto(playerName, card);


                game.CardsPlayed.Add(cardDto);
                game.Deck.Remove(cardDto);
                var player = game.Players.FirstOrDefault(_ => _.Name == playerName);
                player.PlayerCards = player.PlayerCards.Where(_ => _.Key != card.Key).ToList();

                if (game.Players.Any(player => player.PlayerCards.Count < 1))
                {
                    game.WinnerId = game.Players.FirstOrDefault(_ => _.PlayerCards.Count < 1).Key.ToString();
                    return await UpdateGameAsync(game);
                }

                UpdateGameCurrentPlayerTurn(playerName, game);
                await _dbContext.SaveChangesAsync();
                return await GetGameAsync(game.Key);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static void CheckIFCardCanBePlayed(Card card, Game game)
        {
            var latestPlayedCard = game.CardsPlayed.OrderBy(_ => _.PlayedAt).Last();
            if (!(latestPlayedCard.Symbol == card.Symbol || latestPlayedCard.Color == card.Color || latestPlayedCard.Color == "inherit" || card.Color == "inherit"))
            {
                throw new Exception("This card cannot be played");
            }
        }

        private static CardDto BuildCardDto(string playerName, Card card)
        {
            return new CardDto()
            {
                Id = card.Id,
                Color = card.Color,
                Effect = card.Effect,
                Key = card.Key,
                PlayedBy = playerName,
                Symbol = card.Symbol,
                Type = card.Type,
                PlayedAt = DateTime.UtcNow
            };
        }
    }
}
