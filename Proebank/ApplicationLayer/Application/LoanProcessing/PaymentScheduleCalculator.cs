﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Domain.Enums;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    public class PaymentScheduleCalculator
    {
        public static PaymentSchedule Calculate(decimal loanAmount, Tariff tariff, int term, DateTime? startDate = null)
        {
            if (tariff == null)
            {
                throw new ArgumentNullException("tariff");
            }
            if (term > tariff.MaxTerm || term < tariff.MinTerm)
            {
                throw new ArgumentException(String.Format("Term is not within the range of Tariff : {0}", tariff.Name));
            }
            if (loanAmount > tariff.MaxAmount || loanAmount < tariff.MinAmount)
            {
                throw new ArgumentException(String.Format("Sum is not within the range of Tariff : {0}", tariff.Name));
            }

            switch (tariff.PmtType)
            {
                case PaymentCalculationType.Annuity:
                    return CalculateAnnuitySchedule(tariff, loanAmount, term, startDate);
                case PaymentCalculationType.Standard:
                    return CalculateStandardSchedule(tariff, loanAmount, term, startDate);
                default:
                    throw new ArgumentException("Unknown payment calculation type: " + tariff.PmtType);
            }
        }

        internal static PaymentSchedule Calculate(LoanApplication loanApplication)
        {
            var status = loanApplication.Status;
            DateTime? startDate;
            switch (status)
            {
                case LoanApplicationStatus.Contracted:
                    startDate = loanApplication.TimeContracted;
                    break;
                case LoanApplicationStatus.Approved:
                case LoanApplicationStatus.New:
                    startDate = loanApplication.TimeCreated;
                    break;
                default:
                    throw new ArgumentException("schedule should not be calculated for application in status " + loanApplication.Status);
            }
            if (startDate == null)
            {
                throw new Exception("Loan application of its status must have its time (created or contracted)");
            }

            var tariff = loanApplication.Tariff;
            var loanAmount = loanApplication.LoanAmount;
            var term = loanApplication.Term;

            var schedule = Calculate(loanAmount, tariff, term, startDate);
            schedule = RoundPayments(schedule, 2);
            return schedule;
        }

        public static PaymentSchedule CalculateAnnuitySchedule(Tariff tariff, decimal loanAmount, int term, DateTime? startDate)
        {
            var rate = tariff.InterestRate * tariff.PmtFrequency / 12;
            var helpCoeff = PowDecimal(1 + rate, term);
            var annuityCoeff = (rate * helpCoeff) / (helpCoeff - 1);
            var monthlyPayment = loanAmount * annuityCoeff;

            var schedule = new PaymentSchedule();
            var remainMainDebt = loanAmount;
            if (startDate.HasValue)
            {
                var firstPmt = CalculateFirstPmt(loanAmount, tariff, startDate.Value);
                schedule.AddPayment(firstPmt);
                startDate = firstPmt.AccruedOn;
            }
            for (var i = 0; i < term; i++)
            {
                var pmtInterest = remainMainDebt * rate;
                var pmtMainDebt = monthlyPayment - pmtInterest;
                var pmt = new Payment
                {
                    MainDebtAmount = pmtMainDebt,
                    AccruedInterestAmount = pmtInterest,
                    AccruedOn = CalculatePaymentDate(startDate, i),
                    ShouldBePaidBefore = CalculatePaymentDate(startDate, i + 1)
                };
                schedule.AddPayment(pmt);
                remainMainDebt -= pmtMainDebt;
            }
            // TODO: test and add final payment if needed

            return schedule;
        }

        private static PaymentSchedule CalculateStandardSchedule(Tariff tariff, decimal loanAmount, int term, DateTime? startDate)
        {
            var rate = tariff.InterestRate * tariff.PmtFrequency / 12;
            var pmtMainDebt = loanAmount / term;
            var schedule = new PaymentSchedule();
            var remainMainDebt = loanAmount;
            for (var i = 0; i < term; i++)
            {
                var pmtInterest = remainMainDebt * rate;
                var pmt = new Payment
                {
                    MainDebtAmount = pmtMainDebt,
                    AccruedInterestAmount = pmtInterest,
                    AccruedOn = CalculatePaymentDate(startDate, i),
                    ShouldBePaidBefore = CalculatePaymentDate(startDate, i + 1)
                };
                schedule.AddPayment(pmt);
                remainMainDebt -= pmtMainDebt;
            }
            return schedule;
        }

        private static Payment CalculateFirstPmt(decimal loanAmount, Tariff tariff, DateTime startDate)
        {
            var rate = tariff.InterestRate/12;
            // startDate is included to the interest accruals
            // first payment contains only interest - http://calculator-credit.ru/articles/credit-calc.html, see #6 description
            var interestValue = loanAmount*rate*((30 - startDate.Day + 1) / 30M);   // this says loans on 31 have no interest accruals
            var accruedOnYear = startDate.Year;
            var accruedOnMonth = startDate.Month;
            var daysInMonth = DateTime.DaysInMonth(accruedOnYear, accruedOnMonth);
            var accruedOnDay = daysInMonth >= 30 ? 30 : daysInMonth;
            // latest time on that day
            var accruedDate = new DateTime(accruedOnYear, accruedOnMonth, accruedOnDay).AddDays(1).AddTicks(-1);
            var pmt = new Payment
            {
                AccruedInterestAmount = interestValue,
                AccruedOn = accruedDate,
                IsPaid = false,
                MainDebtAmount = 0M,
                OverdueInterestAmount = 0M,
                OverdueMainDebtAmount = 0M,
                ShouldBePaidBefore = accruedDate.AddMonths(tariff.PmtFrequency),
            };
            return pmt;
        }

        private static DateTime? CalculatePaymentDate(DateTime? startDate, int i)
        {
            if (startDate == null) return null;
            var date = startDate.Value;
            var paymentDate = date.AddMonths(i);
            // This is for getting the first day of the month. It is not needed anymore
            //paymentDate = new DateTime(paymentDate.Year, paymentDate.Month, DateTime.DaysInMonth(paymentDate.Year, paymentDate.Month)).AddDays(1).AddTicks(-1);
            if (paymentDate.DayOfWeek == DayOfWeek.Saturday || paymentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                paymentDate = paymentDate.AddDays(paymentDate.DayOfWeek == DayOfWeek.Saturday ? 2 : 1);
            }
            return paymentDate;
        }

        private static decimal PowDecimal(decimal x, int y)
        {
            if (y < 0) throw new ArgumentException("y");
            var result = 1M;
            for (var i = 0; i < y; i++)
            {
                result *= x;
            }
            return result;
        }

        private static PaymentSchedule RoundPayments(PaymentSchedule paymentSchedule, int digitsAfterZero)
        {
            var resultPaymentSchedule = new PaymentSchedule();
            foreach (var payment in paymentSchedule.Payments)
            {
                if (payment == null) continue;
                resultPaymentSchedule.AddPayment(new Payment
                    {
                        MainDebtAmount = Math.Round(payment.MainDebtAmount, digitsAfterZero),
                        AccruedInterestAmount = Math.Round(payment.AccruedInterestAmount, digitsAfterZero),
                        OverdueMainDebtAmount = Math.Round(payment.OverdueMainDebtAmount, digitsAfterZero),
                        OverdueInterestAmount = Math.Round(payment.OverdueInterestAmount, digitsAfterZero)
                    });
            }
            return resultPaymentSchedule;
        }
    }
}
