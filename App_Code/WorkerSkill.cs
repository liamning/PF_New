using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Dapper;


public class WorkerSkill
{
    #region Standar Function
    SqlConnection db;
    SqlTransaction transaction;

    public WorkerSkill()
    {
        db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    }

    public WorkerSkill(SqlConnection db, SqlTransaction transaction)
    {
        this.db = db;
        this.transaction = transaction;
    }

    public List<string> GetWorkerIDList(string WorkerID)
    {
        //db.Open();
        String query = "select top 10 WorkerID from WorkerSkill where (@WorkerID = '' or WorkerID like '%' + @WorkerID + '%') order by WorkerID";
        var obj = (List<string>)db.Query<string>(query, new { WorkerID = WorkerID });
        //db.Close();
        return obj;
    }


    public bool IsExisted(WorkerSkillInfo info)
    {
        //db.Open();
        String query = "select count(*)  from WorkerSkill "
        + " where WorkerID = @WorkerID and RowNo = @RowNo";
        var obj = (List<int>)db.Query<int>(query, info, this.transaction);
        //db.Close();
        return obj[0] > 0;
    }

    public void Save(WorkerSkillInfo info)
    {
        if (this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info);
    }


    public List<WorkerSkillInfo> GetSkillList(string WorkerID)
    {
        db.Open();

        string query = "select * from WorkerSkill "
        + " where WorkerID = @WorkerID ";

        var obj = (List<WorkerSkillInfo>)db.Query<WorkerSkillInfo>(query, new { WorkerID = WorkerID });
        db.Close();

        return obj;
    }

    public void Delete(string WorkerID)
    {
        //db.Open();

        string query = "delete  from WorkerSkill "
        + " where WorkerID = @WorkerID ";

        db.Execute(query, new { WorkerID = WorkerID }, this.transaction);
        //db.Close();
    }

    public void Update(WorkerSkillInfo info)
    {
        //db.Open();

        string query = " UPDATE [dbo].[WorkerSkill] SET   [Description] = @Description "
        + " where WorkerID = @WorkerID and RowNo = @RowNo  ";


        db.Execute(query, info, this.transaction);
        //db.Close();
    }

    public void Insert(WorkerSkillInfo info)
    {
        //db.Open();

        string query = "INSERT INTO [dbo].[WorkerSkill] ( [WorkerID] "
        + ",[RowNo] "
        + ",[Description] "
        + ") "
        + "VALUES ( @WorkerID "
        + ",@RowNo "
        + ",@Description "
        + ") ";


        db.Execute(query, info, this.transaction);
        //db.Close();
    }
    #endregion

}