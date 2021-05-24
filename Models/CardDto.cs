using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webuno.API.Models
{
    public class CardDto
    {
        public string Id { get; set; }
        public string Key { get; set; }

        public string Symbol { get; set; }

        public string Type { get; set; }

        public string Color { get; set; }

        public string Effect { get; set; }
        public string PlayedBy { get; set; }
        public DateTime PlayedAt { get; set; }
    }
}
