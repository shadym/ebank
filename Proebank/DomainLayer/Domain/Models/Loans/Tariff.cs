﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models.Loans
{
    public class Tariff : Entity
    {
        [DisplayName("Tariff Name")]
        public string Name { get; set; }
        
        [DisplayName("Interest Rate")]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public decimal InterestRate { get; set; }

        [DisplayName("Min Amount")]
        public decimal MinAmount { get; set; }

        [DisplayName("Max Amount")]
        public decimal MaxAmount { get; set; }

        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }

        [DisplayName("Active?")]
        public bool IsActive { get; set; }

        [DisplayName("Min Term")]
        public int MinTerm { get; set; }

        [DisplayName("Max Term")]
        public int MaxTerm { get; set; }

        [DisplayName("Payment frequency, in months")]
        [Range(1, 12)]
        public int PmtFrequency { get; set; }

        [DisplayName("Payment type")]
        public PaymentCalculationType PmtType { get; set; }

        [DisplayName("Min Age")]
        public ushort MinAge { get; set; }

        [DisplayName("Max Age")]
        public ushort? MaxAge { get; set; }

        [DisplayName("Initial Fee")]
        public decimal InitialFee { get; set; }

        [DisplayName("Guarantor")]
        public bool IsGuarantorNeeded { get; set; }

        [DisplayName("Purpose")]
        public LoanPurpose LoanPurpose { get; set; }

        [DisplayName("Currency")]
        public Currency Currency { get; set; }
    }
}
