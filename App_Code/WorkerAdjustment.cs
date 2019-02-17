using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class WorkerAdjustment
{
    #region Standar Function

    SqlConnection db;
    SqlTransaction transaction;

    public WorkerAdjustment()
    {
        db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    }

    public WorkerAdjustment(SqlConnection db, SqlTransaction transaction)
    {
        this.db = db;
        this.transaction = transaction;
    }


    public List<string> GetWorkerIDList(string WorkerID)
    { 
        db.Open();
        String query = "select top 10 WorkerID from WorkerAdjustment where (@WorkerID = '' or WorkerID like '%' + @WorkerID + '%') order by WorkerID";
        var obj = (List<string>)db.Query<string>(query, new { WorkerID = WorkerID });
        db.Close();
        return obj;
    }


    public bool IsExisted(WorkerAdjustmentInfo info)
    {
       // db.Open();
        String query = "select count(*)  from WorkerAdjustment "
        + " where WorkerID = @WorkerID and RowNo = @RowNo";
        var obj = (List<int>)db.Query<int>(query, info, this.transaction);
        //db.Close();
        return obj[0] > 0;
    }

    public void Save(WorkerAdjustmentInfo info)
    {
        if(this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info); 
    }


    public List<WorkerAdjustmentInfo> GetList(string WorkerID)
    {
		db.Open();

        string query = "select * from WorkerAdjustment " 
		+ " where WorkerID = @WorkerID ";
		
        var obj = (List<WorkerAdjustmentInfo>)db.Query<WorkerAdjustmentInfo>(query, new {  WorkerID = WorkerID  });
        db.Close();

        return obj;
    }

    public void Delete(string WorkerID, int RowNo)
    {
		//db.Open();

        string query = "delete  from WorkerAdjustment "
        + " where WorkerID = @WorkerID and RowNo >= @RowNo";

        db.Execute(query, new { WorkerID = WorkerID, RowNo = RowNo }, this.transaction);
        //db.Close();
    }
	
    public void Update(WorkerAdjustmentInfo info)
    {
        //db.Open();

        string query = " UPDATE [dbo].[WorkerAdjustment] SET  "
		+ " [RowNo] = @RowNo " 
		+ ", [AdjustAmount] = @AdjustAmount " 
		+ ", [UpdateDate] = @UpdateDate "
        + " where WorkerID = @WorkerID and RowNo = @RowNo  ";


        db.Execute(query, info, this.transaction);
       // db.Close();
    }

    public void Insert(WorkerAdjustmentInfo info)
    {
        

        string query = "INSERT INTO [dbo].[WorkerAdjustment] ( [WorkerID] " 
		+ ",[RowNo] " 
		+ ",[AdjustAmount] " 
		+ ",[UpdateDate] " 
		+") "
		+ "VALUES ( @WorkerID "
		+ ",@RowNo " 
		+ ",@AdjustAmount " 
		+ ",@UpdateDate " 
		+") ";


        db.Execute(query, info, this.transaction);
    }
	#endregion 

}