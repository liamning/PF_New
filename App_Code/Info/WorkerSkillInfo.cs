using System;
public class WorkerSkillInfo
{
	public string WorkerID { get; set; }
	public int RowNo { get; set; }
	public string Description { get; set; }
	public class FieldName
	{
		public const string WorkerID = "WorkerID";
		public const string RowNo = "RowNo";
		public const string Description = "Description";
	}
}
