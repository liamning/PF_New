using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class PayrollItem
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    SqlTransaction transaction;

    public PayrollItem(SqlConnection db, SqlTransaction transaction) {
        this.db = db;
        this.transaction = transaction;
    }

    public List<string> GetWorkerIDList(string WorkerID)
    { 
       //db.Open();
        String query = "select top 10 WorkerID from PayrollItem where (@WorkerID = '' or WorkerID like '%' + @WorkerID + '%') order by WorkerID";
        var obj = (List<string>)db.Query<string>(query, new { WorkerID = WorkerID });
        //db.Close();
        return obj;
    }


    public bool IsExisted(PayrollItemInfo info)
    {
       //db.Open();
        String query = "select count(*)  from PayrollItem " 
		+ " where WorkerID = @WorkerID and SalaryDate = @SalaryDate and RowNo = @RowNo ";
        var obj = (List<int>)db.Query<int>(query, info, this.transaction);
        //db.Close();
        return obj[0] > 0;
    }

    public void Save(PayrollItemInfo info)
    {
        if(this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info);
    }


    public PayrollItemInfo Get(string WorkerID)
    {
        db.Open();

        string query = "select * from PayrollItem "
        + " where WorkerID = @WorkerID ";

        var obj = (List<PayrollItemInfo>)db.Query<PayrollItemInfo>(query, new { WorkerID = WorkerID });
        //db.Close();

        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }


    public List<PayrollItemInfo> Get(string WorkerID, DateTime salaryDate)
    {
        db.Open();

        string query = "select * from PayrollItem "
        + " where WorkerID = @WorkerID and SalaryDate = @SalaryDate ";

        var obj = (List<PayrollItemInfo>)db.Query<PayrollItemInfo>(query, new { WorkerID = WorkerID, SalaryDate = salaryDate });
        db.Close();

        return obj;
    }

    public void DeleteNotIn(string WorkerID, DateTime SalaryDate, List<int> RowNoList)
    { 

        string query = "delete  from PayrollItem " 
		+ " where WorkerID = @WorkerID and  SalaryDate = @SalaryDate and RowNo not in @RowNoList ";
		
        db.Execute(query, new { WorkerID = WorkerID, SalaryDate = SalaryDate, RowNoList = RowNoList }, this.transaction);
        //db.Close();
    }
	
    public void Update(PayrollItemInfo info)
    {
       //db.Open();

        string query = " UPDATE [dbo].[PayrollItem] SET  "
		+ "  [ItemCode] = @ItemCode " 
		+ ", [Description] = @Description " 
		+ ", [Amount] = @Amount " 
		+ " where WorkerID = @WorkerID and  SalaryDate = @SalaryDate and RowNo = @RowNo ";

         
        db.Execute(query, info, this.transaction);
        //db.Close();
    }

    public void Insert(PayrollItemInfo info)
    {
       //db.Open();

        string query = "INSERT INTO [dbo].[PayrollItem] ( [WorkerID] " 
		+ ",[RowNo] "
        + ",[SalaryDate] "
        + ",[ItemCode] "
        + ",[Description] " 
		+ ",[Amount] " 
		+") "
		+ "VALUES ( @WorkerID "
		+ ",@RowNo "
        + ",@SalaryDate "
        + ",@ItemCode "
        + ",@Description " 
		+ ",@Amount " 
		+") ";


        db.Execute(query, info, this.transaction);
        //db.Close();
    }
	#endregion 

}