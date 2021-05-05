using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webuno.API.Models;

namespace Webuno.API.Services
{
    public class CardsRepository : ICardsRepository
    {
        private readonly WebunoDbContext _dbContext;
        private const string CardsType = "card";
        public CardsRepository(WebunoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Card>> GetCardsAsync()
        {
            return await _dbContext.Cards.Where(_ => _.Type == CardsType).ToListAsync();
        }

        public async Task<Card> GetCardAsync(string key)
        {
            return await _dbContext.Cards.FirstOrDefaultAsync(_ => _.Type == CardsType && _.Key == key);
        }
    }
}
