using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webuno.API.Models;

namespace Webuno.API.Services
{
    public class GameRepository : IGameRepository
    {
        private readonly WebunoDbContext _dbContext;

        public GameRepository(WebunoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Game> GetGameAsync(string key)
        {
            var game = await _dbContext.Games.FirstOrDefaultAsync(game => game.Key.Equals(key));
            return game;
        }

        public async Task<Game> StartGameAsync(string playerName)
        {
            try
            {

            var host = new Player { Name = playerName, Key = Guid.NewGuid(), Cards = new List<Card>(), IsHost = true };
            var newGame = new Game { Key = Guid.NewGuid().ToString(), Players = new List<Player> { host }, CardsPlayed = new List<Card>(), CurrentPlayerTurn = host.Key };
            
            await _dbContext.Games.AddAsync(newGame);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return await GetGameAsync(newGame.Key.ToString());
            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
