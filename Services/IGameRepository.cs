using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webuno.API.Models;

namespace Webuno.API.Services
{
    public interface IGameRepository
    {
        Task<Game> GetGameAsync(string key);
        Task<Game> StartGameAsync(string playerName);
    }
}
