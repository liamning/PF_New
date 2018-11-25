using System;
public class TimeSlotInfo
{
	public string TimeSlotCode { get; set; }
	public string TimeSlotType { get; set; }
	public string TimeSlotDesc { get; set; }
	public decimal HourRate { get; set; }
	public string Remarks { get; set; }
	public string CreateUser { get; set; }
	public DateTime? CreateDate { get; set; }
	public string LastModifyUser { get; set; }
	public DateTime? LastModifyDate { get; set; }
	public class FieldName
	{
		public const string TimeSlotCode = "TimeSlotCode";
		public const string TimeSlotType = "TimeSlotType";
		public const string TimeSlotDesc = "TimeSlotDesc";
		public const string HourRate = "HourRate";
		public const string Remarks = "Remarks";
		public const string CreateUser = "CreateUser";
		public const string CreateDate = "CreateDate";
		public const string LastModifyUser = "LastModifyUser";
		public const string LastModifyDate = "LastModifyDate";
	}
}
