using System;
using System.Threading.Tasks;
using Webuno.API.Models;

namespace Webuno.API.Services
{
    public interface IGameHub
    {
        Task<Game> StartGame(string hostName);
        Task JoinGame(Guid gameKey, string playerName);
        Task SendMessage(string message, string playerName, Guid gameKey);
    }
}
