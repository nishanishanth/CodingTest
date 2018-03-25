﻿using System;
using System.Linq;

namespace ATMMachine.BusinessLogic
{
    public class WithdrawalByLeastNumberOfItems : IWithdrawal
    {
        readonly AtmMoneyStore _moneyStore;

        public WithdrawalByLeastNumberOfItems(AtmMoneyStore moneyStore)
        {
            this._moneyStore = moneyStore;
        }

        public Cash Withdraw(double amountToWithdraw)
        {
            Cash cash = new Cash();
            var moneyStoreSortedByDenominationDescending = _moneyStore.AvailableCash.CoinOrNotes.OrderByDescending(c => c.Value);
            foreach(var coinOrNote in moneyStoreSortedByDenominationDescending)
            {
                if (amountToWithdraw.Equals(0))
                    break;
                int NumberOfCoinsOrNotes = (int)(amountToWithdraw / coinOrNote.Value);
                if (NumberOfCoinsOrNotes > 0)
                {
                    Denomination item = new Denomination { Type = coinOrNote.Type, Count = NumberOfCoinsOrNotes };
                    cash.CoinOrNotes.Add(item);
                    amountToWithdraw = Math.Round(amountToWithdraw - item.Value * item.Count, 2);
                }
            }
            UpdateCashBalanceInAtmMachine(cash);
            return cash;
        }

        private void UpdateCashBalanceInAtmMachine(Cash cash)
        {
            foreach(var coinOrNote in cash.CoinOrNotes)
            {
                _moneyStore.AvailableCash.CoinOrNotes.First<Denomination>(c => c.Type == coinOrNote.Type).Count -= coinOrNote.Count;
            }
        }
    }
}
