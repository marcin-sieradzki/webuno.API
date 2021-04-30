using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Webuno.API.Services;

namespace Webuno.API
{
    public class GameHub : Hub
    {
        public async Task StartGame(string gameKey)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameKey);
        }
        public async Task JoinGame(string gameKey, string playerName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameKey);
            await Clients.OthersInGroup(gameKey).SendAsync("PlayerJoined", playerName);
        }
        public async Task SendMessage(string message, string playerName, string gameKey )
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameKey);
            await Clients.Group(gameKey).SendAsync("MessageReceived", new { message, playerName, gameKey });
        }
    }
}
