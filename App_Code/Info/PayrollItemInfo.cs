using System;
public class PayrollItemInfo
{
    public string WorkerID { get; set; }
    public DateTime SalaryDate { get; set; }
    public int RowNo { get; set; }
	public string ItemCode { get; set; }
	public string Description { get; set; }
	public decimal Amount { get; set; }
	public class FieldName
	{
		public const string WorkerID = "WorkerID";
		public const string RowNo = "RowNo";
		public const string ItemCode = "ItemCode";
		public const string Description = "Description";
		public const string Amount = "Amount";
	}

    public class Type
    {
        public const string Adjustment = "Adjustment";
        public const string Bonus = "Bonus";
        public const string Salary = "Salary";
    }
}
