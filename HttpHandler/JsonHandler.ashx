<%@ WebHandler Language="C#" Class="JsonHandler" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;

public class JsonHandler : IHttpHandler, IRequiresSessionState
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
            string table = request.QueryString["Table"];
            string input = request.QueryString["Input"];
            result = Newtonsoft.Json.JsonConvert.SerializeObject(new GeneralMaster().RefreshTableList(table, input), new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = GlobalSetting.DateTimeFormat });


        }
        catch (Exception e)
        {
            result = "{\"message\":\"" + e.Message.Replace("\r\n", "") + "\"}";
            Log.Error(e.Message + "\r\n" + e.StackTrace);
        }

        response.Clear();
        response.AddHeader("Access-Control-Allow-Origin", "*");
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

 