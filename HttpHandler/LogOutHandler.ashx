<%@ WebHandler Language="C#" Class="LogoutHandler" %>

using System;
using System.Web;
using System.Web.SessionState;

public class LogoutHandler : IHttpHandler, IRequiresSessionState
{
     
    public void ProcessRequest(HttpContext context)
    { 

        HttpRequest request = context.Request;
        HttpResponse response = context.Response;
        HttpSessionState session = context.Session;

        session.Clear(); 
        
        response.Clear();
        response.AddHeader("Access-Control-Allow-Origin", "http://localhost:8080");   
         response.AddHeader("Access-Control-Allow-Credentials", "true");
        response.Redirect("~/");
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

