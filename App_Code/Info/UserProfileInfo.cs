using System;
public class UserProfileInfo
{
	public string UserID { get; set; }
	public string UserName { get; set; }
	public string Password { get; set; }
	public string Role { get; set; }
	public int Age { get; set; }
	public string Gender { get; set; }
	public string Mobile { get; set; }
	public string Email { get; set; }
	public string Location { get; set; }
	public string CreateUser { get; set; }
	public DateTime? CreateDate { get; set; }
	public string LastUpdateUser { get; set; }
	public DateTime? LastUpdateDate { get; set; }
	public class FieldName
	{
		public const string UserID = "UserID";
		public const string UserName = "UserName";
		public const string Password = "Password";
		public const string Role = "Role";
		public const string Age = "Age";
		public const string Gender = "Gender";
		public const string Mobile = "Mobile";
		public const string Email = "Email";
		public const string Location = "Location";
		public const string CreateUser = "CreateUser";
		public const string CreateDate = "CreateDate";
		public const string LastUpdateUser = "LastUpdateUser";
		public const string LastUpdateDate = "LastUpdateDate";
	}
}
