using System;
public class SampleInfo
{
	public string SampleNo { get; set; }
	public string SampleText { get; set; }
	public string SampleTextarea { get; set; }
	public string SampleRadioButton { get; set; }
	public string Email { get; set; }
	public string Relationship { get; set; }
	public decimal Asset { get; set; }
	public decimal Liability { get; set; }
	public DateTime? SampleDate { get; set; }
	public string CreateUser { get; set; }
	public DateTime? UpdateDate { get; set; }
	public string UpdateUser { get; set; }
	public DateTime? SampleTime { get; set; }
	public class FieldName
	{
		public const string SampleNo = "SampleNo";
		public const string SampleText = "SampleText";
		public const string SampleTextarea = "SampleTextarea";
		public const string SampleRadioButton = "SampleRadioButton";
		public const string Email = "Email";
		public const string Relationship = "Relationship";
		public const string Asset = "Asset";
		public const string Liability = "Liability";
		public const string SampleDate = "SampleDate";
		public const string CreateUser = "CreateUser";
		public const string UpdateDate = "UpdateDate";
		public const string UpdateUser = "UpdateUser";
		public const string SampleTime = "SampleTime";
	}
}
