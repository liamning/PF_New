using System;
public class HolidayInfo
{
	public DateTime HolidayDate { get; set; }
	public string EngDesc { get; set; }
	public string ChiDesc { get; set; }
	public DateTime? LastModifyDate { get; set; }
	public string LastModifyUser { get; set; }
	public DateTime? CreateDate { get; set; }
	public string CreateUser { get; set; }
	public class FieldName
	{
		public const string HolidayDate = "HolidayDate";
		public const string EngDesc = "EngDesc";
		public const string ChiDesc = "ChiDesc";
		public const string LastModifyDate = "LastModifyDate";
		public const string LastModifyUser = "LastModifyUser";
		public const string CreateDate = "CreateDate";
		public const string CreateUser = "CreateUser";
	}
}
