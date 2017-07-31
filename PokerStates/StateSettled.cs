using System;

namespace PokerStates
{
    public class StateSettled
    {
        public Boolean IsSettled { get; set; }

        public override String ToString()
        {
            return IsSettled ? "SETTLED" : "UNSETTLED";
        }
    }
}
