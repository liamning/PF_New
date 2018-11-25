using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class ClientBU
{
    #region Standar Function
    SqlConnection db;
    SqlTransaction transaction;

    public ClientBU()
    {
        this.db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    }
    public ClientBU(SqlConnection db, SqlTransaction transaction)
    {
        this.db = db;
        this.transaction = transaction;
    }

    public List<string> GetClientCodeList(string ClientCode)
    { 
        db.Open();
        String query = "select top 10 ClientCode from ClientBU where (@ClientCode = '' or ClientCode like '%' + @ClientCode + '%') order by ClientCode";
        var obj = (List<string>)db.Query<string>(query, new { ClientCode = ClientCode });
        db.Close();
        return obj;
    }


    public bool IsExisted(ClientBUInfo info)
    {
        //db.Open();
        String query = "select count(*)  from ClientBU " 
		+ " where ClientCode = @ClientCode and RowNo = @RowNo";
        var obj = (List<int>)db.Query<int>(query, info, this.transaction);
        //db.Close();
        return obj[0] > 0;
    }

    public void Save(ClientBUInfo info)
    {
        if(this.IsExisted(info))
            this.Update(info);
        else
            this.Insert(info); 
    }

	 
    public List<ClientBUInfo> Get(string ClientCode)
    {
		db.Open();

        string query = "select * from ClientBU " 
		+ " where ClientCode = @ClientCode ";
		
        var obj = (List<ClientBUInfo>)db.Query<ClientBUInfo>(query, new {  ClientCode = ClientCode  });
        db.Close();

        return obj;
    }

    public void DeleteNotIn(string ClientCode, List<int> rowNoList)
    {
		//db.Open();

        string query = "delete  from ClientBU " 
		+ " where ClientCode = @ClientCode and RowNo not in @RowNoList ";
		
        db.Execute(query, new {  ClientCode = ClientCode , RowNoList = rowNoList }, this.transaction);
        //db.Close();
    }
	
    public void Update(ClientBUInfo info)
    {
        //db.Open();

        string query = " UPDATE [dbo].[ClientBU] SET  "
		+ " [RowNo] = @RowNo " 
		+ ", [BU] = @BU " 
		+ ", [Location] = @Location " 
		+ " where ClientCode = @ClientCode and RowNo = @RowNo";

         
        db.Execute(query, info, this.transaction);
        //db.Close();
    }

    public void Insert(ClientBUInfo info)
    {
        //db.Open();

        string query = "INSERT INTO [dbo].[ClientBU] ( [ClientCode] " 
		+ ",[RowNo] " 
		+ ",[BU] " 
		+ ",[Location] " 
		+") "
		+ "VALUES ( @ClientCode "
		+ ",@RowNo " 
		+ ",@BU " 
		+ ",@Location " 
		+") ";


        db.Execute(query, info, this.transaction);
        //db.Close();
    }
	#endregion 

}