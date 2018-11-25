using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class WorkerClientList
{
    #region Standar Function
    SqlConnection db;// = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);  
    SqlTransaction transaction;

    public WorkerClientList()
    {
        this.db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    }
    public WorkerClientList(SqlConnection db, SqlTransaction transaction)
    {
        this.db = db;
        this.transaction = transaction;
    }

    public List<string> GetWorkerIDList(string WorkerID)
    { 
        //db.Open();
        String query = "select top 10 WorkerID from WorkerClientList where (@WorkerID = '' or WorkerID like '%' + @WorkerID + '%') order by WorkerID";
        var obj = (List<string>)db.Query<string>(query, new { WorkerID = WorkerID });
        //db.Close();
        return obj;
    }


    public bool IsExisted(WorkerClientListInfo info)
    {
        //db.Open();
        String query = "select count(*)  from WorkerClientList " 
		+ " where WorkerID = @WorkerID ";
        var obj = (List<int>)db.Query<int>(query, info, this.transaction);
        //db.Close();
        return obj[0] > 0;
    }

    public void Save(List<WorkerClientListInfo> list, string workerID)
    {
        this.Delete(workerID);
        this.Insert(list);
    }

	 
    public List<WorkerClientListInfo> Get(string WorkerID)
    {
		db.Open();

        string query = "select * from WorkerClientList " 
		+ " where WorkerID = @WorkerID ";
		
        var obj = (List<WorkerClientListInfo>)db.Query<WorkerClientListInfo>(query, new {  WorkerID = WorkerID  });
        db.Close();

        return obj;
    }

    public void Delete(string WorkerID)
    {
		//db.Open();

        string query = "delete  from WorkerClientList " 
		+ " where WorkerID = @WorkerID ";
		
        db.Execute(query, new {  WorkerID = WorkerID  }, this.transaction);
        //db.Close();
    }
	
    public void Update(WorkerClientListInfo info)
    {
        //db.Open();

        string query = " UPDATE [dbo].[WorkerClientList] SET  "
		+ " [ClientCode] = @ClientCode " 
		+ ", [StaffNo] = @StaffNo " 
		+ " where WorkerID = @WorkerID ";

         
        db.Execute(query, info);
        //db.Close();
    }

    public void Insert(List<WorkerClientListInfo> list)
    {
        //db.Open();

        string query = "INSERT INTO [dbo].[WorkerClientList] ( [WorkerID] " 
		+ ",[ClientCode] "
        + ",[BU] "
        + ",[StaffNo] "
        + ") "
		+ "VALUES ( @WorkerID "
        + ",@ClientCode "
        + ",@BU "
        + ",@StaffNo " 
		+") ";

        if (list!=null)
        foreach(var info in list)
            db.Execute(query, info, this.transaction);

        //db.Close();
    }
	#endregion 

}