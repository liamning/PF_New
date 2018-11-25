using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Dapper;


public class Attendance
{
    #region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    SqlTransaction transaction;


    public List<string> GetAttendanceIDList(string AttendanceID)
    {
        db.Open();
        String query = "select top 10 AttendanceID from Attendance where (@AttendanceID = '' or AttendanceID like '%' + @AttendanceID + '%') order by AttendanceID";
        var obj = (List<string>)db.Query<string>(query, new { AttendanceID = AttendanceID });
        db.Close();
        return obj;
    }


    public bool IsExisted(AttendanceInfo info)
    {
        String query = @"

select count(*) 
from Attendance 
where WorkerID = @WorkerID 
and Client = @Client 
and BU = @BU
and AttendanceDate = @AttendanceDate 
and TimeSlot = @TimeSlot
";

        var obj = (List<int>)db.Query<int>(query, info, this.transaction);

        return obj[0] > 0;
    }

    public void Save(AttendanceInfo info)
    {
        if (this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info);
    }


    public AttendanceInfo Get(int AttendanceID)
    {
        db.Open();

        string query = "select * from Attendance "
        + " where AttendanceID = @AttendanceID ";

        var obj = (List<AttendanceInfo>)db.Query<AttendanceInfo>(query, new { AttendanceID = AttendanceID });
        db.Close();

        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public void Delete(int AttendanceID)
    {
        db.Open();

        string query = "delete  from Attendance "
        + " where AttendanceID = @AttendanceID ";

        db.Execute(query, new { AttendanceID = AttendanceID });
        db.Close();
    }

    public string Update(AttendanceInfo info)
    {
        string error = "";
        string query = @" 

select count(*) Total from [dbo].[Attendance] 
where WorkerID = @WorkerID 
and Client = @Client 
and BU = @BU
and AttendanceDate = @AttendanceDate 
and TimeSlot = @TimeSlot
and IsPaid = 1";


        var isPaidList = db.Query(query, info, this.transaction);
        foreach(var item in isPaidList)
        {
            if(item.Total > 0)
            {
                error = string.Format("Can't update the paid attendance of {0} on {1:dd/MM/yyyy}", info.WorkerID, info.AttendanceDate);
            }
            break;
        }
        if (string.IsNullOrEmpty(error))
        { 
            query = " UPDATE [dbo].[Attendance] SET  "
            + " [WorkerID] = @WorkerID "
            + ", [Client] = @Client "
            + ", [BU] = @BU "
            + ", [ClientStaffNo] = @ClientStaffNo "
            + ", [ClientStaffName] = @ClientStaffName "
            + ", [AttendanceDate] = @AttendanceDate "
            + ", [TimeIn] = @TimeIn "
            + ", [TimeOut] = @TimeOut "
            + ", [Hours] = @Hours "
            + ", [OTHours] = @OTHours "
            + ", [AdjustedHours] = @AdjustedHours "
            + ", [AdjustedOTHours] = @AdjustedOTHours "
            + ", [HourRate] = @HourRate "
            + ", [OTHourRate] = @OTHourRate "
            + ", [Amount] = @Amount "
            + ", [Remarks] = @Remarks "
              + @" where WorkerID = @WorkerID 
and Client = @Client 
and BU = @BU
and AttendanceDate = @AttendanceDate 
and TimeSlot = @TimeSlot";


            db.Execute(query, info, this.transaction);
        }

        return error;
    }

    public void Insert(AttendanceInfo info)
    {

        string query = "INSERT INTO [dbo].[Attendance] ( "
          + "[WorkerID] "
          + ",[Client] "
          + ",[TimeSlot] "
          + ",[BU] "
          + ",[ClientStaffNo] "
          + ",[ClientStaffName] "
          + ",[AttendanceDate] "
          + ",[TimeIn] "
          + ",[TimeOut] "
          + ",[Hours] "
          + ",[OTHours] "
          + ",[AdjustedHours] "
          + ",[AdjustedOTHours] "
          + ",[HourRate] "
          + ",[OTHourRate] "
          + ",[Amount] "
          + ",[Remarks] "
          + ") "
          + "VALUES ( "
          + "@WorkerID "
          + ",@Client "
          + ",@TimeSlot "
          + ",@BU "
          + ",@ClientStaffNo "
          + ",@ClientStaffName "
          + ",@AttendanceDate "
          + ",@TimeIn "
          + ",@TimeOut "
          + ",@Hours "
          + ",@OTHours "
          + ",@AdjustedHours "
          + ",@AdjustedOTHours "
          + ",@HourRate "
          + ",@OTHourRate "
          + ",@Amount "
          + ",@Remarks "
          + ") ";


        db.Execute(query, info, this.transaction);
         
    }
    #endregion

    public List<AttendanceInfo> GetAttendanceList(string clientCode, int BU, DateTime startDate, DateTime endDate)
    {

        var obj = (List<AttendanceInfo>)this.db.Query<AttendanceInfo>(@"

select *
from Attendance
where 
Client = @ClientCode 
and BU = @BU 
and AttendanceDate between @StartDate and @EndDate

", new { ClientCode = clientCode, BU = BU, StartDate = startDate, EndDate = endDate });

        return obj;

    }
    public List<AttendanceInfo> GetAttendanceList(string clientCode, int BU, string workerID, DateTime targetDate)
    {

        var obj = (List<AttendanceInfo>)this.db.Query<AttendanceInfo>(@"

select *
from Attendance
where 
Client = @ClientCode 
and BU = @BU 
and AttendanceDate = @TargetDate
and WorkerID = @WorkerID 

", new { ClientCode = clientCode, BU = BU, WorkerID = workerID, TargetDate = targetDate }, this.transaction);

        return obj;

    }

    public Dictionary<string, Dictionary<DateTime, int>> GetWorkerWorkingHoursByWeek(string clientCode, int BU, DateTime week)
    {
        Dictionary<string, Dictionary<DateTime, int>> result = new Dictionary<string, Dictionary<DateTime, int>>();
        int dayofWeek = (int)week.DayOfWeek;

        DateTime startDate = week.AddDays(-dayofWeek);
        DateTime endDate = week.AddDays(-dayofWeek + 6);

        var obj = this.db.Query<dynamic>(@"
select Client, BU, WorkerID, AttendanceDate, Hours+OTHours TotalHours, TimeSlot
from Attendance
where 
Client = @ClientCode 
and BU = @BU 
and AttendanceDate between @StartDate and @EndDate",

                new { ClientCode = clientCode, BU = BU, StartDate = startDate, EndDate = endDate });

        foreach (var info in obj)
        {
            if (!result.ContainsKey(info.WorkerID + info.TimeSlot)) result.Add(info.WorkerID + info.TimeSlot, new Dictionary<DateTime, int>());
            if (!result[info.WorkerID + info.TimeSlot].ContainsKey(info.AttendanceDate)) result[info.WorkerID + info.TimeSlot].Add(info.AttendanceDate, (int)info.TotalHours);
        }

        return result;
    }


    public Dictionary<string, Dictionary<DateTime, int>> GetWorkerWorkingHoursBy2Week(string clientCode, int BU, DateTime week)
    {
        Dictionary<string, Dictionary<DateTime, int>> result = new Dictionary<string, Dictionary<DateTime, int>>();
        int dayofWeek = (int)week.DayOfWeek;

        DateTime startDate = week.AddDays(-16);
        DateTime endDate = week;

        var obj = this.db.Query<dynamic>(@"
select Client, BU, WorkerID, AttendanceDate, Hours+OTHours TotalHours, TimeSlot
from Attendance
where 
Client = @ClientCode 
and BU = @BU 
and AttendanceDate between @StartDate and @EndDate",

                new { ClientCode = clientCode, BU = BU, StartDate = startDate, EndDate = endDate });

        foreach (var info in obj)
        {
            if (!result.ContainsKey(info.WorkerID + info.TimeSlot)) result.Add(info.WorkerID + info.TimeSlot, new Dictionary<DateTime, int>());
            if (!result[info.WorkerID + info.TimeSlot].ContainsKey(info.AttendanceDate)) result[info.WorkerID + info.TimeSlot].Add(info.AttendanceDate, (int)info.TotalHours);
        }

        return result;
    }

    public string[] GetWorkderID(string clientCode, int BU, string staffNo)
    {
        var obj = this.db.Query<dynamic>(@"
select Worker.WorkerID, Worker.Gender , Worker.PositionGrade 
from WorkerClientList 
join Worker on WorkerClientList.WorkerID = Worker.WorkerID
where ClientCode = @ClientCode and  BU = @BU and StaffNo = @StaffNo",
                new { ClientCode = clientCode, BU = BU, StaffNo = staffNo }, transaction);

        foreach (var info in obj)
        {
            return new string[] { info.WorkerID, info.Gender, info.PositionGrade };
        }


        return null;

    }

    public List<AttendanceInfo> BatchInsert(List<AttendanceInfo> list)
    { 
        db.Open();
        transaction = db.BeginTransaction();

        try
        {
            DateTime startDate = DateTime.MaxValue, endDate = DateTime.MinValue;
            string error;
            foreach (AttendanceInfo info in list)
            {
                if (this.IsExisted(info))
                {
                    error = this.Update(info);
                    if (!string.IsNullOrEmpty(error))
                    { 
                        info.Remarks = error; 
                        Log.Error(error);
                    }
                    else
                    {
                        info.Remarks = "Imported"; 
                    }
                }
                else
                {

                    this.Insert(info);
                    info.Remarks = "Imported"; 
                }

                //startDate = startDate > info.AttendanceDate.Value ? info.AttendanceDate.Value : startDate;
                //endDate = endDate < info.AttendanceDate.Value ? info.AttendanceDate.Value : endDate;
            }

            this.BatchUpdateRate(list);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            db.Close();
        }

        return list;

    }

    public List<AttendanceInfo> GetNightShiftAttendanceList(string clientCode, int BU, string workerID, DateTime startDate, DateTime endDate)
    {
        //DateTime startDate = sunday;
        //DateTime endDate = sunday.AddDays(6);
        var obj = (List<AttendanceInfo>)this.db.Query<AttendanceInfo>(@"

select *
from Attendance
where 
Client = @ClientCode 
and BU = @BU 
and WorkerID = @WorkerID
and Timeslot = '3'
and AttendanceDate between @StartDate and @EndDate
order by AttendanceDate

", new { ClientCode = clientCode, BU = BU, StartDate = startDate, WorkerID = workerID, EndDate = endDate }, this.transaction);

        return obj;

    }

    //public double GetWeelyHours(List<AttendanceInfo> list)
    //{
    //    double weeklyHours = 0;
    //    int dayofWeek = (int)targetDate.DayOfWeek;
    //    DateTime startDate = targetDate.AddDays(-dayofWeek);

    //    for (int i = 0; i <= 6; i++)
    //    {
    //        if (dailyHoursDict.ContainsKey(startDate.AddDays(i)))
    //            weeklyHours += dailyHoursDict[startDate.AddDays(i)];
    //    }

    //    return weeklyHours;
    //}

    public List<AttendanceInfo> BatchUpdateRate(List<AttendanceInfo> list)
    {
        //List<AttendanceInfo> nightShiftAttendance = new List<AttendanceInfo>();
        List<AttendanceInfo> newAttendanceList = new List<AttendanceInfo>();
        List<AttendanceInfo> tmpList, satList;
        List<string> attendanceKey = new List<string>();
        string tmpKey;
        int dayofWeek;
        DateTime startDate;
        DateTime endDate;
        AttendanceInfo sundayAttendance; 
        double weekHours;
        List<string> intervalList = new HourlyRateMapping().GetIntervalList(list[0].Client, list[0].BU);
        foreach (AttendanceInfo info in list)
        {
            if (info.TimeSlot == "3")
            {
                if (intervalList.Contains("W"))
                {
                    dayofWeek = (int)info.AttendanceDate.Value.DayOfWeek;
                    startDate = info.AttendanceDate.Value.AddDays(-dayofWeek); //sunday - saturday
                    tmpKey = string.Format("{0}{1}{2}{3:yyyyMMdd}", info.Client, info.BU, info.WorkerID, startDate);
                    if (!attendanceKey.Contains(tmpKey))
                    {
                        weekHours = 0; 
                        sundayAttendance = null;
                        tmpList = this.GetNightShiftAttendanceList(info.Client, info.BU, info.WorkerID, startDate, startDate.AddDays(6));
                        attendanceKey.Add(tmpKey);
                        foreach (var dbInfo in tmpList)
                        {
                            weekHours += (info.Hours + info.OTHours);
                        }
                        foreach (var dbInfo in tmpList)
                        {
                            dbInfo.WeeklyHours = weekHours;
                            dbInfo.WorkerID = info.WorkerID;
                            dbInfo.PositionGrade = info.PositionGrade;
                            dbInfo.Gender = info.Gender;
                            newAttendanceList.Add(dbInfo);

                            if ((int)dbInfo.AttendanceDate.Value.DayOfWeek == 0)
                            {
                                sundayAttendance = dbInfo;
                            }
                            else if ((int)dbInfo.AttendanceDate.Value.DayOfWeek == 6)
                            {
                                sundayAttendance.SatHours = info.Hours + info.OTHours;
                            }
                        } 
                    }
                } else if (intervalList.Contains("M"))
                { 
                    //sunday = info.AttendanceDate.Value.AddDays(-dayofWeek);
                    if(info.AttendanceDate.Value.Day >= 26)
                    {
                        startDate = new DateTime(info.AttendanceDate.Value.Year, info.AttendanceDate.Value.Month, 26);
                        endDate = new DateTime(info.AttendanceDate.Value.Year, info.AttendanceDate.Value.Month + 1, 10);
                    } else if (info.AttendanceDate.Value.Day <= 10)
                    { 
                        startDate = new DateTime(info.AttendanceDate.Value.Year, info.AttendanceDate.Value.Month - 1, 26);
                        endDate = new DateTime(info.AttendanceDate.Value.Year, info.AttendanceDate.Value.Month , 10);
                    } else
                    { 
                        startDate = new DateTime(info.AttendanceDate.Value.Year, info.AttendanceDate.Value.Month, 11);
                        endDate = new DateTime(info.AttendanceDate.Value.Year, info.AttendanceDate.Value.Month, 25);
                    }

                    tmpKey = string.Format("{0}{1}{2}{3:yyyyMMdd}", info.Client, info.BU, info.WorkerID, startDate);
                    if (!attendanceKey.Contains(tmpKey))
                    {
                        weekHours = 0; 
                        tmpList = this.GetNightShiftAttendanceList(info.Client, info.BU, info.WorkerID, startDate, endDate);
                        attendanceKey.Add(tmpKey);
                        foreach (var dbInfo in tmpList)
                        {
                            weekHours += (info.Hours + info.OTHours);
                        }
                        foreach (var dbInfo in tmpList)
                        {
                            dbInfo.WeeklyHours = weekHours;
                            dbInfo.WorkerID = info.WorkerID;
                            dbInfo.PositionGrade = info.PositionGrade;
                            dbInfo.Gender = info.Gender;
                            newAttendanceList.Add(dbInfo);

                            if (dbInfo.AttendanceDate.Value.DayOfWeek == 0)
                            {
                                //get the sat day in the same week 
                                satList = this.GetNightShiftAttendanceList(dbInfo.Client, dbInfo.BU, dbInfo.WorkerID, dbInfo.AttendanceDate.Value.AddDays(6), dbInfo.AttendanceDate.Value.AddDays(6));
                                foreach (var satInfo in satList)
                                {
                                    dbInfo.SatHours = satInfo.Hours + satInfo.OTHours;
                                } 
                            }
                        } 
                    }
                }
                else
                {
                    newAttendanceList.Add(info);
                }

            }
            else if (info.AttendanceDate.Value.DayOfWeek == 0)
            {
                //get the sat day in the same week 
                satList = this.GetAttendanceList(info.Client, info.BU, info.WorkerID, info.AttendanceDate.Value.AddDays(6));
                foreach (var satInfo in satList)
                {
                    info.SatHours = satInfo.Hours + satInfo.OTHours;
                }
                newAttendanceList.Add(info);
            }
            else
            {
                newAttendanceList.Add(info);
            }
        }

        foreach (AttendanceInfo info in newAttendanceList)
        {
            List<HourlyRateMappingInfo> hourlyRateList = this.DetermineHoulyRate(info);
            HourlyRateMappingInfo hourlyRateInfo = null;
            foreach (HourlyRateMappingInfo tmpInfo in hourlyRateList)
            {
                //sat , sun hourly >= 8
                if (tmpInfo.Type == "S")
                {
                    if (((int)info.AttendanceDate.Value.DayOfWeek) == 0
                    && info.SatHours >= 8
                    && info.Hours >= 8)
                    {
                        hourlyRateInfo = tmpInfo; break;
                    }
                    else
                    {
                        continue;
                    }
                } //normal case
                else
                {
                    hourlyRateInfo = tmpInfo; break;
                }

            }
            if (hourlyRateInfo != null)
            {
                info.HourRate = hourlyRateInfo.Rate;
                info.OTHourRate = hourlyRateInfo.OTRate ?? 0;
                if (hourlyRateInfo.OT > 0 && info.Hours > hourlyRateInfo.OT)
                {
                    
                    //info.OTHours = (double)((decimal)info.Hours - (decimal)hourlyRateInfo.OT);
                    info.OTHours = (double)((decimal)info.Hours - (decimal)hourlyRateInfo.OT) + info.OTHours;
                    info.Hours = hourlyRateInfo.OT ?? 0;//info.Hours - info.OTHours;
                    
                    //info.Amount = info.HourRate * (decimal)hourlyRateInfo.OT + info.OTHourRate * (decimal)info.OTHours;
                }
                //else
                //{
                //    info.Amount = info.HourRate * (decimal)info.Hours;
                //}
                info.Amount = info.HourRate * (decimal)info.Hours + info.OTHourRate * (decimal)info.OTHours;
            }

            this.Update(info);
        }

        foreach (AttendanceInfo info in list)
        {
            if (info.HourRate == 0) info.Remarks += "; Failure to calculate the hour rate;";
        }

        return newAttendanceList;
    }

    public List<HourlyRateMappingInfo> DetermineHoulyRate(AttendanceInfo info)
    {

        string query = @"
select * from HourlyRateMapping 
where ClientCode = @ClientCode 
and BU = @BU 
and PositionGrade = @PositionGrade 
and Gender = @Gender 
and TimeSlot = @TimeSlot
and CHARINDEX(@DayOfWeek,[DayOfWeek]) > 0
and (Interval = 'D' or ((HoursFrom <= @WeekHours or HoursFrom is null) and (HoursTo >= @WeekHours or HoursTo is null)))
and (Type <> 'H' or exists (select 1 from Holiday where HolidayDate = @AttendanceDate) )
order by Rate desc
";


        HourlyRateMappingInfo mappingCriteria = new HourlyRateMappingInfo();
        mappingCriteria.ClientCode = info.Client;
        mappingCriteria.BU = info.BU;
        mappingCriteria.PositionGrade = info.PositionGrade;
        mappingCriteria.Gender = info.Gender;
        mappingCriteria.AttendanceDate = info.AttendanceDate;
        mappingCriteria.TimeSlot = info.TimeSlot;
        mappingCriteria.WeekHours = info.WeeklyHours;
        mappingCriteria.DayOfWeek = ((int)info.AttendanceDate.Value.DayOfWeek).ToString();

        List<HourlyRateMappingInfo> mapping = (List<HourlyRateMappingInfo>)db.Query<HourlyRateMappingInfo>(query, mappingCriteria, this.transaction);

        return mapping;
    }



}
