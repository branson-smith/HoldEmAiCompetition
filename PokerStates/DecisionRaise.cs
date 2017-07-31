using System;
using System.Collections.Generic;
using System.Text;

namespace PokerStates
{
    public class DecisionRaise : Decision
    {
        public DecisionRaise(int amount)
        {
            Name = "RAISE";
            Amount = amount;
        }
    }
}
