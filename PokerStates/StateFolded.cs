using System;
using System.Collections.Generic;
using System.Text;

namespace PokerStates
{
    public class StateFolded
    {
        public Boolean IsFolded { get; set; }

        public override String ToString()
        {
            return IsFolded ? "FOLDED" : "NOT FOLDED";
        }
    }
}
