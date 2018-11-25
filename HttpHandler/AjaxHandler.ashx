<%@ WebHandler Language="C#" Class="AjaxHandler" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;

public class AjaxHandler : IHttpHandler, IRequiresSessionState
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
                case "getSample":
                    string SampleNo = request["SampleNo"];
                    result = Newtonsoft.Json.JsonConvert.SerializeObject(new Sample().Get(SampleNo), IsoDateTimeConverter);
                    break; 

                case "getGeneralMasterList":
                    string[] masterNames = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(request["categories"]);
                    result = Newtonsoft.Json.JsonConvert.SerializeObject(new GeneralMaster().GetGeneralMasterList(masterNames));
                    break; 
                case "refreshList":
                    string table = request["Table"];
                    string input = request["Input"]; 
                    result = Newtonsoft.Json.JsonConvert.SerializeObject(new GeneralMaster().RefreshTableList(table, input), new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = GlobalSetting.DateTimeFormat });
                    break; 
                case "refreshBUList":
                    table = request["Table"];
                    input = request["Input"];
                    string clientCode = request["ClientCode"];
                    result = Newtonsoft.Json.JsonConvert.SerializeObject(new GeneralMaster().RefreshBUTableList(table, input, clientCode));
                    break; 
                case "refreshWorkerList":
                    table = request["Table"];
                    input = request["Input"];
                    clientCode = request["ClientCode"];
                    int BU = Convert.ToInt16(request["BU"]);
                    result = Newtonsoft.Json.JsonConvert.SerializeObject(new GeneralMaster().RefreshWorkerList(table, input, clientCode, BU));
                    break; 

                default:
                    break;

            }
                 
            //Log.Info(action);
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

 