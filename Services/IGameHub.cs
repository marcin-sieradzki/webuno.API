using System;
using System.Threading.Tasks;
using Webuno.API.Models;

namespace Webuno.API.Services
{
    public interface IGameHub
    {
        Task<Game> StartGame(string hostName);
        Task<Game> JoinGame(Guid gameKey, string playerName);
        Task<Game> DisconnectFromGame(Guid gameKey, string playerName);
        Task SendMessage(string message, string playerName, Guid gameKey);
    }
}
