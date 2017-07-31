using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

namespace PokerStates
{
    public class DecisionFold : Decision
    {
        public DecisionFold()
        {
            Name = "Fold";
            Amount = 0;
        }
    }
}
