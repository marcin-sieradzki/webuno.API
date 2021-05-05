using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webuno.API.Models;

namespace Webuno.API.Services
{
    public interface ICardsRepository
    {
        Task<List<Card>> GetCardsAsync();
        Task<Card> GetCardAsync(string key);
    }
}
