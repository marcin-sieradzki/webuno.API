
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Webuno.API.Models
{
    public class Game
    {
        public string Key { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public List<CardDto> CardsPlayed { get; set; } = new List<CardDto>();
        public List<CardDto> Deck { get; set; } = new List<CardDto>();
        public string CurrentPlayerTurn { get; set; }
        public string WinnerId { get; set; }
        public bool AreTurnsReversed { get; set; }
    }
}
 