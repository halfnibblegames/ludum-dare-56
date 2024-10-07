using System;
using System.Threading.Tasks;

namespace HalfNibbleGame.Objects.Cards;

public static class Cards
{
    private static readonly Random random = new();
    
    public static readonly Card[] AllCards = 
    {
        // new Sugar(),
        new Feature(),
        new RoyalGuard(),
        new MagnifierGlass(),
        new HornyJail(),
        new RoyalSuccession(),
        new CitronellaSpiral()
    };

    public static Card GetRandomCard()
        => AllCards[random.Next(AllCards.Length)];
}
