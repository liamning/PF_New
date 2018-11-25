using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class Introducer
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);  
     

    public List<GeneralCodeDesc> GetIntroducerCodeList(string IntroducerCode)
    { 
        db.Open();
        String query = "select top 10 IntroducerCode Code, IntroducerCode [Desc] from Introducer where (@IntroducerCode = '' or IntroducerCode like '%' + @IntroducerCode + '%') order by IntroducerCode";
        var obj = (List<GeneralCodeDesc>)db.Query<GeneralCodeDesc>(query, new { IntroducerCode = IntroducerCode });
        db.Close();
        return obj;
    }


    public bool IsExisted(IntroducerInfo info)
    {
        db.Open();
        String query = "select count(*)  from Introducer " 
		+ " where IntroducerCode = @IntroducerCode ";
        var obj = (List<int>)db.Query<int>(query, info);
        db.Close();
        return obj[0] > 0;
    }

    public void Save(IntroducerInfo info)
    {
        if(this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info); 
    }

	 
    public IntroducerInfo Get(string IntroducerCode)
    {
		db.Open();

        string query = "select * from Introducer " 
		+ " where IntroducerCode = @IntroducerCode ";
		
        var obj = (List<IntroducerInfo>)db.Query<IntroducerInfo>(query, new {  IntroducerCode = IntroducerCode  });
        db.Close();
		
        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public void Delete(string IntroducerCode)
    {
		db.Open();

        string query = "delete  from Introducer " 
		+ " where IntroducerCode = @IntroducerCode ";
		
        db.Execute(query, new {  IntroducerCode = IntroducerCode  });
        db.Close();
    }
	
    public void Update(IntroducerInfo info)
    {
        db.Open();

        string query = " UPDATE [dbo].[Introducer] SET  "
		+ " [IntroducerName] = @IntroducerName " 
		+ ", [IntroducerWorkerID] = @IntroducerWorkerID " 
		+ ", [CreateUser] = @CreateUser " 
		+ ", [CreateDate] = @CreateDate " 
		+ ", [LastModifyUser] = @LastModifyUser " 
		+ ", [LastModifyDate] = @LastModifyDate " 
		+ " where IntroducerCode = @IntroducerCode ";

         
        db.Execute(query, info);
        db.Close();
    }

    public void Insert(IntroducerInfo info)
    {
        db.Open();

        string query = "INSERT INTO [dbo].[Introducer] ( [IntroducerCode] " 
		+ ",[IntroducerName] " 
		+ ",[IntroducerWorkerID] " 
		+ ",[CreateUser] " 
		+ ",[CreateDate] " 
		+ ",[LastModifyUser] " 
		+ ",[LastModifyDate] " 
		+") "
		+ "VALUES ( @IntroducerCode "
		+ ",@IntroducerName " 
		+ ",@IntroducerWorkerID " 
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