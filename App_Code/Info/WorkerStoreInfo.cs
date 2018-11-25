using System;
public class WorkerStoreInfo
{
	public string WorkerID { get; set; }
    public string ClientCode { get; set; }
    public string BU { get; set; }
    public string ClientWorkerID { get; set; }
	public string StoreCode { get; set; }
	public DateTime? StartDate { get; set; }
	public DateTime? ToDate { get; set; }
	public class FieldName
	{
		public const string WorkerID = "WorkerID";
		public const string ClientCode = "ClientCode";
		public const string ClientWorkerID = "ClientWorkerID";
		public const string StoreCode = "StoreCode";
		public const string StartDate = "StartDate";
		public const string ToDate = "ToDate";
	}
}
