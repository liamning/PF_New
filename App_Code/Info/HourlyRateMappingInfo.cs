using System;
public class HourlyRateMappingInfo
{
	public int ID { get; set; }
    public string ClientCode { get; set; }
    public int BU { get; set; }
    public string PositionGrade { get; set; }
    public string Gender { get; set; }
    public string TimeSlot { get; set; }
    public string Interval { get; set; }
    public string GenderDesc { get; set; }
    public string TimeSlotDesc { get; set; }
    public string IntervalDesc { get; set; }
    public string DayOfWeek { get; set; }
    public double WeekHours { get; set; }
    public string Type { get; set; }
    public string TypeDesc { get; set; }
    public DateTime? AttendanceDate { get; set; }
    public int? HoursFrom { get; set; }
	public int? HoursTo { get; set; }
	public decimal Rate { get; set; }
	public int? OT { get; set; }
	public decimal? OTRate { get; set; }
	public string CreateUser { get; set; }
	public DateTime? CreateDate { get; set; }
	public string LastModifyUser { get; set; }
	public DateTime? LastModifyDate { get; set; }
	public class FieldName
	{
		public const string ID = "ID";
		public const string StoreCode = "StoreCode";
		public const string Gender = "Gender";
		public const string TimeSlot = "TimeSlot";
		public const string Interval = "Interval";
		public const string DayOfWeek = "DayOfWeek";
		public const string Type = "Type";
		public const string HoursFrom = "HoursFrom";
		public const string HoursTo = "HoursTo";
		public const string Rate = "Rate";
		public const string OT = "OT";
		public const string OTRate = "OTRate";
		public const string CreateUser = "CreateUser";
		public const string CreateDate = "CreateDate";
		public const string LastModifyUser = "LastModifyUser";
		public const string LastModifyDate = "LastModifyDate";
	}
}
