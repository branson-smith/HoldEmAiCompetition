using System;
using System.Collections.Generic;
using System.Text;

namespace PokerStates
{
    public class Card
    {
        public int Value { get; set; }
        public int Suit { get; set; }
        public Card(int value, int suit)
        {
            Value = value;
            Suit = suit;
        }

        public char[] ValueChars = new[]
        {
            'X', 'X', '2', '3', '4',
            '5', '6', '7', '8', '9',
            'T', 'J', 'Q', 'K', 'A'
        };
        public char[] SuitChars = new[]
        {
            'S', 'H', 'C', 'D'
        };

        public override string ToString()
        {
            return $"{ValueChars[Value]}{SuitChars[Suit]}";
        }
    }
}
