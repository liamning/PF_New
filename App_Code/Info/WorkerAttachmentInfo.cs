using System;
public class WorkerAttachmentInfo
{
	public string WorkerID { get; set; }
    public int RowNo { get; set; }
    public string AttachmentType { get; set; }
    public string MIME { get; set; }
    public string Path { get; set; }
    public string Content { get; set; }
    public string FileName { get; set; }
    public string Ext { get; set; }
	public string CreateUser { get; set; }
	public DateTime? CreateDate { get; set; }
	public string LastModifyUser { get; set; }
	public DateTime? LastModifyDate { get; set; }
	public class FieldName
	{
		public const string WorkerID = "WorkerID";
		public const string RowNo = "RowNo";
		public const string AttachmentType = "AttachmentType";
		public const string Path = "Path";
		public const string CreateUser = "CreateUser";
		public const string CreateDate = "CreateDate";
		public const string LastModifyUser = "LastModifyUser";
		public const string LastModifyDate = "LastModifyDate";
	}
}
