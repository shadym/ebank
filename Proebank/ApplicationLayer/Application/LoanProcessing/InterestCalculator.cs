﻿using System;
using System.Linq;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    static class InterestCalculator
    {
        public static void CalculateInterestFor(Loan loan, DateTime date, Entry destinyEntry)
        {
            // very basic logic
            var payments =
                loan.PaymentSchedule.Payments.Where(
                    p => p.AccruedOn.HasValue && p.AccruedOn.Value.Date == date);

            destinyEntry.Amount = payments.Sum(p => p.Amount);
            destinyEntry.Currency = loan.Application.Currency;
            destinyEntry.Type = EntryType.Accrual;
            destinyEntry.SubType = EntrySubType.Interest;
            destinyEntry.Date = date;
        }
    }
}
