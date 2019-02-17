using System;
using System.Collections.Generic;

public class WorkerInfo
{
	public string WorkerID { get; set; }
	public string ChineseName { get; set; }
	public string EnglishName { get; set; }
	public string Introducer { get; set; }
	public string Address { get; set; }
	public string HKID1 { get; set; }
	public string HKID2 { get; set; }
	public string HKID3 { get; set; }
	public DateTime? DOB { get; set; }
	public string Gender { get; set; }
	public string PhoneNo { get; set; }
	public string PayrollGroup { get; set; }
	public string PayrollRemarks { get; set; }
	public string BankCode { get; set; }
	public string BankACNo { get; set; }
	public string BankName { get; set; }
	public bool MPFOption { get; set; }
	public DateTime? DateJoin { get; set; }
	public string WorkArea { get; set; }
	public string Remarks { get; set; }


    public string BeneficialName { get; set; }
    public string District { get; set; }
    public string PositionGrade { get; set; }
    public string WorkerStatus { get; set; }
    public string Position { get; set; }
    public string AppraisalGrade { get; set; }
    public string PayrollMethod { get; set; }
    public string CardStatus { get; set; }
    public DateTime? ReturnDate { get; set; }


    public string CreateUser { get; set; }
	public DateTime? CreateDate { get; set; }
	public string LastModifyUser { get; set; }
	public DateTime? LastModifyDate { get; set; }

    public List<WorkerSkillInfo> SkillList { get; set; }
    public List<WorkerClientListInfo> ClientList { get; set; }
    public List<WorkerAdjustmentInfo> AdjustmentList { get; set; }
    public List<WorkerAttachmentInfo> AttachmentList { get; set; }

    public class FieldName
	{
		public const string WorkerID = "WorkerID";
		public const string ChineseName = "ChineseName";
		public const string EnglishName = "EnglishName";
		public const string Introducer = "Introducer";
		public const string Address = "Address";
		public const string HKID1 = "HKID1";
		public const string HKID2 = "HKID2";
		public const string HKID3 = "HKID3";
		public const string DOB = "DOB";
		public const string Gender = "Gender";
		public const string PhoneNo = "PhoneNo";
		public const string PayrollGroup = "PayrollGroup";
		public const string PayrollRemarks = "PayrollRemarks";
		public const string BankCode = "BankCode";
		public const string BankACNo = "BankACNo";
		public const string BankName = "BankName";
		public const string MPFOption = "MPFOption";
		public const string DateJoin = "DateJoin";
		public const string WorkArea = "WorkArea";
		public const string Remarks = "Remarks";
		public const string CreateUser = "CreateUser";
		public const string CreateDate = "CreateDate";
		public const string LastModifyUser = "LastModifyUser";
		public const string LastModifyDate = "LastModifyDate";
	}
}
