﻿using System.Collections.Generic;
using Domain.Models;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    class LoanService
    {

        public IEnumerable<Loan> Loans
        {
            get;
            set;
        }

        public void CreateLoanContract()
        {
            throw new System.NotImplementedException();
        }

        public void CloseLoanContract()
        {
            throw new System.NotImplementedException();
        }

        public void RegisterPayment()
        {
            throw new System.NotImplementedException();
        }
    }
}