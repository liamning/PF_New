using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;
using System.IO;


public class WorkerAttachment
{
	#region Standar Function

   SqlConnection db;
    SqlTransaction transaction;

    public WorkerAttachment()
    {
        db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    }

    public WorkerAttachment(SqlConnection db, SqlTransaction transaction)
    {
        this.db = db;
        this.transaction = transaction;
    }

    public List<string> GetWorkerIDList(string WorkerID)
    { 
        //db.Open();
        String query = "select top 10 WorkerID from WorkerAttachment where (@WorkerID = '' or WorkerID like '%' + @WorkerID + '%') order by WorkerID";
        var obj = (List<string>)db.Query<string>(query, new { WorkerID = WorkerID });
        //db.Close();
        return obj;
    }


    public bool IsExisted(WorkerAttachmentInfo info)
    {
        //db.Open();
        String query = "select count(*)  from WorkerAttachment " 
		+ " where WorkerID = @WorkerID and RowNo=@RowNo ";
        var obj = (List<int>)db.Query<int>(query, info, this.transaction);
        //db.Close();
        return obj[0] > 0;
    }

    public void Save(WorkerAttachmentInfo info)
    {
        if(this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info);

        if (!string.IsNullOrEmpty(info.Content))
            GlobalSetting.SaveFile(string.Format("{0}_{1}", info.WorkerID, info.RowNo), info.Content);
    }


    public WorkerAttachmentInfo Get(string WorkerID, int rowno)
    {
        //db.Open();

        string query = "select * from WorkerAttachment "
        + " where WorkerID = @WorkerID and RowNo = @RowNo ";

        var obj = (List<WorkerAttachmentInfo>)db.Query<WorkerAttachmentInfo>(query, new { WorkerID = WorkerID, RowNo = rowno });
        //db.Close();

        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public List<WorkerAttachmentInfo> GetList(string WorkerID)
    {
        db.Open();

        string query = "select * from WorkerAttachment "
        + " where WorkerID = @WorkerID ";

        var obj = (List<WorkerAttachmentInfo>)db.Query<WorkerAttachmentInfo>(query, new { WorkerID = WorkerID });
        db.Close();

        return obj;
    }

    public void Delete(string WorkerID, List<int> RowNoList)
    {
		//db.Open();

        string query = "select RowNo from WorkerAttachment "
        + " where WorkerID = @WorkerID and RowNo not in @RowNoList;delete  from WorkerAttachment "
        + " where WorkerID = @WorkerID and RowNo not in @RowNoList";

        var result = db.Query(query, new { WorkerID = WorkerID, RowNoList = RowNoList }, this.transaction);

        foreach (var obj in result)
        {
            GlobalSetting.DeleteFile(string.Format("{0}_{1}", WorkerID, obj.RowNo));
        }
        //db.Close();
    }
	
    public void Update(WorkerAttachmentInfo info)
    {
        //db.Open();

        info.CreateDate = DateTime.Now;

        string query = " UPDATE [dbo].[WorkerAttachment] SET  " 
        + "  [MIME] = @MIME "
        + ", [AttachmentType] = @AttachmentType "
        + ", [Path] = @Path "
        + ", [FileName] = @FileName "
        + ", [Ext] = @Ext " 
		+ ", [CreateUser] = @CreateUser " 
		+ ", [CreateDate] = @CreateDate " 
		+ ", [LastModifyUser] = @LastModifyUser " 
		+ ", [LastModifyDate] = @LastModifyDate " 
		+ " where WorkerID = @WorkerID and RowNo = @RowNo ";


        db.Execute(query, info, this.transaction);
        //db.Close();
    }

    public void Insert(WorkerAttachmentInfo info)
    {
        //db.Open();
        info.CreateDate = DateTime.Now;

        string query = "INSERT INTO [dbo].[WorkerAttachment] ( [WorkerID] "
        + ",[RowNo] "
        + ",[AttachmentType] "
        + ",[MIME] "
        + ",[Path] "
        + ",[FileName] "
        + ",[Ext] " 
		+ ",[CreateUser] " 
		+ ",[CreateDate] " 
		+ ",[LastModifyUser] " 
		+ ",[LastModifyDate] " 
		+") "
		+ "VALUES ( @WorkerID "
        + ",@RowNo "
        + ",@AttachmentType "
        + ",@MIME "
        + ",@Path "
        + ",@FileName "
        + ",@Ext " 
		+ ",@CreateUser " 
		+ ",@CreateDate " 
		+ ",@LastModifyUser " 
		+ ",@LastModifyDate " 
		+") ";


        db.Execute(query, info, this.transaction);
        //db.Close();
    }
	#endregion 

}