using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerStates
{
    public class Table
    {
        public Table()
        {
            Seats = new List<Seat>();
            SeatsInHand = new List<Seat>();
            Players = new List<Player>();
            Deck = new List<Card>();
            Board = new List<Card>();
            //TODO: SET Button and Blind Positions
            DealerPosition = 2;
            BigBlindPosition = 1;
            SmallBlindPosition = 0;
            BigBlindValue = 100;
            SmallBlindValue = 50;
        }

        public int BigBlindValue { get; set; }
        public int SmallBlindValue { get; set; }

        public List<Player> Players { get; set; }
        public List<Seat> Seats { get; set; }
        public List<Seat> SeatsInHand { get; set; }
        public int BigBlindPosition { get; set; }
        public int SmallBlindPosition { get; set; }
        public int DealerPosition { get; set; }
        public int Pot { get; set; }
        public int CurrentBet { get; set; }

        public List<Card> Deck { get; set; }
        public List<Card> Board { get; set; }

        public Card NextCard()
        {
            if (Deck.Count < 1)
            {
                throw new IndexOutOfRangeException();
            }
            var nextCard = Deck[0];
            Deck.RemoveAt(0);
            return nextCard;
        }

        public void GetPlayersFromSeats()
        {
            Players = new List<Player>();
            foreach (Seat s in Seats)
            {
                if (s.Player != null)
                {
                    Players.Add(s.Player);
                }
            }
        }

        public override String ToString()
        {
            String str = Seats.Aggregate($"Table-----cards: {Deck.Count}----Pot: {Pot}-----Bet: {CurrentBet}--------------------\n", 
                                        (current, seat) => current + $"{seat.ToString()}\n");
                                str += "--Board|";
            foreach (var card in Board)
            {
                str += card.ToString() + " ";
            }
            str += "--------------------------------";
            return str;
        }


        public void UnfoldAllSeatsInHand()
        {
            foreach (var seat in SeatsInHand)
            {
                seat.StateFolded.IsFolded = false;
            }
        }

        public void NewHand()
        {
            RefreshDeck();
            Board.Clear();
            DealAllSeatsInHand();            
        }

        public void DealAllSeatsInHand()
        {
            foreach (var seat in SeatsInHand)
            {
                seat.Hand.Add(NextCard());
                seat.Hand.Add(NextCard());
            }
        }

        public void SettleFoldedHands()
        {
            foreach (var seat in SeatsInHand)
            {
                if (seat.StateFolded.IsFolded)
                {
                    seat.StateSettled.IsSettled = true;
                }
            }
        }

        public void UnSettleAllSeatsInHand()
        {
            foreach (var seat in SeatsInHand)
            {
                seat.StateSettled.IsSettled = false;
            }
        }

        public Boolean AllSeatsInHandAreSettled()
        {
            foreach (var seat in SeatsInHand)
            {
                if (!(seat.StateSettled.IsSettled))
                {
                    return false;
                }
            }
            return true;
        }
        public Boolean MoreThanOneSeatInHandNotFolded()
        {
            int count = 0;
            foreach (var seat in SeatsInHand)
            {
                if (!(seat.StateFolded.IsFolded))
                {
                    count++;
                }
            }
            return (count > 1);
        }

        public void Flop()
        {
            Board.Add(NextCard());
            Board.Add(NextCard());
            Board.Add(NextCard());
            ClearBets();
        }
        public void Turn()
        {
            Board.Add(NextCard());
            ClearBets();
        }
        public void River()
        {
            Board.Add(NextCard());
            ClearBets();
        }
        public void RefreshDeck()
        {
            Deck = new List<Card>();
            for (int v = 2; v < 15; v++)
            {
                for (int s = 0; s < 4; s++)
                {
                    Deck.Add(new Card(v, s));
                }
            }
        }

        public void ClearBets()
        {
            foreach (Seat seat in SeatsInHand)
            {
                seat.Bet = 0;
            }
            CurrentBet = 0;
        }
        public bool HandleCall(int seatThatCalled)
        {
            var amountToCall = CurrentBet - SeatsInHand[seatThatCalled].Bet;
            if (SeatsInHand[seatThatCalled].Chips < amountToCall)
            { 
                SeatsInHand[seatThatCalled].Bet += SeatsInHand[seatThatCalled].Chips;
                Pot += SeatsInHand[seatThatCalled].Chips;
                SeatsInHand[seatThatCalled].Chips = 0;
                return true;
            }
            SeatsInHand[seatThatCalled].Bet += amountToCall;
            SeatsInHand[seatThatCalled].Chips -= amountToCall;
            Pot += amountToCall;
            return true;
        }

        public bool HandleRaise(int seatThatRaised, int amount)
        {
            var alreadyBet = SeatsInHand[seatThatRaised].Bet;
            SeatsInHand[seatThatRaised].Bet = amount;
            SeatsInHand[seatThatRaised].Chips -= (amount - alreadyBet);
            Pot += amount - alreadyBet;
            CurrentBet = amount;
            return true;
        }

        public void AwardChipsToWinner()
        {
            var winningSeat = SeatsInHand.First(seat => !seat.StateFolded.IsFolded);
            winningSeat.Chips += Pot;
        }
    }
}
