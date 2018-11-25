<%@ WebHandler Language="C#" Class="LoginHandler" %>

using System;
using System.Web;
using System.Web.SessionState;

public class LoginHandler : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {

        HttpRequest request = context.Request;
        HttpResponse response = context.Response;
        HttpSessionState session = context.Session;

        string userID = "";
        string result = "";

        try
        {

            string action = request.Form["action"].ToString();

            switch (action)
            {
                case "login":
                    userID = request["UserID"];
                    string password = request["Password"];
                    bool success = new UserProfile().Login(userID, password);
                    if (success)
                    {
                        session[GlobalSetting.SessionKey.LoginID] = userID;
                        result = "{\"result\":\"1\"}";
                    }
                    else
                    {
                        result = "{\"result\":\"0\"}";
                    }
                    break;


                default:
                    break;

            }
        }
        catch (Exception e)
        {
            result = "{\"message\":\"" + e.Message.Replace("\r\n", "") + "\"}";
            Log.Error(e.Message);
            Log.Error(e.StackTrace);
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


