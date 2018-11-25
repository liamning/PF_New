using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class TimeslotMapping
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    SqlTransaction transaction;
     

    public List<string> GetTimeSlotCodeList(string TimeSlotCode)
    { 
        db.Open();
        String query = "select top 10 TimeSlotCode from TimeslotMapping where (@TimeSlotCode = '' or TimeSlotCode like '%' + @TimeSlotCode + '%') order by TimeSlotCode";
        var obj = (List<string>)db.Query<string>(query, new { TimeSlotCode = TimeSlotCode });
        db.Close();
        return obj;
    }


    public bool IsExisted(TimeslotMappingInfo info)
    {
        //db.Open();
        String query = "select count(*)  from TimeslotMapping " 
		+ " where RowNo = @RowNo ";
        var obj = (List<int>)db.Query<int>(query, info, transaction);
        //db.Close();
        return obj[0] > 0;
    }

    public void Save(List<TimeslotMappingInfo> list)
    {
        db.Open();
        this.transaction = this.db.BeginTransaction();
        try
        {
            List<int> rowNoList = new List<int>();
            foreach(var info in list)
            {
                rowNoList.Add(info.RowNo);
                if (this.IsExisted(info))
                    this.Update(info);
                else
                    this.Insert(info);

            }
            this.DeleteNotIn(rowNoList);


            this.transaction.Commit();
        }
        catch
        {
            this.transaction.Rollback();
            throw;

        }
        finally
        {
            this.db.Close();
        }
    }


    public List<TimeslotMappingInfo> GetALL()
    {
        db.Open();

        string query = @"
select TimeslotMapping.*, ChiDesc TimeSlotDesc from TimeslotMapping 
join GeneralMaster on TimeSlotCode = Code and Category = 'TimeSlot'
";

        var obj = (List<TimeslotMappingInfo>)db.Query<TimeslotMappingInfo>(query);
        db.Close();

        return obj;
    }


    public Dictionary<string, string> GetDescToTimeslot()
    {
        var obj = this.GetALL();
        Dictionary<string, string> descToTimeslot = new Dictionary<string, string>();
        foreach(var info in obj)
        {
            if (!descToTimeslot.ContainsKey(info.Description))
            {
                descToTimeslot.Add(info.Description, info.TimeSlotCode);
            }
        }

        return descToTimeslot;
    }

    public void DeleteNotIn(List<int> rowNoList)
    { 
        string query = "delete  from TimeslotMapping " 
		+ " where RowNo not in @RowNoList";
		
        db.Execute(query, new { RowNoList = rowNoList }, this.transaction); 
    }
	
    public void Update(TimeslotMappingInfo info)
    { 

        string query = " UPDATE [dbo].[TimeslotMapping] SET  "
        + " [TimeSlotCode] = @TimeSlotCode " 
		+ ", [Description] = @Description " 
		+ " where RowNo = @RowNo ";

         
        db.Execute(query, info, transaction); 
    }

    public void Insert(TimeslotMappingInfo info)
    { 
        string query = "INSERT INTO [dbo].[TimeslotMapping] ( [TimeSlotCode] " 
		+ ",[RowNo] " 
		+ ",[Description] " 
		+") "
		+ "VALUES ( @TimeSlotCode "
		+ ",@RowNo " 
		+ ",@Description " 
		+") ";


        db.Execute(query, info, this.transaction); 
    }
	#endregion 

}