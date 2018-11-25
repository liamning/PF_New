using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class PayrollGroup
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);  
     

    public List<GeneralCodeDesc> GetPayrollGroupIDList(string PayrollGroupID)
    { 
        db.Open();
        String query = "select top 10 PayrollGroupID Code, PayrollGroupDesc [Desc] from PayrollGroup where (@PayrollGroupID = '' or PayrollGroupID like '%' + @PayrollGroupID + '%') order by PayrollGroupID";
        var obj = (List<GeneralCodeDesc>)db.Query<GeneralCodeDesc>(query, new { PayrollGroupID = PayrollGroupID });
        db.Close();
        return obj;
    }


    public bool IsExisted(PayrollGroupInfo info)
    {
        db.Open();
        String query = "select count(*)  from PayrollGroup " 
		+ " where PayrollGroupID = @PayrollGroupID ";
        var obj = (List<int>)db.Query<int>(query, info);
        db.Close();
        return obj[0] > 0;
    }

    public void Save(PayrollGroupInfo info)
    {
        if(this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info); 
    }

	 
    public PayrollGroupInfo Get(string PayrollGroupID)
    {
		db.Open();

        string query = "select * from PayrollGroup " 
		+ " where PayrollGroupID = @PayrollGroupID ";
		
        var obj = (List<PayrollGroupInfo>)db.Query<PayrollGroupInfo>(query, new {  PayrollGroupID = PayrollGroupID  });
        db.Close();
		
        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public void Delete(string PayrollGroupID)
    {
		db.Open();

        string query = "delete  from PayrollGroup " 
		+ " where PayrollGroupID = @PayrollGroupID ";
		
        db.Execute(query, new {  PayrollGroupID = PayrollGroupID  });
        db.Close();
    }
	
    public void Update(PayrollGroupInfo info)
    {
        db.Open();

        string query = " UPDATE [dbo].[PayrollGroup] SET  "
		+ " [PayrollGroupDesc] = @PayrollGroupDesc " 
		+ ", [CreateUser] = @CreateUser " 
		+ ", [CreateDate] = @CreateDate " 
		+ ", [LastModifyUser] = @LastModifyUser " 
		+ ", [LastModifyDate] = @LastModifyDate " 
		+ " where PayrollGroupID = @PayrollGroupID ";

         
        db.Execute(query, info);
        db.Close();
    }

    public void Insert(PayrollGroupInfo info)
    {
        db.Open();

        string query = "INSERT INTO [dbo].[PayrollGroup] ( [PayrollGroupID] " 
		+ ",[PayrollGroupDesc] " 
		+ ",[CreateUser] " 
		+ ",[CreateDate] " 
		+ ",[LastModifyUser] " 
		+ ",[LastModifyDate] " 
		+") "
		+ "VALUES ( @PayrollGroupID "
		+ ",@PayrollGroupDesc " 
		+ ",@CreateUser " 
		+ ",@CreateDate " 
		+ ",@LastModifyUser " 
		+ ",@LastModifyDate " 
		+") ";


        db.Execute(query, info);
        db.Close();
    }
	#endregion 

}