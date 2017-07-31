using System;
using System.Collections.Generic;

namespace PokerStates
{
    class Program
    {
        static void Main()
        {
            
            var table = new Table();

            const int startingChips = 1000;

            var branson = new Player(){ Name = "Branson" };
            var max = new Player()    { Name = "Max" };
            var trent = new Player()  { Name = "Trent" };

            var seat1 = new Seat() { Id = 1, Player = branson, Chips = startingChips };
            var seat2 = new Seat() { Id = 2, Player = max,     Chips = startingChips };
            var seat3 = new Seat() { Id = 3, Player = trent,   Chips = startingChips };

            table.Seats.Add(seat1);
            table.Seats.Add(seat2);
            table.Seats.Add(seat3);
            table.GetPlayersFromSeats();
            table.Deck = FreshDeck();

            var gameRunner = new GameRunner(table);
            while (gameRunner.IsRunning)
            {
                Console.WriteLine($"Hand State = {gameRunner.GetHandState().GetName()}");
                Console.WriteLine(table.ToString());
                gameRunner.ActOnState();
            }

        }

        static List<Card> FreshDeck()
        {
            var cards = new List<Card>();
            for (int v = 2; v < 15; v++)
            {
                for (int s = 0; s < 4; s++)
                {
                    cards.Add(new Card(v, s));
                }
            }
            return cards;
        }
    }
}