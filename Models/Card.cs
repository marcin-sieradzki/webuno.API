using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Webuno.API.Models
{
    public class Card
    {
        public string Id { get; set; }
        
        public string Key { get; set; }

        public string Symbol { get; set; }
        
        public string Type { get; set; }

        public string Color { get; set; }

        public string Effect { get; set; }

    }
}
