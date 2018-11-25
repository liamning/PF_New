using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Dapper;

/// <summary>
/// Summary description for UserProfile
/// </summary>
public class UserProfile
{
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    //SqlConnection db = new SqlConnection(GlobalSetting.SqlServerConnString);
    SqlTransaction transaction;
    public UserProfile()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public List<GeneralCodeDesc> GetUserProfileList(string UserID)
    {
        db.Open();
        String query = "select top 10 UserID Code, UserName [Desc] from UserProfile where (@UserID = '' or UserID like '%' + @UserID + '%') order by UserID";
        var obj = (List<GeneralCodeDesc>)db.Query<GeneralCodeDesc>(query, new { UserID = UserID });
        db.Close();
        return obj;
    }

    public bool Login(string userID, string password)
    {
        db.Open();
        string query = "select count(*) from [dbo].[UserProfile] where UserID = @UserID and Password = @Password ";
        var obj = (List<int>)db.Query<int>(query, new { UserID = userID, Password = password });
        db.Close();
        return obj[0] > 0;
    }


    public bool IsExisted(UserProfileInfo info)
    {
        db.Open();
        String query = "select count(*)  from UserProfile "
        + " where UserID = @UserID ";
        var obj = (List<int>)db.Query<int>(query, info);
        db.Close();
        return obj[0] > 0;
    }
    public void Save(UserProfileInfo info)
    {
        if (this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info);
    }


    public UserProfileInfo Get(string UserID)
    {
        db.Open();

        string query = "select * from UserProfile "
        + " where UserID = @UserID ";

        var obj = (List<UserProfileInfo>)db.Query<UserProfileInfo>(query, new { UserID = UserID });
        db.Close();

        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public void Delete(string UserID)
    {
        db.Open();

        string query = "delete  from UserProfile "
        + " where UserID = @UserID ";

        db.Execute(query, new { UserID = UserID });
        db.Close();
    }

    public bool UpdatePassword(string userID, string currentPassword, string newPassword)
    {
        if (!Login(userID, currentPassword))
        {
            return false;
        }

        string query = " UPDATE [dbo].[UserProfile] SET  "
        + " [Password] = @Password " 
        + " where UserID = @UserID ";


        db.Open();

        db.Execute(query, new
        {
            UserID = userID,
            Password = newPassword
        });

        db.Close();

        return true;
    }
    public void Update(UserProfileInfo info)
    {
        db.Open();

        string query = " UPDATE [dbo].[UserProfile] SET  "
        + " [UserName] = @UserName "
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
        + " where UserID = @UserID ";


        db.Execute(query, info);
        db.Close();
    }

    public void Insert(UserProfileInfo info)
    {
        db.Open();

        string query = "INSERT INTO [dbo].[UserProfile] ( [UserID] "
        + ",[UserName] "
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
        + ") "
        + "VALUES ( @UserID "
        + ",@UserName "
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
        + ") ";


        db.Execute(query, info);
        db.Close();
    }

}