using System;
using System.Collections.Generic;

public class PayrollInfo
{
    public string WorkerID { get; set; }
    public DateTime SalaryDate { get; set; }
    public DateTime Asat { get; set; }
	public DateTime PayFrom { get; set; }
    public DateTime PayTo { get; set; }
    public float Hours { get; set; }
    public float Last30DayTotal { get; set; }
	public float OTHours { get; set; }
	public decimal Amount { get; set; }
	public string Remarks { get; set; }
    public List<PayrollItemInfo> PayrollItemList;
	public class FieldName
	{
		public const string WorkerID = "WorkerID";
		public const string SalaryDate = "SalaryDate";
		public const string PayFrom = "PayFrom";
		public const string PayTo = "PayTo";
		public const string Hours = "Hours";
		public const string OTHours = "OTHours";
		public const string Amount = "Amount";
		public const string Remarks = "Remarks";
	}
}
