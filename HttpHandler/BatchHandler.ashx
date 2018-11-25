<%@ WebHandler Language="C#" Class="BatchHandler" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Globalization;

public class BatchHandler : IHttpHandler, IRequiresSessionState
{

    string userID = "";
    public void ProcessRequest(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;
        System.Web.SessionState.HttpSessionState session = context.Session;

        if (session[GlobalSetting.SessionKey.LoginID] != null)
            userID = session[GlobalSetting.SessionKey.LoginID].ToString();


        if (userID == "")
        {
            response.StatusCode = 403;
            response.End();
        }


        string result = "", action = "";
        Dictionary<string, string> resultDict = new Dictionary<string, string>();
        Newtonsoft.Json.Converters.IsoDateTimeConverter IsoDateTimeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = GlobalSetting.DateTimeFormat };
        Newtonsoft.Json.Converters.IsoDateTimeConverter IsoDateConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = GlobalSetting.DateFormat };

        try
        {
            JArray serviceRequest = JArray.Parse(request.Form["params"]);
            foreach (JObject dataDict in serviceRequest)
            {
                action = dataDict["action"].ToString();
                switch (action)
                {
                    case "isStaffNoDuplicated":
                        string WorkerClientListInfoStr = dataDict["WorkerClientListInfo"].ToString();;
                        var WorkerClientListInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkerClientListInfo>(WorkerClientListInfoStr, IsoDateTimeConverter);
                        string workerID = new Worker().IsStaffNoDuplicated(WorkerClientListInfo);
                        if (!string.IsNullOrEmpty(workerID))
                            result = "{\"message\":\"Staff No.  " + WorkerClientListInfo.StaffNo + " already existed in worker " + workerID + "\"}";
                        break;
                    case "getSample":
                        string SampleNo = request["SampleNo"];
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new Sample().Get(SampleNo), IsoDateTimeConverter);
                        break;
                    case "saveSample":
                        string sampleString = dataDict["SampleInfo"].ToString();
                        var sampleInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<SampleInfo>(sampleString, IsoDateTimeConverter);
                        sampleInfo.CreateUser = userID;
                        new Sample().Save(sampleInfo);
                        result = "{\"message\":\"Done.\"}";
                        break;

                    case "getTimeSlot":
                        string timeSlotCode = dataDict["TimeSlotCode"].ToString();
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new TimeSlot().Get(timeSlotCode), IsoDateTimeConverter);
                        break;
                    case "saveTimeSlot":
                        string timeSlotInfoString = dataDict["TimeSlotInfo"].ToString();
                        var timeSlotInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<TimeSlotInfo>(timeSlotInfoString, IsoDateTimeConverter);
                        timeSlotInfo.CreateUser = userID;
                        new TimeSlot().Save(timeSlotInfo);
                        result = "{\"message\":\"Done.\"}";
                        break;
                    case "deleteTimeSlot":
                        timeSlotCode = dataDict["TimeSlotCode"].ToString();
                        new TimeSlot().Delete(timeSlotCode);
                        result = "{\"message\":\"Done.\"}";
                        break;

                    //Introducer 
                    case "getIntroducer":
                        string introducerCode = dataDict["IntroducerCode"].ToString();
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new Introducer().Get(introducerCode), IsoDateTimeConverter);
                        break;
                    case "saveIntroducer":
                        string introducerInfoString = dataDict["IntroducerInfo"].ToString();
                        var introducerInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<IntroducerInfo>(introducerInfoString, IsoDateTimeConverter);
                        introducerInfo.CreateUser = userID;
                        new Introducer().Save(introducerInfo);
                        result = "{\"message\":\"Done.\"}";
                        break;
                    case "deleteIntroducer":
                        introducerCode = dataDict["IntroducerCode"].ToString();
                        new Introducer().Delete(introducerCode);
                        result = "{\"message\":\"Done.\"}";
                        break;


                    //Client 
                    case "getClient":
                        string clientCode = dataDict["ClientCode"].ToString();
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new Client().Get(clientCode), IsoDateTimeConverter);
                        break;
                    case "saveClient":
                        string clientInfoString = dataDict["ClientInfo"].ToString();
                        var clientInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientInfo>(clientInfoString, IsoDateTimeConverter);
                        clientInfo.CreateUser = userID;
                        new Client().Save(clientInfo);
                        result = "{\"message\":\"Done.\"}";
                        break;
                    case "deleteClient":
                        clientCode = dataDict["ClientCode"].ToString();
                        new Client().Delete(clientCode);
                        result = "{\"message\":\"Done.\"}";
                        break;


                    //Worker 
                    case "getWorker":
                        string WorkerID = dataDict["WorkerID"].ToString();
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new Worker().Get(WorkerID), IsoDateTimeConverter);
                        break;
                    case "saveWorker":
                        string WorkerInfoString = dataDict["WorkerInfo"].ToString();
                        var WorkerInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkerInfo>(WorkerInfoString, IsoDateTimeConverter);
                        WorkerInfo.CreateUser = userID;
                        new Worker().Save(WorkerInfo);
                        result = "{\"message\":\"Done.\"}";
                        break;
                    case "deleteWorker":
                        WorkerID = dataDict["WorkerID"].ToString();
                        new Worker().Delete(WorkerID);
                        result = "{\"message\":\"Done.\"}";
                        break;

                    //PayrollGroup 
                    case "getPayrollGroup":
                        string PayrollGroupID = dataDict["PayrollGroupID"].ToString();
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new PayrollGroup().Get(PayrollGroupID), IsoDateTimeConverter);
                        break;
                    case "savePayrollGroup":
                        string PayrollGroupInfoString = dataDict["PayrollGroupInfo"].ToString();
                        var PayrollGroupInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PayrollGroupInfo>(PayrollGroupInfoString, IsoDateTimeConverter);
                        PayrollGroupInfo.CreateUser = userID;
                        new PayrollGroup().Save(PayrollGroupInfo);
                        result = "{\"message\":\"Done.\"}";
                        break;
                    case "deletePayrollGroup":
                        PayrollGroupID = dataDict["PayrollGroupID"].ToString();
                        new PayrollGroup().Delete(PayrollGroupID);
                        result = "{\"message\":\"Done.\"}";
                        break;


                    //UserProfile 
                    case "getUserProfile":
                        string UserID = dataDict["UserID"].ToString();
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new UserProfile().Get(UserID), IsoDateTimeConverter);
                        break;
                    case "saveUserProfile":
                        string UserProfileInfoString = dataDict["UserProfileInfo"].ToString();
                        var UserProfileInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileInfo>(UserProfileInfoString, IsoDateTimeConverter);
                        UserProfileInfo.CreateUser = userID;
                        new UserProfile().Save(UserProfileInfo);
                        result = "{\"message\":\"Done.\"}";
                        break;
                    case "deleteUserProfile":
                        UserID = dataDict["UserID"].ToString();
                        new UserProfile().Delete(UserID);
                        result = "{\"message\":\"Done.\"}";
                        break;

                    case "changePassword":
                        string currentPassword = dataDict["CurrentPassword"].ToString();
                        string newPassword = dataDict["NewPassword"].ToString();
                        bool status = new UserProfile().UpdatePassword(userID, currentPassword, newPassword);
                        if (status)
                            result = "{\"message\":\"Done.\"}";
                        else
                        {
                            result = "{\"message\":\"Failure to update the password, please input the correct password.\"}";
                        }
                        break;


                    case "inquiry":

                        JArray filters = JArray.Parse(dataDict["Filters"].ToString());
                        string[] Fields = null;
                        if (dataDict["Fields"] != null)
                            Fields = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(dataDict["Fields"].ToString());
                        string view = dataDict["View"].ToString();

                        result = new Inquiry().View_Search(filters, Fields, view);
                        break;

                    //Hourly Rate 
                    case "saveHourlyRateMapping":
                        string HourlyRateMappingString = dataDict["HourlyRateMapping"].ToString();
                        var hourlyRateMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HourlyRateMappingInfo>>(HourlyRateMappingString, IsoDateTimeConverter);
                        new HourlyRateMapping().Save(hourlyRateMapping);
                        result = "{\"message\":\"Done.\"}";
                        break;
                    case "saveClientHourlyRateMapping":
                        clientCode = dataDict["ClientCode"].ToString();
                        string BUCode = dataDict["BUCode"].ToString();
                        string positionGrade = dataDict["PositionGrade"].ToString();
                        HourlyRateMappingString = dataDict["HourlyRateMapping"].ToString();
                        hourlyRateMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HourlyRateMappingInfo>>(HourlyRateMappingString, IsoDateTimeConverter);
                        new HourlyRateMapping().Save(hourlyRateMapping, clientCode, BUCode, positionGrade);
                        result = "{\"message\":\"Done.\"}";
                        break;
                    case "getHourlyRateMapping":
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new HourlyRateMapping().Get(), IsoDateTimeConverter);
                        break;

                    case "getClientHourlyRateMapping":
                        clientCode = dataDict["ClientCode"].ToString();
                        BUCode = dataDict["BUCode"].ToString();
                        positionGrade = dataDict["PositionGrade"].ToString();
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new HourlyRateMapping().Get(clientCode, BUCode, positionGrade), IsoDateTimeConverter);
                        break;

                    case "generatePayroll":
                        string PayrollGroup = dataDict["PayrollGroup"].ToString();
                        DateTime asat = DateTime.ParseExact(dataDict["AsAt"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        DateTime SalaryDate = DateTime.ParseExact(dataDict["SalaryDate"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        string remarks = dataDict["Remarks"].ToString();
                        new Payroll().Generate(PayrollGroup, asat, SalaryDate, remarks);
                        result = "{\"message\":\"Done.\"}";
                        break;

                    case "getPayroll":
                        WorkerID = dataDict["WorkerID"].ToString();
                        SalaryDate = DateTime.ParseExact(dataDict["SalaryDate"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new Payroll().Get(WorkerID, SalaryDate), IsoDateConverter);
                        break;

                    case "savePayroll":
                        WorkerID = dataDict["WorkerID"].ToString();
                        var payroll = Newtonsoft.Json.JsonConvert.DeserializeObject<PayrollInfo>(dataDict["Payroll"].ToString(), IsoDateConverter);
                        SalaryDate = DateTime.ParseExact(dataDict["SalaryDate"].ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        payroll.SalaryDate = SalaryDate;
                        payroll.WorkerID = WorkerID;
                        new Payroll().Save(payroll);
                        result = "{\"message\":\"Done.\"}";
                        break;

                    case "getTimeslotMapping":
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new TimeslotMapping().GetALL(), IsoDateConverter);
                        break;

                    case "saveTimeslotMapping":
                        var TimeslotMappingList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TimeslotMappingInfo>>(dataDict["TimeslotMappingList"].ToString(), IsoDateConverter);
                        new TimeslotMapping().Save(TimeslotMappingList);
                        result = "{\"message\":\"Done.\"}";
                        break;

                    case "getGeneralMaster":
                        var category = dataDict["Category"].ToString();
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new GeneralMaster().getGeneralMasterList(category), IsoDateConverter);
                        break;
                    case "saveGeneralMaster":
                        var generalMasterList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GeneralMasterInfo>>(dataDict["GeneralMasterList"].ToString(), IsoDateConverter);
                        new GeneralMaster().Save(generalMasterList);
                        result = "{\"message\":\"Done.\"}";
                        break;

                    case "getHoliday":
                        var year = Convert.ToInt16(dataDict["Year"].ToString());
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new Holiday().GetHoliday(year), IsoDateTimeConverter);
                        break;
                    case "saveHoliday":
                        year = Convert.ToInt16(dataDict["Year"].ToString());
                        var holidayList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HolidayInfo>>(dataDict["HolidayList"].ToString(), IsoDateTimeConverter);
                        new Holiday().Save(holidayList, year);
                        result = "{\"message\":\"Done.\"}";
                        break;

                    case "saveAttendanceEntries":
                        clientCode = dataDict["ClientCode"].ToString();
                        var BU = Convert.ToInt16(dataDict["BU"].ToString());
                        var AttendanceList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AttendanceEntry>>(dataDict["AttendanceList"].ToString(), IsoDateTimeConverter);
                        new AttendanceImport().Save(AttendanceList, clientCode, BU);
                        result = "{\"message\":\"Done.\"}";
                        break;
                            
                    case "beneficialNameExists":
                        WorkerID = dataDict["WorkerID"].ToString();
                        var beneficialName  = dataDict["BeneficialName"].ToString();
                        var trueFalse = new Worker().BeneficialNameExists(beneficialName, WorkerID);
                        result = "{\"result\": \""+trueFalse+"\"}";
                        break;


                    default:
                        break;

                }
                resultDict.Add(action, result);
            }

        }
        catch (Exception e)
        {
            resultDict.Add("error", e.Message.Replace("\r\n", ""));
            Log.Error(e.Message);
            Log.Error(e.StackTrace);
        }

        response.Clear();
        response.AddHeader("Access-Control-Allow-Origin", "http://localhost:8080");
        response.AddHeader("Access-Control-Allow-Credentials", "true");
        response.ContentType = "application/json;charset=UTF-8;";
        response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(resultDict));
        response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }


}
