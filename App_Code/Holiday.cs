using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class Holiday
{
    #region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    SqlTransaction transaction;

    public List<string> GetHolidayDateList(string HolidayDate)
    { 
        db.Open();
        String query = "select top 10 HolidayDate from Holiday where (@HolidayDate = '' or HolidayDate like '%' + @HolidayDate + '%') order by HolidayDate";
        var obj = (List<string>)db.Query<string>(query, new { HolidayDate = HolidayDate });
        db.Close();
        return obj;
    }


    public bool IsExisted(HolidayInfo info)
    { 
        String query = "select count(*)  from Holiday " 
		+ " where HolidayDate = @HolidayDate ";
        var obj = (List<int>)db.Query<int>(query, info, this.transaction); 
        return obj[0] > 0;
    }

    public void Save(List<HolidayInfo> list, int year)
    {
        this.db.Open();
        this.transaction = this.db.BeginTransaction();
        try
        {
            List<DateTime> dateList = new List<DateTime>();
            foreach (var info in list)
            {

                if (this.IsExisted(info))
                    this.Update(info);
                else
                    this.Insert(info);

                dateList.Add(info.HolidayDate);
            }
            this.DeleteNotIn(year, dateList);


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


    public void DeleteNotIn(int year, List<DateTime> dateList)
    {
        string query = "delete  from Holiday "
        + " where Year(HolidayDate) = @Year and HolidayDate not in @DateList ";

        db.Execute(query, new { Year = year, DateList = dateList }, this.transaction);
    }



    public HolidayInfo Get(DateTime HolidayDate)
    {
		db.Open();

        string query = "select * from Holiday " 
		+ " where HolidayDate = @HolidayDate ";
		
        var obj = (List<HolidayInfo>)db.Query<HolidayInfo>(query, new {  HolidayDate = HolidayDate  });
        db.Close();
		
        if (obj.Count > 0)
            return obj[0];
        else
            return null;
    }

    public void Delete(DateTime HolidayDate)
    {
		db.Open();

        string query = "delete  from Holiday " 
		+ " where HolidayDate = @HolidayDate ";
		
        db.Execute(query, new {  HolidayDate = HolidayDate  });
        db.Close();
    }
	
    public void Update(HolidayInfo info)
    { 

        string query = " UPDATE [dbo].[Holiday] SET  "
		+ " [EngDesc] = @EngDesc " 
		+ ", [ChiDesc] = @ChiDesc " 
		+ ", [LastModifyDate] = @LastModifyDate " 
		+ ", [LastModifyUser] = @LastModifyUser " 
		+ ", [CreateDate] = @CreateDate " 
		+ ", [CreateUser] = @CreateUser " 
		+ " where HolidayDate = @HolidayDate ";

         
        db.Execute(query, info, this.transaction); 
    }

    public void Insert(HolidayInfo info)
    { 
        string query = "INSERT INTO [dbo].[Holiday] ( [HolidayDate] " 
		+ ",[EngDesc] " 
		+ ",[ChiDesc] " 
		+ ",[LastModifyDate] " 
		+ ",[LastModifyUser] " 
		+ ",[CreateDate] " 
		+ ",[CreateUser] " 
		+") "
		+ "VALUES ( @HolidayDate "
		+ ",@EngDesc " 
		+ ",@ChiDesc " 
		+ ",@LastModifyDate " 
		+ ",@LastModifyUser " 
		+ ",@CreateDate " 
		+ ",@CreateUser " 
		+") ";


        db.Execute(query, info, this.transaction); 
    }

    public List<HolidayInfo> GetHoliday(int year)
    {
        this.db.Open();

        try
        {
            string query = @" 
SELECT TOP (1000) [HolidayDate]
      ,[EngDesc]
      ,[ChiDesc]
      ,[LastModifyDate]
      ,[LastModifyUser]
      ,[CreateDate]
      ,[CreateUser]
  FROM [dbo].[Holiday]
where Year(HolidayDate) = @Year
                ";
            List<HolidayInfo> result = (List<HolidayInfo>)this.db.Query<HolidayInfo>(query, new { Year = year });
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

    #endregion

}