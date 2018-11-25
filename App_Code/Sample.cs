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
public class Sample
{
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    SqlTransaction transaction;
	public Sample()
	{
		//
		// TODO: Add constructor logic here
		//
    }

    public List<string> GetSampleNoList(string SampleNo)
    { 
        db.Open();
        String query = "select top 10 SampleNo from Sample where (@SampleNo = '' or SampleNo like '%' + @SampleNo + '%') order by SampleNo";
        var obj = (List<string>)db.Query<string>(query, new { SampleNo = SampleNo });
        db.Close();
        return obj;
    }

    public SampleInfo Get(string SampleNo)
    {
        db.Open();
        String query = "select * from Sample where SampleNo=@SampleNo";
        var obj = (List<SampleInfo>)db.Query<SampleInfo>(query, new { SampleNo = SampleNo });
        db.Close();


        if (obj.Count > 0)
        {
           // obj[0].StaffList = this.getStaffList(SampleNo);

            return obj[0];
        }

        else
            return null;
    }

    public bool IsExisted(string sampleNo)
    {
        db.Open();
        string query = "select count(*) from [dbo].[Sample] where SampleNo = @SampleNo ";
        var obj = (List<int>)db.Query<int>(query, new { SampleNo = sampleNo });
        db.Close();
        return obj[0] > 0;
    }

    public void Save(SampleInfo sampleInfo)
    {
        if(this.IsExisted(sampleInfo.SampleNo))
            this.Update(sampleInfo);
        else
            this.Insert(sampleInfo); 
    }

    public void Update(SampleInfo sampleInfo)
    {
        db.Open();
        transaction = db.BeginTransaction();
        try
        { 
            string query =
              @"
UPDATE [dbo].[Sample]
SET
 [SampleText] = @SampleText
,[SampleTextarea] = @SampleTextarea
,[SampleRadioButton] = @SampleRadioButton
,[Email] = @Email
,[Relationship] = @Relationship
,[Asset] = @Asset
,[Liability] = @Liability
,[SampleDate] = @SampleDate
,[SampleTime] = @SampleTime
,[UpdateDate] = @UpdateDate
,[UpdateUser] = @UpdateUser 
WHERE SampleNo = @SampleNo 

";
 
            db.Execute(query, sampleInfo, transaction);

            //List<string> staffIDList = new List<string>();
            //foreach (StaffInfo info in sampleInfo.StaffList)
            //{
            //    staffIDList.Add(info.ID);
            //}
            //this.DeleteStaffNotInTheList(staffIDList, sampleInfo.SampleNo);

            //foreach (StaffInfo staff in sampleInfo.StaffList)
            //{
            //    if (!this.StaffIsExisted(sampleInfo.SampleNo, staff.ID))
            //    {
            //        staff.CreateDate = DateTime.Now;
            //        staff.SampleNo = sampleInfo.SampleNo;
            //        this.InsertStaff(staff);
            //    }
            //    else
            //    {
            //        staff.UpdateDate = DateTime.Now;
            //        staff.SampleNo = sampleInfo.SampleNo;
            //        this.UpdateStaff(staff);
            //    }
                 
            //}

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            transaction.Dispose();
            transaction = null;
            db.Close();
        }

          
    }

    public void Insert(SampleInfo sampleInfo)
    {
        db.Open();
        string query =
        @"

INSERT INTO [dbo].[Sample]
([SampleNo]
,[SampleText]
,[SampleTextarea]
,[SampleRadioButton]
,[Email]
,[Relationship]
,[Asset]
,[Liability]
,[SampleDate]
,[CreateUser]
,[UpdateDate]
,[UpdateUser]
,[SampleTime])
VALUES
(@SampleNo
,@SampleText
,@SampleTextarea
,@SampleRadioButton
,@Email
,@Relationship
,@Asset
,@Liability
,@SampleDate
,@CreateUser
,@UpdateDate
,@UpdateUser
,@SampleTime)

";

        db.Execute(query, sampleInfo);
         
        db.Close();
    }

    //public void Delete(List<string> clientNoList)
    //{
    //    db.Open();
    //    transaction = db.BeginTransaction();
    //    try
    //    {
    //        String query = "Delete from Sample where SampleNo = @SampleNo ";
    //        foreach (string SampleNo in clientNoList)
    //        {
    //            db.Execute(query, new { SampleNo = SampleNo }, transaction);
    //        }

    //        transaction.Commit();
    //    }
    //    catch
    //    {
    //        transaction.Rollback();
    //        throw;
    //    }
    //    finally
    //    {
    //        transaction.Dispose();
    //        transaction = null;
    //        db.Close();
    //    }
    //}

    //#region client staff

    //public bool StaffIsExisted(string SampleNo, string staffID)
    //{ 
    //    String query = "select count(*) from ClientStaff where SampleNo=@SampleNo and ID=@ID";
    //    var obj = (List<int>)db.Query<int>(query, new { SampleNo = SampleNo, ID = staffID }, transaction); 
    //    return obj[0] > 0;
    //}

    //public void UpdateStaff(StaffInfo staffInfo)
    //{
    //   // db.Open();
    //    string query =
    //        "UPDATE [dbo].[ClientStaff] "
    //        + "SET  "  
    //        + " [Name] = @Name "
    //        + ",[Type] = @Type "
    //        + ",[UpdateDate] = @UpdateDate "
    //        + ",[UpdateUser] = @UpdateUser "
    //        + "WHERE SampleNo = @SampleNo and [ID] = @ID  ";

    //    db.Execute(query, staffInfo, transaction);
    //    //db.Close();
    //}

    //public void InsertStaff(StaffInfo staffInfo)
    //{
    //   // db.Open();
    //    string query = "INSERT INTO [dbo].[ClientStaff] "
    //                + "([SampleNo] "
    //                + ",[ID] "
    //                + ",[Name] "
    //                + ",[Type] "
    //                + ",[CreateDate] "
    //                + ",[CreateUser] ) "
    //                + "VALUES "
    //                + "(@SampleNo "
    //                + ",@ID "
    //                + ",@Name "
    //                + ",@Type "
    //                + ",@CreateDate "
    //                + ",@CreateUser ) ";

    //    db.Execute(query, staffInfo, transaction);

    //   // db.Close();
    //}

    ////delete the staff not in the list
    //public void DeleteStaffNotInTheList(List<string> staffNoList, string parentNo)
    //{

    //    String query = "Delete from ClientStaff where SampleNo = @SampleNo and ID not in @Ids ";
    //    db.Execute(query, new { SampleNo = parentNo, Ids = staffNoList }, transaction);

    //}

    //public List<StaffInfo> getStaffList(string SampleNo)
    //{
    //    db.Open();
    //    String query = "select * from ClientStaff where SampleNo = @SampleNo ";
    //    List<StaffInfo> list = (List<StaffInfo>)db.Query<StaffInfo>(query, new { SampleNo = SampleNo });
    //    db.Close();
    //    return list; 
    //}

    //#endregion

}