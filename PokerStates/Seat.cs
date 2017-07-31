using System;
using System.Collections.Generic;
using System.Text;

namespace PokerStates
{
    public class Seat
    {

        public Seat()
        {
            LastDecision = new DecisionCall();
            StateSettled = new StateSettled() { IsSettled = true };
            StateFolded = new StateFolded() { IsFolded = true };
            Hand = new List<Card>();
        }

        public Player Player { get; set; }
        public int Chips { get; set; }
        public int Id { get; set; }
        public List<Card> Hand { get; set; } 
        public int Bet { get; set; }

        public Decision LastDecision { get; set; }
        public StateSettled StateSettled { get; set; }
        public StateFolded StateFolded { get; set; }

        public Decision GetPlayerDecision(Situation situation)
        {
            return Player.MakeDecision(situation);
        }

        public override string ToString()
        {
            string str = $"{Id}| ";

            if (Hand.Count == 2)
            {
                str += $"[{Hand[0].ToString()} {Hand[1].ToString()}] | ";
            }

            str += $"{Player.Name} | {Chips} | Bet = {Bet} | Folded = {StateFolded.IsFolded} | Settled = {StateSettled.IsSettled}";
            return str;
        }
    }
}
