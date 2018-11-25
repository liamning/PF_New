using System;
public class ClientBUInfo
{
	public string ClientCode { get; set; }
	public int RowNo { get; set; }
	public string BU { get; set; }
	public string Location { get; set; }
	public class FieldName
	{
		public const string ClientCode = "ClientCode";
		public const string RowNo = "RowNo";
		public const string BU = "BU";
		public const string Location = "Location";
	}
}
