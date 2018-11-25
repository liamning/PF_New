using System;
public class IntroducerInfo
{
	public string IntroducerCode { get; set; }
	public string IntroducerName { get; set; }
	public string IntroducerWorkerID { get; set; }
	public string CreateUser { get; set; }
	public DateTime? CreateDate { get; set; }
	public string LastModifyUser { get; set; }
	public DateTime? LastModifyDate { get; set; }
	public class FieldName
	{
		public const string IntroducerCode = "IntroducerCode";
		public const string IntroducerName = "IntroducerName";
		public const string IntroducerWorkerID = "IntroducerWorkerID";
		public const string CreateUser = "CreateUser";
		public const string CreateDate = "CreateDate";
		public const string LastModifyUser = "LastModifyUser";
		public const string LastModifyDate = "LastModifyDate";
	}
}
