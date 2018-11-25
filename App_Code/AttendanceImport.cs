using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for AttendanceImport
/// </summary>
public class AttendanceImport
{
    public AttendanceImport()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public List<AttendanceInfo> Import(Stream file, string clientCode, int BU)
    {
        if (clientCode == "Zara")
            return Import_Zara(file, clientCode, BU);
        else
            return Import_Nike(file, clientCode, BU);

    }


    public List<AttendanceInfo> Import_Zara(Stream file, string clientCode, int BU)
    {

        //get mapping table of desc to timslot
        Dictionary<string, string> descToTimeslot = new TimeslotMapping().GetDescToTimeslot();

        List<AttendanceInfo> attendanceList = new List<AttendanceInfo>();
        List<AttendanceInfo> invalidAttendanceList = new List<AttendanceInfo>();
        Attendance attendanceObj = new Attendance();
        AttendanceInfo tempAttendance;

        string[] workerInfo;

        XSSFWorkbook hssfwb = new XSSFWorkbook(file);
        // ISheet sheet = hssfwb.GetSheet("每日出勤表");
        ISheet sheet = hssfwb.GetSheetAt(0);
        IRow currentRow;
        //ICell currentCell;

        string dateFormat = "yyyy-MM-dd";
        string datetimeFormat = "yyyy-MM-dd HH:mm";

        DateTime firstAttendanceDate = DateTime.MinValue;
 
        //get the week day
        currentRow = sheet.GetRow(3); //the day of month stored in row 4
        Dictionary<DateTime, int> dateColMapping = new Dictionary<DateTime, int>();
        DateTime workingDate = DateTime.MaxValue;
        for (int col = 2; col < currentRow.LastCellNum - 1; col=col+2)
        {
            System.Diagnostics.Debug.WriteLine(col);
            if (currentRow.GetCell(col) == null) continue;

            if (currentRow.GetCell(col).CellType == CellType.Numeric)
            {
                dateColMapping.Add(currentRow.GetCell(col).DateCellValue, col);
                workingDate = currentRow.GetCell(col).DateCellValue;
            }
            else if (!string.IsNullOrEmpty(currentRow.GetCell(col).StringCellValue))
            {
                if (currentRow.GetCell(col).StringCellValue == "Grand Total") break;
                workingDate = DateTime.ParseExact(currentRow.GetCell(col).StringCellValue, dateFormat, CultureInfo.InvariantCulture);
                dateColMapping.Add(workingDate, col);
            }
            else if (string.IsNullOrEmpty(currentRow.GetCell(col).StringCellValue))
            {
                break;
            }

        }


        //get the working hours from excel file: data row start at row 8
        for (int row = 7; row < sheet.LastRowNum; row++)
        {
            System.Diagnostics.Debug.WriteLine(row);
            currentRow = sheet.GetRow(row);
            if (currentRow != null) //null is when the row only contains empty cells 
            {
                if (currentRow.GetCell(0) == null || string.IsNullOrEmpty(currentRow.GetCell(0).StringCellValue)) break;

                foreach (DateTime attendanceDate in dateColMapping.Keys)
                {
                    System.Diagnostics.Debug.WriteLine(attendanceDate.ToString("ddMMyyyy"));
                    if (currentRow.GetCell(dateColMapping[attendanceDate]) == null) continue;

                    tempAttendance = new AttendanceInfo();
                    tempAttendance.Client = clientCode;
                    tempAttendance.BU = BU;

                    tempAttendance.AttendanceDate = attendanceDate;
                    tempAttendance.ClientStaffNo = currentRow.GetCell(0).StringCellValue;

                    if (tempAttendance.ClientStaffNo == "Grand Total") break;
                    if (!descToTimeslot.ContainsKey(currentRow.GetCell(1).StringCellValue))
                    {
                        tempAttendance.Remarks = string.Format("Unknown timeslot: {0}", currentRow.GetCell(1).StringCellValue);
                        invalidAttendanceList.Add(tempAttendance);
                        continue;
                    }
                    tempAttendance.TimeSlot = descToTimeslot[currentRow.GetCell(1).StringCellValue];
                    tempAttendance.Hours = currentRow.GetCell(dateColMapping[attendanceDate]).NumericCellValue;

                    //after 22:00
                    if (currentRow.GetCell(dateColMapping[attendanceDate] + 1) != null)
                        tempAttendance.OTHours = currentRow.GetCell(dateColMapping[attendanceDate] + 1).NumericCellValue;
                    else
                        tempAttendance.OTHours = 0;

                    if (tempAttendance.Hours == 0) continue;


                    //get WorkerID /Gender/ PositionGrade
                    workerInfo = attendanceObj.GetWorkderID(clientCode, BU, tempAttendance.ClientStaffNo);
                    if (workerInfo == null)
                    {
                        tempAttendance.Remarks = "Worker Not Found";
                        invalidAttendanceList.Add(tempAttendance);
                        continue;
                    }
                    else
                    {
                        tempAttendance.WorkerID = workerInfo[0];
                        tempAttendance.Gender = workerInfo[1];
                        tempAttendance.PositionGrade = workerInfo[2];

                    }

                    attendanceList.Add(tempAttendance);
                }
            }
        }
         

        if (invalidAttendanceList.Count == 0)
        {
            return attendanceObj.BatchInsert(attendanceList);
        }
        else
        {
            return invalidAttendanceList;
        }

    }


    public List<AttendanceInfo> Import_Nike(Stream file, string clientCode, int BU)
    {

        //get mapping table of desc to timslot
        Dictionary<string, string> descToTimeslot = new TimeslotMapping().GetDescToTimeslot();

        List<AttendanceInfo> attendanceList = new List<AttendanceInfo>();
        List<AttendanceInfo> invalidAttendanceList = new List<AttendanceInfo>();
        Attendance attendanceObj = new Attendance();
        AttendanceInfo tempAttendance;

        string[] workerInfo;

        XSSFWorkbook hssfwb = new XSSFWorkbook(file);
        // ISheet sheet = hssfwb.GetSheet("每日出勤表");
        ISheet sheet = hssfwb.GetSheetAt(0);
        IRow currentRow;
        //ICell currentCell;

        string dateFormat = "yyyy-MM-dd";
        string datetimeFormat = "yyyy-MM-dd HH:mm";

        DateTime firstAttendanceDate = DateTime.MinValue;
        try
        {
            currentRow = sheet.GetRow(1);
            firstAttendanceDate = DateTime.ParseExact(currentRow.GetCell(0).StringCellValue, dateFormat, CultureInfo.InvariantCulture);
        }
        catch { }

        if (firstAttendanceDate != DateTime.MinValue)
        {
            for (int row = 1; row <= sheet.LastRowNum; row++)
            {

                //System.Diagnostics.Debug.WriteLine(row);

                currentRow = sheet.GetRow(row);
                if (currentRow != null) //null is when the row only contains empty cells 
                {
                    //考勤日期	小組 	員工編號	員工姓名	班次	扣減工時   (分鐘)	上班	下班	考勤工時	考勤工時2	加班	加班2	Total hrs	備注	hrs	22:00後
                    if (currentRow.GetCell(0) == null || string.IsNullOrEmpty(currentRow.GetCell(0).StringCellValue)) break;
                    if (currentRow.GetCell(2) == null) continue;

                    tempAttendance = new AttendanceInfo();
                    System.Diagnostics.Debug.WriteLine(currentRow.GetCell(0).StringCellValue);

                    tempAttendance.ClientStaffNo = currentRow.GetCell(2).StringCellValue;

                    //get WorkerID /Gender/ PositionGrade
                    workerInfo = attendanceObj.GetWorkderID(clientCode, BU, tempAttendance.ClientStaffNo);
                    tempAttendance.AttendanceDate = DateTime.ParseExact(currentRow.GetCell(0).StringCellValue, dateFormat, CultureInfo.InvariantCulture);
                    tempAttendance.Client = clientCode;
                    tempAttendance.BU = BU;
                    tempAttendance.ClientStaffName = currentRow.GetCell(3).StringCellValue;
                    tempAttendance.Remarks = currentRow.GetCell(4).StringCellValue;
                    tempAttendance.TimeSlot = descToTimeslot[currentRow.GetCell(4).StringCellValue];
                    tempAttendance.Hours = currentRow.GetCell(12).NumericCellValue;

                    if (workerInfo == null)
                    {
                        tempAttendance.Remarks = "Worker Not Found";
                        invalidAttendanceList.Add(tempAttendance);
                        continue;
                    }
                    else
                    {
                        tempAttendance.WorkerID = workerInfo[0];
                        tempAttendance.Gender = workerInfo[1];
                        tempAttendance.PositionGrade = workerInfo[2];

                    }


                    //time in
                    if (currentRow.GetCell(6).CellType == CellType.Numeric)
                    {
                        tempAttendance.TimeIn = currentRow.GetCell(6).DateCellValue;
                    }
                    else if (!string.IsNullOrEmpty(currentRow.GetCell(6).StringCellValue))
                    {
                        tempAttendance.TimeIn = DateTime.ParseExact(currentRow.GetCell(0).StringCellValue + " " + getTimeInExceptionInput(currentRow.GetCell(6).StringCellValue), datetimeFormat, CultureInfo.InvariantCulture);
                    }

                    //time out
                    if (currentRow.GetCell(7).CellType == CellType.Numeric)
                    {
                        tempAttendance.TimeIn = currentRow.GetCell(7).DateCellValue;
                    }
                    else if (!string.IsNullOrEmpty(currentRow.GetCell(7).StringCellValue))
                    {
                        tempAttendance.TimeOut = DateTime.ParseExact(currentRow.GetCell(0).StringCellValue + " " + getTimeInExceptionInput(currentRow.GetCell(7).StringCellValue), datetimeFormat, CultureInfo.InvariantCulture);
                    }


                    attendanceList.Add(tempAttendance);

                }
            }
        }
        else
        {
            //get the week day
            currentRow = sheet.GetRow(1);
            Dictionary<DateTime, int> dateColMapping = new Dictionary<DateTime, int>();
            DateTime workingDate = DateTime.MaxValue;
            for (int col = 3; col < currentRow.LastCellNum - 1; col++)
            {
                System.Diagnostics.Debug.WriteLine(col);
                if (currentRow.GetCell(col) == null) continue;

                if (currentRow.GetCell(col).CellType == CellType.Numeric)
                {
                    dateColMapping.Add(currentRow.GetCell(col).DateCellValue, col);
                    workingDate = currentRow.GetCell(col).DateCellValue;
                }
                else if (!string.IsNullOrEmpty(currentRow.GetCell(col).StringCellValue))
                {
                    if (currentRow.GetCell(col).StringCellValue == "Grand Total") break;
                    workingDate = DateTime.ParseExact(currentRow.GetCell(col).StringCellValue, dateFormat, CultureInfo.InvariantCulture);
                    dateColMapping.Add(workingDate, col);
                }

            }

            //get the working hours from excel file
            for (int row = 3; row < sheet.LastRowNum; row++)
            {
                System.Diagnostics.Debug.WriteLine(row);
                currentRow = sheet.GetRow(row);
                if (currentRow != null) //null is when the row only contains empty cells 
                {
                    if (currentRow.GetCell(1) == null || string.IsNullOrEmpty(currentRow.GetCell(1).StringCellValue)) continue;

                    foreach (DateTime attendanceDate in dateColMapping.Keys)
                    {
                        System.Diagnostics.Debug.WriteLine(attendanceDate.ToString("ddMMyyyy"));
                        if (currentRow.GetCell(dateColMapping[attendanceDate]) == null) continue;

                        tempAttendance = new AttendanceInfo();
                        tempAttendance.Client = clientCode;
                        tempAttendance.BU = BU;

                        tempAttendance.AttendanceDate = attendanceDate;
                        tempAttendance.ClientStaffNo = currentRow.GetCell(1).StringCellValue;
                        if (!descToTimeslot.ContainsKey(currentRow.GetCell(2).StringCellValue))
                        {
                            tempAttendance.Remarks = string.Format("Unknown timeslot: {0}", currentRow.GetCell(2).StringCellValue);
                            invalidAttendanceList.Add(tempAttendance);
                            continue;
                        }
                        tempAttendance.TimeSlot = descToTimeslot[currentRow.GetCell(2).StringCellValue];
                        tempAttendance.Hours = currentRow.GetCell(dateColMapping[attendanceDate]).NumericCellValue;

                        if (tempAttendance.Hours == 0) continue;


                        //get WorkerID /Gender/ PositionGrade
                        workerInfo = attendanceObj.GetWorkderID(clientCode, BU, tempAttendance.ClientStaffNo);
                        if (workerInfo == null)
                        {
                            tempAttendance.Remarks = "Worker Not Found";
                            invalidAttendanceList.Add(tempAttendance);
                            continue;
                        }
                        else
                        {
                            tempAttendance.WorkerID = workerInfo[0];
                            tempAttendance.Gender = workerInfo[1];
                            tempAttendance.PositionGrade = workerInfo[2];

                        }

                        attendanceList.Add(tempAttendance);
                    }
                }
            }

        }


        if (invalidAttendanceList.Count == 0)
        {
            return attendanceObj.BatchInsert(attendanceList);
        }
        else
        {
            return invalidAttendanceList;
        }

    }

    //public List<AttendanceInfo> Import_Nike(Stream file, string clientCode, int BU)
    //{
    //    List<AttendanceInfo> attendanceList = new List<AttendanceInfo>();
    //    List<AttendanceInfo> invalidAttendanceList = new List<AttendanceInfo>();
    //    Attendance attendanceObj = new Attendance();
    //    AttendanceInfo tempAttendance;

    //    XSSFWorkbook hssfwb = new XSSFWorkbook(file);
    //    ISheet sheet = hssfwb.GetSheet("Export to PDF");
    //    IRow currentRow;
    //    //ICell currentCell;

    //    string dateFormat = "MM/dd/yyyy";
    //    DateTime startDate = DateTime.MaxValue;
    //    /// string datetimeFormat = "yyyy-MM-dd HH:mm";

       
    //    string[] workerInfo;



    //    return attendanceList;

   // }

    public void Save(List<AttendanceEntry> attendanceEntries, string clientCode, int BU)
    {
        List<AttendanceInfo> attendanceList = new List<AttendanceInfo>();
        List<AttendanceInfo> invalidAttendanceList = new List<AttendanceInfo>();
        Attendance attendanceObj = new Attendance();
        AttendanceInfo tempAttendance;
        Worker workerObj = new Worker();

        int row = 0;
        DateTime startDate = DateTime.MinValue;
        WorkerInfo tmpWorkerInfo = null;
        foreach (var info in attendanceEntries)
        {
            ++row;
            tmpWorkerInfo = workerObj.CheckWorkerWithBU(clientCode, BU, info.WorkerID);
            if (tmpWorkerInfo == null)
            {
                throw new Exception(string.Format("Worker in row {0} is not valid", row));
            }
            tempAttendance = new AttendanceInfo();
            tempAttendance.WorkerID = info.WorkerID;
            tempAttendance.Gender = tmpWorkerInfo.Gender;
            tempAttendance.PositionGrade = tmpWorkerInfo.PositionGrade;

            tempAttendance.Client = clientCode;
            tempAttendance.BU = BU;

            tempAttendance.AttendanceDate = info.AttendanceDate;
            tempAttendance.TimeSlot = info.Timeslot;
            tempAttendance.TimeIn = DateTime.ParseExact(info.AttendanceDate.ToString(GlobalSetting.DateFormat) + " " + info.TimeIn + ":00", GlobalSetting.DateTimeFormat, CultureInfo.InvariantCulture);
            tempAttendance.TimeOut = DateTime.ParseExact(info.AttendanceDate.ToString(GlobalSetting.DateFormat) + " " + info.TimeOut + ":00", GlobalSetting.DateTimeFormat, CultureInfo.InvariantCulture);
            tempAttendance.Hours = info.TotalHours;

            startDate = startDate > tempAttendance.AttendanceDate.Value ? tempAttendance.AttendanceDate.Value : startDate;

            attendanceList.Add(tempAttendance);
        }

        attendanceObj.BatchInsert(attendanceList);

    }





    public double GetWeelyHours(Dictionary<DateTime, int> dailyHoursDict, DateTime targetDate)
    {
        double weeklyHours = 0;
        int dayofWeek = (int)targetDate.DayOfWeek;
        DateTime startDate = targetDate.AddDays(-dayofWeek);

        for (int i = 0; i <= 6; i++)
        {
            if (dailyHoursDict.ContainsKey(startDate.AddDays(i)))
                weeklyHours += dailyHoursDict[startDate.AddDays(i)];
        }

        return weeklyHours;
    }

    public double GetHalfMonthHours(Dictionary<DateTime, int> dailyHoursDict, DateTime targetDate)
    {
        double halfMonthHours = 0;
        int day = (int)targetDate.Day;
        int[] range1 = new int[] { 26, 10 };
        int[] range2 = new int[] { 11, 25 };
        DateTime tmpDate;// = targetDate.AddDays(-dayofWeek);
        int currentYear = targetDate.Year;
        int currentMonth = targetDate.Month;
        if (day >= range1[0])
        {
            tmpDate = new DateTime(currentYear, currentMonth, range1[0]);
            for (int i = 0;; i++)
            {
                tmpDate = tmpDate.AddDays(i);
                if (tmpDate.Day == range2[1]) break;

                if (dailyHoursDict.ContainsKey(tmpDate))
                    halfMonthHours += dailyHoursDict[tmpDate];
            }
        }
        else if (day <= range1[1])
        {
            tmpDate = new DateTime(currentYear, currentMonth - 1, range1[0]);
            for (int i = 0; ; i++)
            {
                tmpDate = tmpDate.AddDays(i);
                if (tmpDate.Day == range2[1]) break;

                if (dailyHoursDict.ContainsKey(tmpDate))
                    halfMonthHours += dailyHoursDict[tmpDate];
            }
        }
        else if (day >= range2[0] && day <= range2[1])
        {
            tmpDate = new DateTime(currentYear, currentMonth, range2[0]);
            for (int i = 0; ; i++)
            {
                tmpDate = tmpDate.AddDays(i);
                if (tmpDate.Day == range1[0]) break;

                if (dailyHoursDict.ContainsKey(tmpDate))
                    halfMonthHours += dailyHoursDict[tmpDate];
            }
        }


        return halfMonthHours;
    }

    public double GetSatHours(Dictionary<string, Dictionary<DateTime, int>> previousWeekdayWorkingHours, DateTime targetDate, string workerID)
    {
        Dictionary<DateTime, int> dailyHoursDict;
        double satHours = 0;
        int dayofWeek = (int)targetDate.DayOfWeek;
        DateTime startDate = targetDate.AddDays(-dayofWeek);

        for (int i = 1; i <= 3; i++)
        {
            if (!previousWeekdayWorkingHours.ContainsKey(workerID + i.ToString())) continue; 
            dailyHoursDict = previousWeekdayWorkingHours[workerID + i.ToString()];


            if (!dailyHoursDict.ContainsKey(startDate.AddDays(6))) continue;
            satHours += dailyHoursDict[startDate.AddDays(6)];
        }

        return satHours;
    }

    private string getTimeInExceptionInput(string input)
    {
        string result = "";

        MatchCollection matches = Regex.Matches(input, @"\d{2}:\d{2}");
        if (matches.Count > 0)
        {
            result = matches[0].Value;
        }

        return result;
    }

     
}