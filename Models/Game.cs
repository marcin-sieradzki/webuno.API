
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Webuno.API.Models
{
    public class Game
    {
        public string Key { get; set; }
        public List<Player> Players { get; set; }
        public List<Card> CardsPlayed { get; set; }
        public Guid CurrentPlayerTurn { get; set; }
    }
}
