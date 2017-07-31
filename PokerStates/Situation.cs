using System;
using System.Collections.Generic;
using System.Text;

namespace PokerStates
{
    public class Situation
    {
        public Table Table { get; }
        public HandState HandState { get; }
        public Situation(Table table, HandState handState)
        {
            Table = table;
            HandState = handState;
        }


    }
}
