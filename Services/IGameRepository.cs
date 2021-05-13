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
        Task<Game> StartGameAsync(string playerName, string hostConnectionId);
        Task<Game> UpdateGameAsync(Game game);
        Task<Game> JoinGameAsync(string playerName, Game game, string connectionId );
        Task<Game> PlayCardAsync(string gameKey, string playerName, string cardKey);
    }
}
