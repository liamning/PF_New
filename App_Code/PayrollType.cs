using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class PayrollType
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);  
     

    public List<string> GetPayrollTypeList(string PayrollType)
    { 
        db.Open();
        String query = "select top 10 PayrollType from PayrollType where (@PayrollType = '' or PayrollType like '%' + @PayrollType + '%') order by PayrollType";
        var obj = (List<string>)db.Query<string>(query, new { PayrollType = PayrollType });
        db.Close();
        return obj;
    }


    public bool IsExisted(PayrollTypeInfo info)
    {
        db.Open();
        String query = "select count(*)  from PayrollType " 
		+ " where PayrollType = @PayrollType ";
        var obj = (List<int>)db.Query<int>(query, info);
        db.Close();
        return obj[0] > 0;
    }

    public void Save(PayrollTypeInfo info)
    {
        if(this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info); 
    }

	 
    public PayrollTypeInfo Get(string PayrollType)
    {
		db.Open();

        string query = "select * from PayrollType " 
		+ " where PayrollType = @PayrollType ";
		
        var obj = (List<PayrollTypeInfo>)db.Query<PayrollTypeInfo>(query, new {  PayrollType = PayrollType  });
        db.Close();
		
        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public void Delete(string PayrollType)
    {
		db.Open();

        string query = "delete  from PayrollType " 
		+ " where PayrollType = @PayrollType ";
		
        db.Execute(query, new {  PayrollType = PayrollType  });
        db.Close();
    }
	
    public void Update(PayrollTypeInfo info)
    {
        db.Open();

        string query = " UPDATE [dbo].[PayrollType] SET  "
		+ " [PayrollTypeDesc] = @PayrollTypeDesc " 
		+ ", [CreateUser] = @CreateUser " 
		+ ", [CreateDate] = @CreateDate " 
		+ ", [LastModifyUser] = @LastModifyUser " 
		+ ", [LastModifyDate] = @LastModifyDate " 
		+ " where PayrollType = @PayrollType ";

         
        db.Execute(query, info);
        db.Close();
    }

    public void Insert(PayrollTypeInfo info)
    {
        db.Open();

        string query = "INSERT INTO [dbo].[PayrollType] ( [PayrollType] " 
		+ ",[PayrollTypeDesc] " 
		+ ",[CreateUser] " 
		+ ",[CreateDate] " 
		+ ",[LastModifyUser] " 
		+ ",[LastModifyDate] " 
		+") "
		+ "VALUES ( @PayrollType "
		+ ",@PayrollTypeDesc " 
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