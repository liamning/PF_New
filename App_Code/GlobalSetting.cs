using System;
using System.Collections.Generic;
using System.IO;
using System.Web; 
using System.Web.SessionState;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;


/// <summary>
/// Summary description for GlobalSetting
/// </summary>
public class GlobalSetting
{
    public const string DateFormat = "dd/MM/yyyy";
    public const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";

    public struct PolicyKey
    {
        public const string POTotalAmountDecimal = "POTotalAmountDecimal";
        public const string PRCheckQtyDays = "PRCheckQtyDays";
        public const string PRCheckQtyPercentage = "PRCheckQtyPercentage";
        public const string TMSCOMDB = "TMSCOMDB";
        public const string TMSCustInvoiceLocation = "TMSCustInvoiceLocation";
        public const string TMSCustInvoiceTxnCode = "TMSCustInvoiceTxnCode";
        public const string TMSInterfaceCreateUser = "TMSInterfaceCreateUser";
        public const string TMSSuppInvoiceLocation = "TMSSuppInvoiceLocation";
        public const string TMSSuppInvoiceTxnCode = "TMSSuppInvoiceTxnCode";
        public const string TMSWorkshopCompanyDB = "TMSWorkshopCompanyDB";
        public const string TMSWorkshopCompanySuppCode = "TMSWorkshopCompanySuppCode";
    }

    //public static readonly Dictionary<string, PolicyInfo> PolicyDict = new PolicyMaster().GetPolicyDict();
   // public static readonly string TMSCOMDBPolicyValue = GlobalSetting.PolicyDict[GlobalSetting.PolicyKey.TMSCOMDB].PolicyValue;
    //public static readonly string TMSWorkshopCompanyDBPolicyValue = GlobalSetting.PolicyDict[GlobalSetting.PolicyKey.TMSWorkshopCompanyDB].PolicyValue;

    public struct SessionKey
    {
        public const string LoginID = "LOGINID";
        public const string LoginRole = "LoginRole";
        public const string Location = "Location";
        public const string LoginRoleList = "LoginRoleList";
        public const string LoggedOut = "LoggedOut";
    }

    public const string TMSMinimumDate = "1900-01-01";
    public const string Culture = "en-US";
    public const string LoginPage = "/#/login";
    public const string Login_KioskPage = "/#/login_Kiosk";
    public const string AjaxHandler = "HttpHandler/AjaxHandler.ashx";
    public const string LoginHandler = "HttpHandler/LoginHandler.ashx";

    public struct ShiftSource
    {
        public const string RosterTempate = "RT";
        public const string ShiftAdjustment = "AD";
    }
     
    public static string GetHashPassword(string password)
    {
        int salt = 0;

        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
        string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
        return base64digest.Substring(0, base64digest.Length - 2);
    } 

}