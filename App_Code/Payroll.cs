using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class Payroll
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    SqlTransaction transaction;
     

    public List<string> GetWorkerIDList(string WorkerID)
    { 
        db.Open();
        String query = "select top 10 WorkerID from Payroll where (@WorkerID = '' or WorkerID like '%' + @WorkerID + '%') order by WorkerID";
        var obj = (List<string>)db.Query<string>(query, new { WorkerID = WorkerID });
        db.Close();
        return obj;
    }

    public PayrollInfo Generate(string PayrollGroup, DateTime asat, DateTime salaryDate,
        int bonusDayCount,
        int totalBonusHours,
        decimal bonusAmount,
        string remarks)
    {
        this.db.Open();

        string query = @"

select attendance.*, ClientBU.BU + ' - ' + ClientBU.Location [Desc] from (
select 
Client,
BU,
 Worker.WorkerID,
min(attendanceDate) PayFrom,
max(attendanceDate) PayTo,
sum(Amount) Amount,
sum(Hours) Hours,
sum(OTHours) OTHours
FROM [dbo].[Attendance]
Join Worker on Attendance.WorkerID = Worker.WorkerID
where PayrollGroup = @PayrollGroup
and AttendanceDate <= @AsAt
and IsPaid is null

group by Client, BU,  Worker.WorkerID
) attendance
join ClientBU on attendance.Client = ClientBU.ClientCode and attendance.BU = ClientBU.RowNo 
 order by workerid, client, bu

";


        string workerID="";
        var attendanceList =  this.db.Query(query, new { PayrollGroup = PayrollGroup, AsAt = asat }); 
        this.db.Close();

        List<string> workerIDList = new List<string>();
        foreach(var info in attendanceList)
        {
            if(!workerIDList.Contains(info.WorkerID))
                workerIDList.Add(info.WorkerID);
        }

        //If over 200 hours per month, ÇÚ¹¤ª„$500
        query = @"

select Attendance.WorkerID,  sum(Hours + OTHours) TotalHours 
from Attendance
Join Worker on Attendance.WorkerID = Worker.WorkerID
where PayrollGroup = @PayrollGroup
and DATEDIFF(DAY, AttendanceDate, @AsAt) <= @BonusDayCount
group by Attendance.WorkerID 


";
        var bonusList = this.db.Query(query, new { PayrollGroup = PayrollGroup, AsAt = asat, BonusDayCount = bonusDayCount }); 


        Dictionary<string, dynamic> bonusIDList = new Dictionary<string, dynamic>();
        foreach (var info in bonusList)
        {
            bonusIDList.Add(info.WorkerID, info);
        }

        List<PayrollInfo> payrollList = new List<PayrollInfo>();
        PayrollInfo payRollInfo = null;
        PayrollItemInfo tmpItem = null;
        int rowNo = 0;
        foreach (var attn in attendanceList)
        {
            if(workerID != attn.WorkerID)
            {
                rowNo = 0;
                payRollInfo = new PayrollInfo();
                payRollInfo.WorkerID = attn.WorkerID;
                payRollInfo.SalaryDate = salaryDate;
                payRollInfo.Remarks = remarks;


                payRollInfo.BonusDayCount = bonusDayCount;
                payRollInfo.TotalBonusHours = totalBonusHours;
                payRollInfo.BonusAmount = bonusAmount;

                //new added fields
                payRollInfo.Asat = asat;

                if (bonusIDList.ContainsKey(payRollInfo.WorkerID))
                    payRollInfo.Last30DayTotal = (float)bonusIDList[payRollInfo.WorkerID].TotalHours;
                else
                    payRollInfo.Last30DayTotal = 0;


                payRollInfo.PayrollItemList = new List<PayrollItemInfo>();

                workerID = attn.WorkerID;



                payrollList.Add(payRollInfo);

            }

            tmpItem = new PayrollItemInfo();
            tmpItem.ItemCode = PayrollItemInfo.Type.Salary;
            tmpItem.Description = attn.Desc;
            tmpItem.Amount = attn.Amount;
            tmpItem.RowNo = ++rowNo;

            payRollInfo.PayFrom = attn.PayFrom;
            payRollInfo.PayTo = attn.PayTo;
            payRollInfo.Hours += attn.Hours;
            payRollInfo.OTHours += attn.OTHours;
            payRollInfo.Amount += attn.Amount;

            payRollInfo.PayrollItemList.Add(tmpItem);
        }


        query = @"
select * from WorkerAdjustment where UpdateDate is null and WorkerID = @WorkerID 
";
       
        List<WorkerAdjustmentInfo> adjustmentList;
        foreach (var info in payrollList)
        {
            if (info.Last30DayTotal > totalBonusHours)
            {
                tmpItem = new PayrollItemInfo();
                tmpItem.ItemCode = PayrollItemInfo.Type.Bonus;
                //tmpItem.Description = string.Format("{0} in {1}/{2}", "Over 200 hours", bonusIDList[info.WorkerID].AttnMonth, bonusIDList[info.WorkerID].AttnYear) ;
                tmpItem.Description = string.Format("Over {0} hours {1} days before {2:dd MMM yyyy}", totalBonusHours, bonusDayCount, asat);
                tmpItem.Amount = bonusAmount;
                tmpItem.RowNo = info.PayrollItemList.Count + 1;

                info.PayrollItemList.Add(tmpItem);
                info.Amount += tmpItem.Amount;

            }

            adjustmentList = (List<WorkerAdjustmentInfo>)this.db.Query<WorkerAdjustmentInfo>(query, new { WorkerID = info.WorkerID });

            foreach (var adjustment in adjustmentList)
            {
                tmpItem = new PayrollItemInfo();
                tmpItem.ItemCode = PayrollItemInfo.Type.Adjustment;
                tmpItem.Description = PayrollItemInfo.Type.Adjustment;
                tmpItem.Amount = adjustment.AdjustAmount;
                tmpItem.RowNo = info.PayrollItemList.Count + 1;

                info.PayrollItemList.Add(tmpItem);
                info.Amount += tmpItem.Amount;

                this.db.Execute(query, new { WorkerID = info.WorkerID });
            }
        }

        this.db.Close();



        if (payrollList.Count == 0) throw new Exception("No payroll item can be generated");

        this.db.Open();
        this.transaction = this.db.BeginTransaction();
        try
        {

            foreach (var info in payrollList)
            {
                this.SaveHelper(info);
            }
                

            this.markAttendance(PayrollGroup, asat);

            //mark bonus: updated: no need to update
            //DateTime tmpMonthEnd;
            //foreach (var key in bonusIDList.Keys)
            //{
            //    tmpMonthEnd = new DateTime(bonusIDList[key].AttnYear, bonusIDList[key].AttnMonth, 1).AddMonths(1).AddDays(-1);
            //    this.UpdateBonus(bonusIDList[key].WorkerID, tmpMonthEnd);
            //}

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


        return payRollInfo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="workerID"></param>
    /// <param name="month">Month End</param>
    public void UpdateBonus(string workerID, DateTime monthEnd)
    {
        string query = @"
select max([Month]) maxMonth from PayrollBonus
where WorkerID = @WorkerID
union all
select min(Attendance.AttendanceDate) from Attendance
where WorkerID = @WorkerID
";
        var maxMonthList = (List<dynamic>)this.db.Query<dynamic>(query, new { WorkerID = workerID }, this.transaction);
        DateTime lastMaxMonth = DateTime.MinValue;
        if(maxMonthList[0].maxMonth != null)
        {
            lastMaxMonth = maxMonthList[0].maxMonth;
        } else
        { 
            lastMaxMonth = new DateTime(maxMonthList[1].maxMonth.Year, maxMonthList[1].maxMonth.Month, 1).AddDays(-1);
        }
        

        if (lastMaxMonth == null) return;

        query = @"
insert into PayrollBonus
select @WorkerID, @Month, @Type
";
        DateTime tmpMonth = lastMaxMonth;
        int tmpType = 0;
        for (int i = 0; i <= 12; i++)
        {
            tmpMonth = tmpMonth.AddDays(1).AddMonths(1).AddDays(-1);
            if (tmpMonth == monthEnd) tmpType = 1;
            else tmpType = 0;

            this.db.Execute(query, new { WorkerID = workerID, Month = tmpMonth, Type = tmpType }, this.transaction);

            if (tmpType == 1) break;
        }

    }

    public void markAttendance(string PayrollGroup, DateTime asat)
    { 
        string query = @"

update [dbo].[Attendance]
set isPaid = 1
where exists (select 1 from Worker where Attendance.WorkerID = Worker.WorkerID and Worker.PayrollGroup = @PayrollGroup)
and AttendanceDate <= @AsAt

";
         
        this.db.Execute(query, new { PayrollGroup = PayrollGroup, AsAt = asat }, this.transaction); 
    }


    public bool IsExisted(PayrollInfo info)
    {
        //db.Open();
        String query = "select count(*)  from Payroll " 
		+ " where WorkerID = @WorkerID and  SalaryDate = @SalaryDate";
        var obj = (List<int>)db.Query<int>(query, info, this.transaction);
        //db.Close();
        return obj[0] > 0;
    }

    public void Save(PayrollInfo info)
    {
        this.db.Open();
        this.transaction = this.db.BeginTransaction();
        try
        {

            this.SaveHelper(info);

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

    public void SaveHelper(PayrollInfo info)
    {
        if (this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info);

        PayrollItem itemObj = new PayrollItem(this.db, this.transaction);
        List<int> rowNoList = new List<int>();
        foreach (var payrollItem in info.PayrollItemList)
        {
            payrollItem.WorkerID = info.WorkerID;
            payrollItem.SalaryDate = info.SalaryDate;
            itemObj.Save(payrollItem);
            rowNoList.Add(payrollItem.RowNo);
        }
        itemObj.DeleteNotIn(info.WorkerID, info.SalaryDate, rowNoList);
    }


    public PayrollInfo Get(string WorkerID)
    {
        db.Open();

        string query = "select * from Payroll "
        + " where WorkerID = @WorkerID ";

        var obj = (List<PayrollInfo>)db.Query<PayrollInfo>(query, new { WorkerID = WorkerID });
        db.Close();

        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }


    public List<PayrollInfo> Get(string WorkerID, DateTime saralyDate)
    {
        db.Open();

        string query = "select * from Payroll "
        + " where WorkerID = @WorkerID and  SalaryDate = @SalaryDate ";

        var obj = (List<PayrollInfo>)db.Query<PayrollInfo>(query, new { WorkerID = WorkerID, SalaryDate = saralyDate });
        db.Close();

        PayrollItem itemHander = new PayrollItem(this.db, this.transaction);
        foreach(var header in obj)
        {
            header.PayrollItemList = itemHander.Get(header.WorkerID, header.SalaryDate);
        }


        return obj;
    }

    public void Delete(string WorkerID)
    {
		db.Open();

        string query = "delete  from Payroll " 
		+ " where WorkerID = @WorkerID ";
		
        db.Execute(query, new {  WorkerID = WorkerID  });
        db.Close();
    }
	
    public void Update(PayrollInfo info)
    {
        //db.Open();

        string query = " UPDATE [dbo].[Payroll] SET  "
		+ "  [PayFrom] = @PayFrom " 
		+ ", [PayTo] = @PayTo "
        + ", [Hours] = @Hours "
        + ", [OTHours] = @OTHours "
        + ", [Last30DayTotal] = @Last30DayTotal "
        + ", [Asat] = @Asat "
        + ", [Amount] = @Amount "
        + ", [Remarks] = @Remarks "
        + ", [BonusDayCount] = @BonusDayCount "
        + ", [TotalBonusHours] = @TotalBonusHours "
        + ", [BonusAmount] = @BonusAmount " 
		+ " where WorkerID = @WorkerID and SalaryDate = @SalaryDate ";

         
        db.Execute(query, info, this.transaction);
        //db.Close();
    }

    public void Insert(PayrollInfo info)
    {
        //db.Open();

        string query = "INSERT INTO [dbo].[Payroll] ( [WorkerID] " 
		+ ",[SalaryDate] " 
		+ ",[PayFrom] "
        + ",[PayTo] "
        + ",[Hours] "
        + ",[Last30DayTotal] "
        + ",[Asat] "
        + ",[OTHours] "
        + ",[Amount] "
        + ",[Remarks] "
        + ",[BonusDayCount] "
        + ",[TotalBonusHours] "
        + ",[BonusAmount] " 
		+") "
		+ "VALUES ( @WorkerID "
		+ ",@SalaryDate " 
		+ ",@PayFrom " 
		+ ",@PayTo "
        + ",@Hours "
        + ",@Last30DayTotal "
        + ",@Asat "
        + ",@OTHours "
        + ",@Amount "
        + ",@Remarks "
        + ",@BonusDayCount "
        + ",@TotalBonusHours "
        + ",@BonusAmount " 
		+") ";


        db.Execute(query, info, this.transaction);
        //db.Close();
    }
	#endregion 

}