﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.FinancialFunctions;
using Domain.Models;

namespace Application.AccountProcessing
{
    static class FeeCalculator
    {
        public static void  Calculate()
        {
            var a = Interest.InterestRate(4, 5);
        }

        public static EntryModel AccrualFor(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
