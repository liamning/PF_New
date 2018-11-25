<%@ WebHandler Language="C#" Class="WebAPI" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class WebAPI : IHttpHandler, IRequiresSessionState
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
            string action = request.Form["action"];
            switch (action)
            {
                //Introducer 
                case "getIntroducer":
                    string introducerCode = request.Form["IntroducerCode"].ToString();
                    result = Newtonsoft.Json.JsonConvert.SerializeObject(new Introducer().Get(introducerCode), IsoDateTimeConverter);
                    break;
                        
                    case "getClient":
                        string clientCode = request.Form["ClientCode"].ToString();
                        result = Newtonsoft.Json.JsonConvert.SerializeObject(new Client().Get(clientCode), IsoDateTimeConverter);
                        break;
                    case "saveClient":
                        string clientInfoString = request.Form["ClientInfo"].ToString();
                        var clientInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientInfo>(clientInfoString, IsoDateTimeConverter);
                        clientInfo.CreateUser = userID;
                        new Client().Save(clientInfo);
                        result = "{\"message\":\"Done.\"}";
                        break;
                    //case "saveIntroducer":
                    //    string introducerInfoString = dataDict["IntroducerInfo"].ToString();
                    //    var introducerInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<IntroducerInfo>(introducerInfoString, IsoDateTimeConverter);
                    //    introducerInfo.CreateUser = userID;
                    //    new Introducer().Save(introducerInfo);
                    //    result = "{\"message\":\"Done.\"}";
                    //    break;
                    //case "deleteIntroducer":
                    //    introducerCode = dataDict["IntroducerCode"].ToString();
                    //    new Introducer().Delete(introducerCode);
                    //    result = "{\"message\":\"Done.\"}";
                    //    break;
            }
            //string table = "Worker";
            //string input = "";
            //result = Newtonsoft.Json.JsonConvert.SerializeObject(new GeneralMaster().RefreshTableList(table, input), new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = GlobalSetting.DateTimeFormat });


        }
        catch (Exception e)
        {
            result = "{\"message\":\"" + e.Message.Replace("\r\n", "") + "\"}";
            Log.Error(e.Message + "\r\n" + e.StackTrace);
        }

        response.Clear();
        response.AddHeader("Access-Control-Allow-Origin", "*");
        response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PATCH, PUT, DELETE, OPTIONS");
        response.AddHeader("Access-Control-Allow-Headers", "Origin, Content-Type, X-Auth-Token");
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
