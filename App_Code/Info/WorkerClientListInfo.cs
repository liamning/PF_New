using System;
public class WorkerClientListInfo
{
	public string WorkerID { get; set; }
    public string ClientCode { get; set; }
    public string BU { get; set; }
    public string StaffNo { get; set; }
    public DateTime? EffectiveFrom{ get; set; }
    public DateTime? EffectiveTo { get; set; }
	public class FieldName
	{
		public const string WorkerID = "WorkerID";
		public const string ClientCode = "ClientCode";
		public const string StaffNo = "StaffNo";
	}
}
