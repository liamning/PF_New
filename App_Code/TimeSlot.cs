using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class TimeSlot
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);  
     

    public List<GeneralCodeDesc> GetTimeSlotCodeList(string TimeSlotCode)
    {   
        db.Open();
        string query = @"(select TimeSlotCode Code, TimeSlotDesc [Desc] from [TimeSlot] where @TimeSlotCode = TimeSlotCode)
                        union
                    (select top 10 TimeSlotCode Code, TimeSlotDesc [Desc] from [TimeSlot]
                    where (@TimeSlotCode = '' or TimeSlotCode like '%' + @TimeSlotCode + '%' ))  order by TimeSlotCode";
        var obj = (List<GeneralCodeDesc>)db.Query<GeneralCodeDesc>(query, new { TimeSlotCode = TimeSlotCode });
        db.Close();

        return obj;
    }


    public bool IsExisted(TimeSlotInfo info)
    {
        db.Open();
        String query = "select count(*)  from TimeSlot " 
		+ " where TimeSlotCode = @TimeSlotCode ";
        var obj = (List<int>)db.Query<int>(query, info);
        db.Close();
        return obj[0] > 0;
    }

    public void Save(TimeSlotInfo info)
    {
        if(this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info); 
    }

	 
    public TimeSlotInfo Get(string TimeSlotCode)
    {
		db.Open();

        string query = "select * from TimeSlot " 
		+ " where TimeSlotCode = @TimeSlotCode ";
		
        var obj = (List<TimeSlotInfo>)db.Query<TimeSlotInfo>(query, new {  TimeSlotCode = TimeSlotCode  });
        db.Close();
		
        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public void Delete(string TimeSlotCode)
    {
		db.Open();

        string query = "delete  from TimeSlot " 
		+ " where TimeSlotCode = @TimeSlotCode ";
		
        db.Execute(query, new {  TimeSlotCode = TimeSlotCode  });
        db.Close();
    }
	
    public void Update(TimeSlotInfo info)
    {
        db.Open();

        string query = " UPDATE [dbo].[TimeSlot] SET  "
		+ " [TimeSlotType] = @TimeSlotType " 
		+ ", [TimeSlotDesc] = @TimeSlotDesc " 
		+ ", [HourRate] = @HourRate " 
		+ ", [Remarks] = @Remarks " 
		+ ", [CreateUser] = @CreateUser " 
		+ ", [CreateDate] = @CreateDate " 
		+ ", [LastModifyUser] = @LastModifyUser " 
		+ ", [LastModifyDate] = @LastModifyDate " 
		+ " where TimeSlotCode = @TimeSlotCode ";

         
        db.Execute(query, info);
        db.Close();
    }

    public void Insert(TimeSlotInfo info)
    {
        db.Open();

        string query = "INSERT INTO [dbo].[TimeSlot] ( [TimeSlotCode] " 
		+ ",[TimeSlotType] " 
		+ ",[TimeSlotDesc] " 
		+ ",[HourRate] " 
		+ ",[Remarks] " 
		+ ",[CreateUser] " 
		+ ",[CreateDate] " 
		+ ",[LastModifyUser] " 
		+ ",[LastModifyDate] " 
		+") "
		+ "VALUES ( @TimeSlotCode "
		+ ",@TimeSlotType " 
		+ ",@TimeSlotDesc " 
		+ ",@HourRate " 
		+ ",@Remarks " 
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