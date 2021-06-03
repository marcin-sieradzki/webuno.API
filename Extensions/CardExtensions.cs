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
                Id = card.Id,
                Color = card.Color,
                Effect = card.Effect,
                Key = card.Key,
                PlayedBy = "",
                Symbol = card.Symbol,
                Type = card.Type
            };
        }
        public static PlayerCardDto ToPlayerDto(this Card card)
        {
            return new PlayerCardDto
            {
                Id = card.Id,
                Color = card.Color,
                Effect = card.Effect,
                Key = card.Key,
                Symbol = card.Symbol,
                Type = card.Type
            };
        }
        public static PlayerCardDto ToPlayerDto(this CardDto card)
        {
            return new PlayerCardDto
            {
                Id = card.Id,
                Color = card.Color,
                Effect = card.Effect,
                Key = card.Key,
                Symbol = card.Symbol,
                Type = card.Type,
                PlayedAt = card.PlayedAt
            };
        }

        public static Card ToCard(this CardDto card)
        {
            return new Card
            {
                Id = card.Id,
                Color = card.Color,
                Effect = card.Effect,
                Key = card.Key,
                Symbol = card.Symbol,
                Type = card.Type
            };
        }
        public static bool HasSpecialEffect(this CardDto card)
        {
            var specialCardSymbols = new List<string>() { "+2", "+4", "reverse", "stop" };
            return specialCardSymbols.Contains(card.Symbol);
        }
    }
}
