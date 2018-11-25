using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class HourlyRateMapping
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);  
     

    //public List<string> GetStoreCodeList(string StoreCode)
    //{ 
    //    db.Open();
    //    String query = "select top 10 StoreCode from HourlyRateMapping where (@StoreCode = '' or StoreCode like '%' + @StoreCode + '%') order by StoreCode";
    //    var obj = (List<string>)db.Query<string>(query, new { StoreCode = StoreCode });
    //    db.Close();
    //    return obj;
    //}


  //  public bool IsExisted(HourlyRateMappingInfo info)
  //  {
  //      db.Open();
  //      String query = "select count(*)  from HourlyRateMapping " 
		//+ " where StoreCode = @StoreCode ";
  //      var obj = (List<int>)db.Query<int>(query, info);
  //      db.Close();
  //      return obj[0] > 0;
  //  }

    public void Save(List<HourlyRateMappingInfo> list)
    {
        Clear();
        foreach (var info in list)
        {
            this.Insert(info);
        }
    }

    public void Save(List<HourlyRateMappingInfo> list, string clientCode, string buCode, string positionGrade)
    {
        Clear(clientCode, buCode, positionGrade);
        foreach (var info in list)
        {
            info.ClientCode = clientCode;
            info.BU = Convert.ToInt32(buCode);
            info.PositionGrade = positionGrade;
            this.Insert(info);
        }
    }

    public List<HourlyRateMappingInfo> Get()
    {
        db.Open();

        string query = @"select 

GM_Gender.EngDesc GenderDesc,
GM_TimeSlot.EngDesc TimeSlotDesc,
GM_Interval.EngDesc IntervalDesc,
GM_Type.EngDesc TypeDesc,
HourlyRateMapping.*

from HourlyRateMapping
join GeneralMaster GM_Gender on GM_Gender.Category = 'Gender' and GM_Gender.Code = Gender
join GeneralMaster GM_TimeSlot on  GM_TimeSlot.Category = 'TimeSlot' and GM_TimeSlot.Code = TimeSlot
join GeneralMaster GM_Interval on  GM_Interval.Category = 'Interval' and GM_Interval.Code = Interval 
join GeneralMaster GM_Type on  GM_Type.Category = 'Type' and GM_Type.Code = Type
";

        var obj = (List<HourlyRateMappingInfo>)db.Query<HourlyRateMappingInfo>(query);
        db.Close();

        return obj;
    }

    public List<HourlyRateMappingInfo> Get(string clientCode, string buCode, string positionGrade)
    {
        db.Open();

        string query = @"
select 

HourlyRateMapping.*,
GM_Gender.EngDesc GenderDesc,
GM_TimeSlot.EngDesc TimeSlotDesc,
GM_Interval.EngDesc IntervalDesc,
GM_Type.EngDesc TypeDesc

from HourlyRateMapping
join GeneralMaster GM_Gender on GM_Gender.Category = 'Gender' and GM_Gender.Code = Gender
join GeneralMaster GM_TimeSlot on  GM_TimeSlot.Category = 'TimeSlot' and GM_TimeSlot.Code = TimeSlot
join GeneralMaster GM_Interval on  GM_Interval.Category = 'Interval' and GM_Interval.Code = Interval 
join GeneralMaster GM_Type on  GM_Type.Category = 'Type' and GM_Type.Code = Type

where ClientCode = @ClientCode and BU = @BU and PositionGrade = @PositionGrade
order by TimeSlot, DayOfWeek, Rate

";

        var obj = (List<HourlyRateMappingInfo>)db.Query<HourlyRateMappingInfo>(query, new { ClientCode = clientCode, BU = Convert.ToInt32(buCode), PositionGrade = positionGrade });
        db.Close();

        return obj;
    }


    public List<string> GetIntervalList(string clientCode, int buCode)
    {
        db.Open();

        string query = @"
 
select distinct Interval from HourlyRateMapping 
where ClientCode = @ClientCode and BU = @BU 
";

        var obj = (List<string>)db.Query<string>(query, new { ClientCode = clientCode, BU = buCode });
        db.Close();

        return obj;
    }

    //    public List<HourlyRateMappingInfo> Get(HourlyRateMappingInfo mappingCriteria)
    //    {
    //		db.Open();

    //        string query = @"   
    //select 

    //*
    //from HourlyRateMapping

    //where StoreCode = @StoreCode and Gender = @Gender and TimeSlot = @TimeSlot
    //and CHARINDEX(@DayOfWeek,[DayOfWeek]) > 0
    //and (Interval = 'D' or (Interval = 'W' and HoursFrom < @WeekHours and HoursTo > @WeekHours))
    //order by Rate desc
    //";

    //        var obj = (List<HourlyRateMappingInfo>)db.Query<HourlyRateMappingInfo>(query, mappingCriteria);
    //        db.Close();

    //        return obj;
    //    }

    public void Clear(string clientCode, string buCode, string positionGrade)
    {
        db.Open();

        string query = "delete from HourlyRateMapping where ClientCode = @ClientCode and BU = @BU and PositionGrade = @PositionGrade ";

        db.Execute(query, new { ClientCode = clientCode, BU = Convert.ToInt32(buCode), PositionGrade = positionGrade });
        db.Close();
    }


    public void Clear()
    {
        db.Open();

        string query = "delete from HourlyRateMapping ";

        db.Execute(query);
        db.Close();
    }

    //public void Delete(string StoreCode)
    //{
    //    db.Open();

    //    string query = "delete  from HourlyRateMapping "
    //    + " where StoreCode = @StoreCode ";

    //    db.Execute(query, new { StoreCode = StoreCode });
    //    db.Close();
    //}

  //  public void Update(HourlyRateMappingInfo info)
  //  {
  //      db.Open();

  //      string query = " UPDATE [dbo].[HourlyRateMapping] SET  "
		//+ " [Gender] = @Gender " 
		//+ ", [Session] = @Session " 
		//+ ", [DayOfWeek] = @DayOfWeek " 
		//+ ", [IsHoliday] = @IsHoliday " 
		//+ ", [HoursFrom] = @HoursFrom " 
		//+ ", [HoursTo] = @HoursTo " 
		//+ ", [Rate] = @Rate " 
		//+ ", [CreateUser] = @CreateUser " 
		//+ ", [CreateDate] = @CreateDate " 
		//+ ", [LastModifyUser] = @LastModifyUser " 
		//+ ", [LastModifyDate] = @LastModifyDate " 
		//+ " where StoreCode = @StoreCode ";

         
  //      db.Execute(query, info);
  //      db.Close();
  //  }

    public void Insert(HourlyRateMappingInfo info)
    {
        db.Open();

        string query = "INSERT INTO [dbo].[HourlyRateMapping] ( "
        + " [ClientCode] "
        + ",[BU] "
        + ",[PositionGrade] "
        + ",[Gender] "
        + ",[TimeSlot] "
        + ",[Interval] "
        + ",[DayOfWeek] "
        + ",[Type] "
        + ",[HoursFrom] "
        + ",[HoursTo] "
        + ",[Rate] "
        + ",[OT] "
        + ",[OTRate] "
        + ",[CreateUser] "
        + ",[CreateDate] "
        + ",[LastModifyUser] "
        + ",[LastModifyDate] "
        + ") "
        + "VALUES ( "
        + " @ClientCode "
        + ",@BU "
        + ",@PositionGrade "
        + ",@Gender "
        + ",@TimeSlot "
        + ",@Interval "
        + ",@DayOfWeek "
        + ",@Type "
        + ",@HoursFrom "
        + ",@HoursTo "
        + ",@Rate "
        + ",@OT "
        + ",@OTRate "
        + ",@CreateUser "
        + ",@CreateDate "
        + ",@LastModifyUser "
        + ",@LastModifyDate "
        + ") ";


        db.Execute(query, info);
        db.Close();
    }
     
    #endregion

}