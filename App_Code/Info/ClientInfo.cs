using System;
using System.Collections.Generic;

public class ClientInfo
{
	public string ClientCode { get; set; }
	public string ClientName { get; set; }
	public string Address { get; set; }
	public string Phone { get; set; }
	public string Fax { get; set; }
	public string ContactPerson { get; set; }
	public string ContactPhone { get; set; }
	public string CreateUser { get; set; }
	public DateTime? CreateDate { get; set; }
	public string LastModifyUser { get; set; }
	public DateTime? LastModifyDate { get; set; }

    public List<ClientBUInfo> BUList { get; set; }

    public class FieldName
	{
		public const string ClientCode = "ClientCode";
		public const string ClientName = "ClientName";
		public const string Address = "Address";
		public const string Phone = "Phone";
		public const string Fax = "Fax";
		public const string ContactPerson = "ContactPerson";
		public const string ContactPhone = "ContactPhone";
		public const string CreateUser = "CreateUser";
		public const string CreateDate = "CreateDate";
		public const string LastModifyUser = "LastModifyUser";
		public const string LastModifyDate = "LastModifyDate";
	}
}
