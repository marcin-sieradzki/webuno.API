using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;
        private const string CardsType = "card";
        private const string CardsCacheKey = "card";
        private static readonly TimeSpan DefaultCacheSlidingExpiration = TimeSpan.FromDays(365);
        public CardsRepository(WebunoDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }
        public async Task<List<Card>> GetCardsAsync()
        {
            return await _cache
            .GetOrCreateAsync(CardsCacheKey, entry =>
            {
                entry.SlidingExpiration = DefaultCacheSlidingExpiration;
                return GetFreshCardsAsync();
            });

        }

        private async Task<List<Card>> GetFreshCardsAsync()
        {
            return await _dbContext.Cards.Where(_ => _.Type == CardsType).ToListAsync();
        }

        public async Task<Card> GetCardAsync(string key)
        {
            return await _dbContext.Cards.FirstOrDefaultAsync(_ => _.Type == CardsType && _.Key == key);
        }
    }
}
