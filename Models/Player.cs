using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webuno.API.Models
{
    public class Player
    {
        [Key]
        public Guid Key { get; set; }

        public string Name { get; set; }

        public bool IsHost { get; set; }
        public string ConnectionId { get; set; }
        public int TurnIndex { get; set; }

        public List<PlayerCardDto> PlayerCards { get; set; }
    }
}
