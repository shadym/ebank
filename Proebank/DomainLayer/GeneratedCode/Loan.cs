﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Loan
{
	public virtual object ContractSignedDate
	{
		get;
		set;
	}

	public virtual object IssuanceDate
	{
		get;
		set;
	}

	public virtual Application Application
	{
		get;
		set;
	}

	public virtual IEnumerable<Payment> Payments
	{
		get;
		set;
	}

	public virtual LoanAccountList CreditAccountList
	{
		get;
		set;
	}

	public virtual LoanAccountList DebitAccountList
	{
		get;
		set;
	}

	public virtual Account GeneralDebtAccount
	{
		get;
		set;
	}

	public virtual Account PercentagesAccount
	{
		get;
		set;
	}

	public virtual Account AccumulatorAccount
	{
		get;
		set;
	}

}
