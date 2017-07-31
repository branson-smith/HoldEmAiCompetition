using System;
using System.Collections.Generic;
using System.Text;

namespace PokerStates
{
    public class Player
    {
        public Player()
        {
            
        }
        public Player(String name)
        {
            Name = name;
        }
        public String Name { get; set; }
        public Decision MakeDecision(Situation Situation)
        {
            Console.Write($"{Name}'s Action: ");
            string kbInput = Console.ReadLine();
            if (kbInput.Equals("raise"))
            {
                //TODO Read in raise amount
                return new DecisionRaise(500);
            }
            if (kbInput.Equals("call"))
            {
                return new DecisionCall();
            }
            if(kbInput.Equals("fold"))
            {
                return new DecisionFold();
            }
            return new DecisionFold();
        }
    }
}
