using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerStates
{
    public class GameRunner 
    {
        private readonly List<HandState> _handStates;
        

        public GameRunner(Table table)
        {
            _handStates = new List<HandState>
            {
                new HandStateNew(),
                new HandStatePreFlop(),
                new HandStateFlop(),
                new HandStateTurn(),
                new HandStateRiver(),
                new HandStateShowDown(),
                new HandStateWrapUp()
            };

            Table = table;
            HandStateIndex = 0;
            NeedToWrapUp = false;
            IsRunning = true;
        }

        public bool NeedToWrapUp { get; set; }
        public Table Table { get; set; }
        public HandState GetHandState()
        {
            return _handStates[HandStateIndex];
        }
        public int HandStateIndex { get; set; }
        public bool IsRunning { get; set; }
        public HandState NextState()
        {
            if (HandStateIndex == _handStates.Count - 1)
            {
                HandStateIndex = 0;
                return _handStates[HandStateIndex];
            }
            if (NeedToWrapUp)
            {
                HandStateIndex = _handStates.Count - 1;
                return new HandStateWrapUp();
            }
            HandStateIndex = (HandStateIndex + 1) % _handStates.Count;
            return _handStates[HandStateIndex];
        }

        public void ActOnState()
        {
            var handState = GetHandState();

            if (handState.GetType() == typeof(HandStateNew))
            {
                ActionNewHand();
            }
            else if (handState.GetType() == typeof(HandStatePreFlop))
            {
                ActionPreFlop();
            }
            else if (handState.GetType() == typeof(HandStateFlop))
            {
                ActionFlop();
            }
            else if (handState.GetType() == typeof(HandStateTurn))
            {
                ActionTurn();
            }
            else if (handState.GetType() == typeof(HandStateRiver))
            {
                ActionRiver();
            }
            else if (handState.GetType() == typeof(HandStateShowDown))
            {
                ActionShowDown();
            }
            else if (handState.GetType() == typeof(HandStateWrapUp))
            {
                ActionWrapUp();
            }
            else
            { 
              throw new Exception("handState Does Not Exist");
            }
        }

        private void ActionWrapUp()
        {
            //TODO handle split pot
            var unfoldedHands = CountUnfoldedHands();
            if (unfoldedHands != 1)
            {
                Console.WriteLine("Not a clear winner.");
            }
            else
            {
                Table.AwardChipsToWinner();
            }
            Console.WriteLine("WRAPPING UP HAND");
            Console.ReadLine();
            NextState();
        }

        private void ActionShowDown()
        {
            //TODO Fold the losing hand(s)
            //TODO Evaluator
            Console.WriteLine("SHOWDOWN!");
            NeedToWrapUp = true;
            NextState();
        }

        private void ActionRiver()
        {
            Table.River();
            Table.UnSettleAllSeatsInHand();
            Table.SettleFoldedHands();
            var seatToActIndex = GetFirstSeatToActIndexPostFlop();
            while (!Table.AllSeatsInHandAreSettled())
            {
                if (!Table.MoreThanOneSeatInHandNotFolded())
                {
                    NeedToWrapUp = true;
                    NextState();
                    return;
                }
                if (!(Table.SeatsInHand[seatToActIndex].StateFolded.IsFolded))
                {
                    var situationForSeat = CreateSituation(Table, GetHandState());
                    var seatToActsDecision = Table.SeatsInHand[seatToActIndex].GetPlayerDecision(situationForSeat);
                    HandleSeatDecision(seatToActsDecision, seatToActIndex);
                }
                seatToActIndex = GetNextSeatToActIndex(seatToActIndex);
            }
            if (!Table.MoreThanOneSeatInHandNotFolded())
            {
                NeedToWrapUp = true;
                NextState();
                return;
            }
            NextState();
        }

        private void ActionTurn()
        {
            Table.Turn();
            Table.UnSettleAllSeatsInHand();
            Table.SettleFoldedHands();
            var seatToActIndex = GetFirstSeatToActIndexPostFlop();
            while (!Table.AllSeatsInHandAreSettled())
            {
                if (!Table.MoreThanOneSeatInHandNotFolded())
                {
                    NeedToWrapUp = true;
                    NextState();
                    return;
                }
                if (!(Table.SeatsInHand[seatToActIndex].StateFolded.IsFolded))
                {
                    var situationForSeat = CreateSituation(Table, GetHandState());
                    var seatToActsDecision = Table.SeatsInHand[seatToActIndex].GetPlayerDecision(situationForSeat);
                    HandleSeatDecision(seatToActsDecision, seatToActIndex);
                }
                seatToActIndex = GetNextSeatToActIndex(seatToActIndex);
            }
            if (!Table.MoreThanOneSeatInHandNotFolded())
            {
                NeedToWrapUp = true;
                NextState();
                return;
            }
            NextState();
        }

        private void ActionFlop()
        {
            Table.Flop();
            Table.ClearBets();
            Table.UnSettleAllSeatsInHand();
            Table.SettleFoldedHands();
            var seatToActIndex = GetFirstSeatToActIndexPostFlop();
            while (!Table.AllSeatsInHandAreSettled())
            {
                if (!Table.MoreThanOneSeatInHandNotFolded())
                {
                    NeedToWrapUp = true;
                    NextState();
                    return;
                }
                if (!(Table.SeatsInHand[seatToActIndex].StateFolded.IsFolded))
                {
                    var situationForSeat = CreateSituation(Table, GetHandState());
                    var seatToActsDecision = Table.SeatsInHand[seatToActIndex].GetPlayerDecision(situationForSeat);
                    HandleSeatDecision(seatToActsDecision, seatToActIndex);
                }
                seatToActIndex = GetNextSeatToActIndex(seatToActIndex);
            }
            if (!Table.MoreThanOneSeatInHandNotFolded())
            {
                NeedToWrapUp = true;
                NextState();
                return;
            }
            NextState();
        }

        private void ActionPreFlop()
        {
            Table.UnSettleAllSeatsInHand();
            var seatToActIndex = GetFirstSeatToActIndexPreFlop();
            Table.ClearBets();
            Table.CurrentBet = Table.BigBlindValue;
            // TODO ConsumeBlinds()
            Table.SeatsInHand[Table.BigBlindPosition].Bet = Table.BigBlindValue;
            Table.SeatsInHand[Table.BigBlindPosition].Chips -= Table.BigBlindValue;
            Table.SeatsInHand[Table.SmallBlindPosition].Bet = Table.SmallBlindValue;
            Table.SeatsInHand[Table.SmallBlindPosition].Chips -= Table.SmallBlindValue;
            Table.Pot += Table.BigBlindValue + Table.SmallBlindValue;

            while (!Table.AllSeatsInHandAreSettled())
            {
                if (!Table.MoreThanOneSeatInHandNotFolded())
                {
                    NeedToWrapUp = true;
                    NextState();
                    return;
                }
                if (!(Table.SeatsInHand[seatToActIndex].StateFolded.IsFolded))
                {
                    Console.WriteLine(Table.SeatsInHand[seatToActIndex].ToString());
                    var situationForSeat = CreateSituation(Table, GetHandState());
                    var seatToActsDecision = Table.SeatsInHand[seatToActIndex].GetPlayerDecision(situationForSeat);
                    HandleSeatDecision(seatToActsDecision, seatToActIndex);
                }
                seatToActIndex = GetNextSeatToActIndex(seatToActIndex);
            }
            if (!Table.MoreThanOneSeatInHandNotFolded())
            {
                NeedToWrapUp = true;
                NextState();
                return;
            }
            NextState();
        }
        private void ActionNewHand()
        {
            NeedToWrapUp = false;
            var seatsInHand = (from seat in Table.Seats
                               where seat.Player != null
                               && seat.Chips > 0
                               select seat).ToList();
            Table.SeatsInHand = seatsInHand;

            Table.UnfoldAllSeatsInHand();
            Table.NewHand();
            if (Table.SeatsInHand.Count < 2)
            {
                Console.WriteLine("Not enough players");
                Console.ReadLine();
            }
            NextState();
            
        }


        private void HandleSeatDecision(Decision seatToActsDecision, int seatThatActedIndex)
        {
            if (seatToActsDecision.GetType() == typeof(DecisionFold))
            {
                Console.WriteLine($"Seat {seatThatActedIndex} decided to FOLD");
                Fold(seatThatActedIndex);
            }
            else if (seatToActsDecision.GetType() == typeof(DecisionCall))
            {
                Console.WriteLine($"Seat {seatThatActedIndex} decided to CALL");
                Call(seatThatActedIndex);
            }
            else if (seatToActsDecision.GetType() == typeof(DecisionRaise))
            {
                Console.WriteLine($"Seat {seatThatActedIndex} decided to RAISE {seatToActsDecision.Amount}");
                Raise(seatThatActedIndex, seatToActsDecision.Amount);
            }
        }

        private Situation CreateSituation(Table table, HandState handState)
        {
            var copyOfTable = CopyTable(table);

            return new Situation(copyOfTable, handState);
        }

        private Table CopyTable(Table table)
        {
            var copyOfTable = new Table();
            copyOfTable.BigBlindPosition = table.BigBlindPosition;
            copyOfTable.SmallBlindPosition = table.SmallBlindPosition;
            copyOfTable.DealerPosition = table.DealerPosition;

            copyOfTable.Seats = CopySeats(table.Seats);
            copyOfTable.SeatsInHand = (from seat in copyOfTable.Seats
                                       where !(seat.StateFolded.IsFolded)
                                       select seat).ToList();
            copyOfTable.GetPlayersFromSeats();
            return copyOfTable;
        }

        private List<Seat> CopySeats(List<Seat> seats)
        {
            var copyOfSeats = new List<Seat>();

            foreach (var seat in seats)
            {
                copyOfSeats.Add(CopySeat(seat));
            }

            return copyOfSeats;
        }

        private Seat CopySeat(Seat seat)
        {
            var copyOfSeat = new Seat();

            copyOfSeat.Chips = seat.Chips;
            copyOfSeat.Id = seat.Id;
            copyOfSeat.StateFolded = new StateFolded {IsFolded = seat.StateFolded.IsFolded};
            copyOfSeat.StateSettled = new StateSettled {IsSettled = seat.StateSettled.IsSettled};
            copyOfSeat.LastDecision = seat.LastDecision;
            copyOfSeat.Player = new Player(seat.Player.Name);
            copyOfSeat.Hand = new List<Card>();

            return copyOfSeat;
        }
        private int GetFirstSeatToActIndexPreFlop()
        {
            var seatsInHand = Table.SeatsInHand;
            var numberSeatsInHand = seatsInHand.Count;
            var bigBlindPosition = Table.BigBlindPosition;
            var positionToActFirst = (bigBlindPosition + 1) % numberSeatsInHand;
            return positionToActFirst;
        }
        private int GetFirstSeatToActIndexPostFlop()
        {
            var seatsInHand = Table.SeatsInHand;
            var numberSeatsInHand = seatsInHand.Count;
            var smallBlindPosition = Table.SmallBlindPosition;
            var positionToActFirst = smallBlindPosition % numberSeatsInHand;
            return positionToActFirst;
        }
        private int GetNextSeatToActIndex(int seatToActIndex)
        {
            var nextSeatToActIndex = (seatToActIndex + 1) % Table.SeatsInHand.Count;
            return nextSeatToActIndex;
        }

        private void Fold(int seatThatFolded)
        {
            Table.SeatsInHand[seatThatFolded].StateFolded.IsFolded = true;
            Table.SeatsInHand[seatThatFolded].StateSettled.IsSettled = true;
        }
        private void Call(int seatThatCalled)
        {
            Table.SeatsInHand[seatThatCalled].StateSettled.IsSettled = true;
            Table.HandleCall(seatThatCalled);
        }
        private void Raise(int seatThatRaised, int amount)
        {
            Table.UnSettleAllSeatsInHand();
            Table.SettleFoldedHands();
            Table.SeatsInHand[seatThatRaised].StateSettled.IsSettled = true;
            Table.HandleRaise(seatThatRaised, amount);
        }

        private int CountUnfoldedHands()
        {
            return (from seat in Table.SeatsInHand where !(seat.StateFolded.IsFolded) select seat).ToList().Count;
        }
    }
}
