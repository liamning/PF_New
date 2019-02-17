<%@ WebHandler Language="C#" Class="FileUpload" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
    
using System.IO;

public class FileUpload : IHttpHandler, IRequiresSessionState
{

    string userID = "";
    public void ProcessRequest(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;
        System.Web.SessionState.HttpSessionState session = context.Session;


        if (session[GlobalSetting.SessionKey.LoginID] != null)
            userID = session[GlobalSetting.SessionKey.LoginID].ToString();

        string result = "";
        Newtonsoft.Json.Converters.IsoDateTimeConverter IsoDateTimeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = GlobalSetting.DateTimeFormat };
        Newtonsoft.Json.Converters.IsoDateTimeConverter IsoDateConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = GlobalSetting.DateFormat };

        try
        {
            string action = request.QueryString["action"].ToString();

            switch (action)
            {

                case "attendance":

                    JArray filters = JArray.Parse(request.QueryString["Filters"].ToString());
                    string[] Fields = new string[] { "WorkerID", "PayrollGroupDesc", "Client", "ClientStaffNo", "ChineseName", "AttendanceDate", "Hours", "OTHours", "TimeIn", "TimeOut", "HourRate", "OTHourRate", "Amount", "BankName", "BeneficialName", "BankACNo" };
                    if (request.QueryString["Fields"] != null)
                        Fields = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(request.QueryString["Fields"].ToString());
                    string view = request.QueryString["View"].ToString();
                    string[] headers = new string[] { "Worker ID", "Payroll Group Desc", "Client", "Client Staff No", "Chinese Name", "Attendance Date", "Hours", "OT Hours", "Time In", "Time Out", "Hour Rate", "OT Hour Rate", "Amount", "Bank Name", "Beneficial Name", "Bank AC No", };

                    MemoryStream exportData = new Inquiry().Export_Search(filters, headers, Fields, view);

                    response.Clear();
                    response.ContentType = "application/vnd.ms-excel";
                    response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "tpms_dict.xls"));
                    response.BinaryWrite(exportData.GetBuffer());
                    response.End();
                    break;
                case "payroll":

                    filters = JArray.Parse(request.QueryString["Filters"].ToString());


                    //view 1
                    Fields = new string[] { "*" };
                    if (request.QueryString["Fields"] != null)
                        Fields = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(request.QueryString["Fields"].ToString());
                    headers = new string[] { "Worker ID", "Salary Date", "Row No", "Item Code", "Item Description", "Amount", "PayrollGroup", };
                    view = "V_Payroll_Item";

                    //view 2
                    string[] headers2 = new string[] { "Worker ID", "Salary Date", "Row No", "Item Code", "Item Description", "Amount", "PayrollGroup", };
                    string[] Fields2 = new string[] { "*" };
                    string view2 = "V_Payroll_Amount";
                    
                    
                    exportData = new Inquiry().Export_Search(filters, headers, Fields, view, headers2, Fields2, view2);

                    response.Clear();
                    response.ContentType = "application/vnd.ms-excel";
                    response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "tpms_dict.xls"));
                    response.BinaryWrite(exportData.GetBuffer());
                    response.End();
                    break;
                case "attachment":
                    string key = request.QueryString["key"].ToString();
                    string[] workerKey = key.Split('_');
                    var workerAtta = new WorkerAttachment();
                    var info = workerAtta.Get(workerKey[0], Convert.ToInt32(workerKey[1]));
                    byte[] fileArray = Convert.FromBase64String(GlobalSetting.ReadFile(key));
                    response.Clear();
                    response.ContentType = info.MIME;
                    response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", info.FileName));
                    response.BinaryWrite(fileArray);
                    response.End();
                    break;  

                default:
                    break;

            }

        }
        catch (Exception e)
        {
            result = "{\"message\":\"" + e.Message.Replace("\r\n", "") + "\"}";
            Log.Error(e.Message + "\r\n" + e.StackTrace);
        }

        response.Clear();
        response.ContentType = "application/json;charset=UTF-8;";
        response.Write(result);
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

 