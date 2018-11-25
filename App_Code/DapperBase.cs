using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class DapperBase
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    SqlTransaction transaction;
    protected string tableID;

    public List<string> GetStaffNoList(string StaffNo)
    { 
        db.Open();
        String query = "select top 10 StaffNo from UserProfile where (@StaffNo = '' or StaffNo like '%' + @StaffNo + '%') order by StaffNo";
        var obj = (List<string>)db.Query<string>(query, new { StaffNo = StaffNo });
        db.Close();
        return obj;
    }


    public bool IsExisted(UserProfileInfo info)
    {
        db.Open();
        String query = string.Format("select count(*) from {0} where StaffNo = @StaffNo ", tableID);
        var obj = (List<int>)db.Query<int>(query, info);
        db.Close();
        return obj[0] > 0;
    }

    public void Save(UserProfileInfo info)
    {
        if(this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info); 
    }

	 
    public UserProfileInfo Get(string StaffNo)
    {
		db.Open();

        string query = "select * from UserProfile " 
		+ " where StaffNo = @StaffNo ";
		
        var obj = (List<UserProfileInfo>)db.Query<UserProfileInfo>(query, new {  StaffNo = StaffNo  });
        db.Close();
		
        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public void Delete(string StaffNo)
    {
		db.Open();

        string query = "delete  from UserProfile " 
		+ " where StaffNo = @StaffNo ";
		
        db.Execute(query, new {  StaffNo = StaffNo  });
        db.Close();
    }
	
    public void Update(UserProfileInfo info)
    {
        db.Open();

        string query = " UPDATE [dbo].[UserProfile] SET  "
		+ " [StaffName] = @StaffName " 
		+ ", [Password] = @Password " 
		+ ", [Role] = @Role " 
		+ ", [Age] = @Age " 
		+ ", [Gender] = @Gender " 
		+ ", [Mobile] = @Mobile " 
		+ ", [Email] = @Email " 
		+ ", [Location] = @Location " 
		+ ", [CreateUser] = @CreateUser " 
		+ ", [CreateDate] = @CreateDate " 
		+ ", [LastUpdateUser] = @LastUpdateUser " 
		+ ", [LastUpdateDate] = @LastUpdateDate " 
		+ " where StaffNo = @StaffNo ";

         
        db.Execute(query, info);
        db.Close();
    }

    public void Insert(UserProfileInfo info)
    {
        db.Open();

        string query = "INSERT INTO [dbo].[UserProfile] ( [StaffNo] " 
		+ ",[StaffName] " 
		+ ",[Password] " 
		+ ",[Role] " 
		+ ",[Age] " 
		+ ",[Gender] " 
		+ ",[Mobile] " 
		+ ",[Email] " 
		+ ",[Location] " 
		+ ",[CreateUser] " 
		+ ",[CreateDate] " 
		+ ",[LastUpdateUser] " 
		+ ",[LastUpdateDate] " 
		+") "
		+ "VALUES ( @StaffNo "
		+ ",@StaffName " 
		+ ",@Password " 
		+ ",@Role " 
		+ ",@Age " 
		+ ",@Gender " 
		+ ",@Mobile " 
		+ ",@Email " 
		+ ",@Location " 
		+ ",@CreateUser " 
		+ ",@CreateDate " 
		+ ",@LastUpdateUser " 
		+ ",@LastUpdateDate " 
		+") ";


        db.Execute(query, info);
        db.Close();
    }
	#endregion 

}