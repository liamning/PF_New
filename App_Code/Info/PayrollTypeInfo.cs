using System;
public class PayrollTypeInfo
{
	public string PayrollType { get; set; }
	public string PayrollTypeDesc { get; set; }
	public string CreateUser { get; set; }
	public DateTime? CreateDate { get; set; }
	public string LastModifyUser { get; set; }
	public DateTime? LastModifyDate { get; set; }
	public class FieldName
	{
		public const string PayrollType = "PayrollType";
		public const string PayrollTypeDesc = "PayrollTypeDesc";
		public const string CreateUser = "CreateUser";
		public const string CreateDate = "CreateDate";
		public const string LastModifyUser = "LastModifyUser";
		public const string LastModifyDate = "LastModifyDate";
	}
}
