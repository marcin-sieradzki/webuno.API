using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webuno.API.Models;

namespace Webuno.API.Extensions
{
    public static class CardExtensions
    {
        public static CardDto ToDto(this Card card)
        {
            return new CardDto
            {
                Color = card.Color,
                Effect = card.Effect,
                Key = card.Key,
                PlayedBy = "",
                Symbol = card.Symbol,
                Type = card.Type
            };
        }
    }
}
