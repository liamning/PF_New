using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Dapper;

/// <summary>
/// Summary description for Sample
/// </summary>
public class GeneralMaster
{
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    SqlTransaction transaction;

    public Dictionary<string, List<GeneralCodeDesc>> GetGeneralMasterList(string[] masterNames)
    {
        Dictionary<string, List<GeneralCodeDesc>> dict = new Dictionary<string, List<GeneralCodeDesc>>();

        foreach (string masterName in masterNames)
        {
            switch (masterName)
            {
                case "GeneralCategory":
                    dict.Add(masterName, this.GetCategoryList());
                    break;
                //case "TransactionType":
                //    dict.Add(masterName, new POManagement().getTransactionList());
                //    break;
                //case "Supplier":
                //    dict.Add(masterName, new POManagement().GetSupplierList());
                //    break;
                //case "Currency":
                //    dict.Add(masterName, new PRManagement().GetCurrencyList());
                //    break;
                //case "PriceTerm":
                //    dict.Add(masterName, new POManagement().GetPriceTermList());
                //    break;
                //case "PaymentTerm":
                //    dict.Add(masterName, new POManagement().GetPaymentList());
                //    break;
                //case "Unit":
                //    dict.Add(masterName, new POManagement().GetUnitList());
                //    break;
                default:
                    dict.Add(masterName, this.getGeneralMaster(masterName));
                    break;
            }
        }

        return dict;
    }

    public List<GeneralCodeDesc> GetRoleList()
    {
        db.Open();
        String query = "select RoleCode Code, RoleName [Desc] from RoleHeader order by RoleName";
        var obj = (List<GeneralCodeDesc>)db.Query<GeneralCodeDesc>(query);
        db.Close();
        return obj;
    }

    public List<GeneralCodeDesc> RefreshTableList(string tableName, string input)
    {
        switch (tableName)
        {
            case "Sample":
                return this.RefreshSampleList(input);
            case "TimeSlot":
                return new TimeSlot().GetTimeSlotCodeList(input);
            case "Introducer":
                return new Introducer().GetIntroducerCodeList(input);
            case "Client":
                return new Client().GetClientCodeList(input);
            case "Worker":
                return new Worker().GetWorkerIDList(input);
            case "PayrollGroup":
                return new PayrollGroup().GetPayrollGroupIDList(input);
            case "UserProfile":
                return new UserProfile().GetUserProfileList(input);
            case "SalaryDate":
                return this.GetSalaryDateList(input);
                //case "Supplier":
                //    return this.RefreshSupplierList(input);
                //case "Staff":
                //    return new StaffProfile().GetStaffNoList(input);
                //case "SupplierGRN":
                //    return new GRNHeader().GetGRNNoList(input);
                //case "SuppInvNo":
                //    return new GRNHeader().RefreshInvList(input);
                //case "Role":
                //    return new RoleFunction().GetRoleCodeList(input);
                //case "PO":
                //    return new POManagement().RefreshPOList(input); 
        }

        return null;
    }

    public List<GeneralCodeDesc> GetSalaryDateList(string input)
    {
        int year = 0;
        if (input.Length < 4)
            year = DateTime.Now.Year;
        else
            year = Convert.ToInt16(input);

        db.Open();
        string query = @"select distinct SalaryDate from Payroll where Year(SalaryDate) = @Year";
        var obj = db.Query<dynamic>(query, new { Year = year });
        var result = new List<GeneralCodeDesc>();

        db.Close();

        foreach (var info in obj)
        {
            result.Add(new GeneralCodeDesc()
            {
                Code = info.SalaryDate.ToString("dd/MM/yyyy HH:mm:ss"),
                Desc = info.SalaryDate.ToString("dd/MM/yyyy"),
            });
        }

        return result;

    }
    public List<GeneralCodeDesc> RefreshBUTableList(string tableName, string input, string clientCode)
    {
        switch (tableName)
        {
            case "BU":
                return new Client().GetBUList(clientCode, input);
                //case "Supplier":
                //    return this.RefreshSupplierList(input);
                //case "Staff":
                //    return new StaffProfile().GetStaffNoList(input);
                //case "SupplierGRN":
                //    return new GRNHeader().GetGRNNoList(input);
                //case "SuppInvNo":
                //    return new GRNHeader().RefreshInvList(input);
                //case "Role":
                //    return new RoleFunction().GetRoleCodeList(input);
                //case "PO":
                //    return new POManagement().RefreshPOList(input); 
        }

        return null;
    }
    
    public List<GeneralCodeDesc> RefreshWorkerList(string tableName, string input, string clientCode, int BU)
    {
        switch (tableName)
        {
            case "WorkerHKIDName":
                return new Worker().GetWorkerIDListByHKIDName(input, clientCode, BU);
        }

        return null;
    }
    

    private List<GeneralCodeDesc> RefreshSampleList(string SampleNo)
    {
        db.Open();
        string query = @"(select SampleNo Code, SampleText [Desc] from [Sample] where @SampleNo = SampleNo)
                        union
                    (select top 10 SampleNo Code, SampleText [Desc] from [Sample]
                    where (@SampleNo = '' or SampleNo like '%' + @SampleNo + '%' ))  order by SampleNo";
        var obj = (List<GeneralCodeDesc>)db.Query<GeneralCodeDesc>(query, new { SampleNo = SampleNo });
        db.Close();
        return obj;
    }

    public List<GeneralCodeDesc> getGeneralMaster(string category)
    {
        this.db.Open();

        try
        {
            string query = @" 
                SELECT [Code] Code
                      ,[EngDesc] [Desc]
                  FROM [dbo].[GeneralMaster]
                  where Category = @Category order by Seq
                ";
            List<GeneralCodeDesc> result = (List<GeneralCodeDesc>)this.db.Query<GeneralCodeDesc>(query, new { category = category });
            return result;
        }
        catch
        {
            throw;
        }
        finally
        {
            this.db.Close();
        }
    }
    public List<GeneralMasterInfo> getGeneralMasterList(string category)
    {
        this.db.Open();

        try
        {
            string query = @" 
                SELECT *
                  FROM [dbo].[GeneralMaster]
                  where Category = @Category order by Seq
                ";
            List<GeneralMasterInfo> result = (List<GeneralMasterInfo>)this.db.Query<GeneralMasterInfo>(query, new { category = category });
            return result;
        }
        catch
        {
            throw;
        }
        finally
        {
            this.db.Close();
        }
    }
    public List<GeneralCodeDesc> GetCategoryList()
    {
        this.db.Open();

        try
        {
            string query = @" 
                SELECT distinct Category Code, CategoryDesc [Desc]
                  FROM [dbo].[GeneralMaster] where (IsLocked != 1 or IsLocked is null) order by CategoryDesc
                ";
            List<GeneralCodeDesc> result = (List<GeneralCodeDesc>)this.db.Query<GeneralCodeDesc>(query);
            return result;
        }
        catch
        {
            throw;
        }
        finally
        {
            this.db.Close();
        }
    }

    //    private List<GeneralCodeDesc> getLocationList()
    //    {
    //        this.db.Open();

    //        try
    //        {
    //            string query = @"
    //                select '-' Code, '-' [Desc]
    //                union all
    //                select CustomerCode [Code], CustomerName [Desc] from " + GlobalSetting.TMSWorkshopCompanyDBPolicyValue + @".[dbo].[TmsCustomer]
    //                ";
    //            List<GeneralCodeDesc> result = (List<GeneralCodeDesc>)this.db.Query<GeneralCodeDesc>(query);
    //            return result;
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //        finally
    //        {
    //            this.db.Close();
    //        }
    //    }

    //    private List<GeneralCodeDesc> RefreshLocationList(string LocationCode)
    //    {
    //        db.Open();
    //        string query = @"(select CustomerCode Code, CustomerName [Desc] from " + GlobalSetting.TMSWorkshopCompanyDBPolicyValue + @".[dbo].[TmsCustomer] where @Code = CustomerCode)
    //                    union
    //                (select top 10 CustomerCode Code, CustomerName [Desc] from " + GlobalSetting.TMSWorkshopCompanyDBPolicyValue + @".[dbo].[TmsCustomer]
    //                where (@Code = '' or CustomerCode like '%' + @Code + '%' or CustomerName like '%' + @Code + '%'))  order by CustomerCode";
    //        var obj = (List<GeneralCodeDesc>)db.Query<GeneralCodeDesc>(query, new { Code = LocationCode });
    //        db.Close();
    //        return obj;
    //    }

    //    private List<GeneralCodeDesc> RefreshSupplierList(string supplierCode)
    //    {
    //        string query = @"(select SupplierCode Code, SupplierName [Desc] from " + GlobalSetting.TMSWorkshopCompanyDBPolicyValue + @".[dbo].[TmsSupplier] where @Code = SupplierCode)
    //                    union
    //                (select top 10 SupplierCode Code, SupplierName [Desc] from " + GlobalSetting.TMSWorkshopCompanyDBPolicyValue + @".[dbo].[TmsSupplier]
    //                where (@Code = '' or SupplierCode like '%' + @Code + '%' ))  order by SupplierCode";
    //        var obj = (List<GeneralCodeDesc>)db.Query<GeneralCodeDesc>(query, new { Code = supplierCode });
    //        db.Close();
    //        return obj;
    //    }



    public bool IsExisted(GeneralMasterInfo info)
    {
        String query = "select count(*)  from GeneralMaster "
        + " where Category=@Category and Seq = @Seq ";
        var obj = (List<int>)db.Query<int>(query, info, transaction);
        return obj[0] > 0;
    }

    public void Save(List<GeneralMasterInfo> list)
    {
        this.db.Open();
        this.transaction = this.db.BeginTransaction();
        try
        {
            List<int> seqList = new List<int>();
            foreach (var info in list)
            {

                if (this.IsExisted(info))
                    this.Update(info);
                else
                    this.Insert(info);

                seqList.Add(info.Seq);
            }
            this.DeleteNotIn(list[0].Category, seqList);


            this.transaction.Commit();
        }
        catch
        {
            this.transaction.Rollback();
            throw;
        }
        finally
        {
            this.db.Close();
        }

    }


    public GeneralMasterInfo Get(int ID)
    {
        db.Open();

        string query = "select * from GeneralMaster "
        + " where ID = @ID ";

        var obj = (List<GeneralMasterInfo>)db.Query<GeneralMasterInfo>(query, new { ID = ID });
        db.Close();

        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public void DeleteNotIn(string category, List<int> seqList)
    { 
        string query = "delete  from GeneralMaster "
        + " where Category = @Category and Seq not in @SeqList ";

        db.Execute(query, new { Category = category, SeqList = seqList }, this.transaction); 
    }

    public void Update(GeneralMasterInfo info)
    {

        string query = " UPDATE [dbo].[GeneralMaster] SET  "
        + " [Code] = @Code "
        + ", [EngDesc] = @EngDesc "
        + " where Category=@Category and Seq = @Seq";


        db.Execute(query, info, this.transaction);
    }

    public void Insert(GeneralMasterInfo info)
    { 

        string query = "INSERT INTO [dbo].[GeneralMaster] (  "
        + "[Category] "
        + ",[CategoryDesc] "
        + ",[Seq] "
        + ",[Code] "
        + ",[EngDesc] "
        + ",[ChiDesc] "
        + ",[IsLocked] "
        + ",[CreateUser] "
        + ",[CreateDate] "
        + ",[LastModifiedUser] "
        + ",[LastModifiedDate] "
        + ") "
        + "VALUES ( "
        + " @Category "
        + ",@CategoryDesc "
        + ",@Seq "
        + ",@Code "
        + ",@EngDesc "
        + ",@ChiDesc "
        + ",@IsLocked "
        + ",@CreateUser "
        + ",@CreateDate "
        + ",@LastModifiedUser "
        + ",@LastModifiedDate "
        + ") ";


        db.Execute(query, info, this.transaction);
    }
}