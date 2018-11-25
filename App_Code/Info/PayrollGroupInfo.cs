using System;
public class PayrollGroupInfo
{
	public string PayrollGroupID { get; set; }
	public string PayrollGroupDesc { get; set; }
	public string CreateUser { get; set; }
	public DateTime? CreateDate { get; set; }
	public string LastModifyUser { get; set; }
	public DateTime? LastModifyDate { get; set; }
	public class FieldName
	{
		public const string PayrollGroupID = "PayrollGroupID";
		public const string PayrollGroupDesc = "PayrollGroupDesc";
		public const string CreateUser = "CreateUser";
		public const string CreateDate = "CreateDate";
		public const string LastModifyUser = "LastModifyUser";
		public const string LastModifyDate = "LastModifyDate";
	}
}
