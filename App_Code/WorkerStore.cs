using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class WorkerStore
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);  
     

    public List<string> GetWorkerIDList(string WorkerID)
    { 
        db.Open();
        String query = "select top 10 WorkerID from WorkerStore where (@WorkerID = '' or WorkerID like '%' + @WorkerID + '%') order by WorkerID";
        var obj = (List<string>)db.Query<string>(query, new { WorkerID = WorkerID });
        db.Close();
        return obj;
    }


    public bool IsExisted(WorkerStoreInfo info)
    {
        db.Open();
        String query = "select count(*)  from WorkerStore " 
		+ " where WorkerID = @WorkerID ";
        var obj = (List<int>)db.Query<int>(query, info);
        db.Close();
        return obj[0] > 0;
    }

    public void Save(WorkerStoreInfo info)
    {
        if(this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info); 
    }

	 
    public WorkerStoreInfo Get(string WorkerID)
    {
		db.Open();

        string query = "select * from WorkerStore " 
		+ " where WorkerID = @WorkerID ";
		
        var obj = (List<WorkerStoreInfo>)db.Query<WorkerStoreInfo>(query, new {  WorkerID = WorkerID  });
        db.Close();
		
        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public void Delete(string WorkerID)
    {
		db.Open();

        string query = "delete  from WorkerStore " 
		+ " where WorkerID = @WorkerID ";
		
        db.Execute(query, new {  WorkerID = WorkerID  });
        db.Close();
    }
	
    public void Update(WorkerStoreInfo info)
    {
        db.Open();

        string query = " UPDATE [dbo].[WorkerStore] SET  "
		+ " [ClientCode] = @ClientCode " 
		+ ", [ClientWorkerID] = @ClientWorkerID " 
		+ ", [StoreCode] = @StoreCode " 
		+ ", [StartDate] = @StartDate " 
		+ ", [ToDate] = @ToDate " 
		+ " where WorkerID = @WorkerID ";

         
        db.Execute(query, info);
        db.Close();
    }

    public void Insert(WorkerStoreInfo info)
    {
        db.Open();

        string query = "INSERT INTO [dbo].[WorkerStore] ( [WorkerID] " 
		+ ",[ClientCode] " 
		+ ",[ClientWorkerID] " 
		+ ",[StoreCode] " 
		+ ",[StartDate] " 
		+ ",[ToDate] " 
		+") "
		+ "VALUES ( @WorkerID "
		+ ",@ClientCode " 
		+ ",@ClientWorkerID " 
		+ ",@StoreCode " 
		+ ",@StartDate " 
		+ ",@ToDate " 
		+") ";


        db.Execute(query, info);
        db.Close();
    }
	#endregion 

}