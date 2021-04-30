using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webuno.API.Services
{
    public interface IGameClient
    {
        Task GameStarted(string gameKey);
        Task PlayerJoined(string playerName);
    }
}
