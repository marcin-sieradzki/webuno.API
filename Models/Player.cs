using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webuno.API.Models
{
    public class Player
    {

        public Guid Key { get; set; }

        public string Name { get; set; }

        public bool IsHost { get; set; }

        [NotMapped]
        public List<Card> Cards { get; set; }
    }
}
