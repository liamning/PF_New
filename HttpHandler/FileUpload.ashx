<%@ WebHandler Language="C#" Class="FileUpload" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
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
            string action = request.Form["action"].ToString();

            switch (action)
            {
                case "saveSample":
                    string sampleString = request["SampleInfo"];
                    var sampleInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<SampleInfo>(sampleString, IsoDateTimeConverter);
                    sampleInfo.CreateUser = userID;
                    new Sample().Save(sampleInfo);
                    result = "{\"message\":\"Done.\"}";
                    break;

                case "attendanceImport":
                    HttpPostedFile file = request.Files["file"];

                    if (file == null || file.ContentLength == 0)
                    {
                        result = "{\"message\":\"Please select the file.\"}";
                    }
                    else
                    {
                        string clientCode = request.Form["ClientCode"].ToString();
                        int BU = Convert.ToInt32(request.Form["BU"].ToString());
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new AttendanceImport().Import(file.InputStream, clientCode, BU), IsoDateConverter);
                    }
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
        response.AddHeader("Access-Control-Allow-Origin", "http://localhost:8080");
        response.AddHeader("Access-Control-Allow-Credentials", "true");
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

 