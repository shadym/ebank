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

public class Transaction : Transaction
{
	public override object Amount
	{
		get;
		set;
	}

	public override object Time
	{
		get;
		set;
	}

	public virtual Employee Employee
	{
		get;
		set;
	}

	public virtual Account From
	{
		get;
		set;
	}

	public virtual Account Account
	{
		get;
		set;
	}

}
