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
                        string[] Fields = null;
                        if (request.QueryString["Fields"] != null)
                            Fields = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(request.QueryString["Fields"].ToString());
                        string view = request.QueryString["View"].ToString();

                        MemoryStream exportData = new Inquiry().Export_Search(filters, Fields, view);
                         
                        response.Clear(); 
                        response.ContentType = "application/vnd.ms-excel";
                        response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "tpms_dict.xls"));
                        response.BinaryWrite(exportData.GetBuffer());
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

 