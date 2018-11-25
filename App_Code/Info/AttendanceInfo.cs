using System;
public class AttendanceInfo
{
	public int AttendanceID { get; set; }
	public string WorkerID { get; set; }
	public string Client { get; set; }
	public int BU { get; set; }
    public string Gender { get; set; }
    public string PositionGrade { get; set; }
    public string ClientStaffNo { get; set; }
    public string ClientStaffName { get; set; }
    public DateTime? AttendanceDate { get; set; }
	public DateTime? TimeIn { get; set; }
	public DateTime? TimeOut { get; set; }
    public double Hours { get; set; }
    public double OTHours { get; set; }
    public double SatHours { get; set; }
    public double WeeklyHours { get; set; }
    public double AdjustedHours { get; set; }
	public double AdjustedOTHours { get; set; }
	public decimal HourRate { get; set; }
	public decimal OTHourRate { get; set; }
	public decimal Amount { get; set; }
    public string Remarks { get; set; }
    public string TimeSlot { get; set; } 
    public class FieldName
	{
		public const string AttendanceID = "AttendanceID";
		public const string WorkerID = "WorkerID";
		public const string Client = "Client";
		public const string Store = "Store";
		public const string ClientStaffNo = "ClientStaffNo";
		public const string ClientStaffName = "ClientStaffName";
		public const string AttendanceDate = "AttendanceDate";
		public const string TimeIn = "TimeIn";
		public const string TimeOut = "TimeOut";
		public const string TotalHours = "TotalHours";
		public const string AdjustedHours = "AdjustedHours";
		public const string AdjustedOTHours = "AdjustedOTHours";
		public const string HourRate = "HourRate";
		public const string OTHourRate = "OTHourRate";
		public const string Amount = "Amount";
		public const string Remarks = "Remarks";
	}
}
