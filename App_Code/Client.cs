using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient; 
using System.Web;
using Dapper;


public class Client
{
	#region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    SqlTransaction transaction;

    public List<GeneralCodeDesc> GetClientCodeList(string ClientCode)
    {
        this.db.Open();
        String query = "select top 10 ClientCode Code, ClientCode [Desc] from Client where (@ClientCode = '' or ClientCode like '%' + @ClientCode + '%') order by ClientCode";
        var obj = (List<GeneralCodeDesc>)this.db.Query<GeneralCodeDesc>(query, new { ClientCode = ClientCode });
        this.db.Close();
        return obj;
    }

    public List<GeneralCodeDesc> GetBUList(string ClientCode, string BU)
    {
        this.db.Open();
        String query = @"select top 10 RowNo Code, BU + ' - ' + [Location] [Desc] from ClientBU 

where ClientCode = @ClientCode and (@BU = '' or BU like '%' + @BU + '%') order by BU";
        var obj = (List<GeneralCodeDesc>)this.db.Query<GeneralCodeDesc>(query, new { ClientCode = ClientCode, BU = BU });
        this.db.Close();
        return obj;
    }


    public bool IsExisted(ClientInfo info)
    {
        //this.db.Open();
        String query = "select count(*)  from Client " 
		+ " where ClientCode = @ClientCode ";
        var obj = (List<int>)this.db.Query<int>(query, info, this.transaction);
       // this.db.Close();
        return obj[0] > 0;
    }

    public void Save(ClientInfo info)
    {
        this.db.Open();
        this.transaction = this.db.BeginTransaction();
        try {

            if (this.IsExisted(info))
                this.Update(info);
            else
                this.Insert(info);

            ClientBU clientBU = new ClientBU(this.db, this.transaction);
            List<int> rowNoList = new List<int>();
            foreach (var bu in info.BUList)
            {
                bu.ClientCode = info.ClientCode;
                clientBU.Save(bu);
                rowNoList.Add(bu.RowNo);
            }
            clientBU.DeleteNotIn(info.ClientCode, rowNoList);

            this.transaction.Commit();
        }
        catch {
            this.transaction.Rollback();
            throw;

        }
        finally {
            this.db.Close();
        }
     
    }

	 
    public ClientInfo Get(string ClientCode)
    {
		this.db.Open();

        string query = "select * from Client " 
		+ " where ClientCode = @ClientCode ";
		
        var obj = (List<ClientInfo>)this.db.Query<ClientInfo>(query, new {  ClientCode = ClientCode  });
        this.db.Close();

		
        if (obj.Count > 0) {
            var result = obj[0];
            result.BUList = new ClientBU().Get(ClientCode);
            return obj[0];
        }
        else
            return null;
    }

    public void Delete(string ClientCode)
    {
		this.db.Open();

        string query = "delete  from Client " 
		+ " where ClientCode = @ClientCode ";
		
        this.db.Execute(query, new {  ClientCode = ClientCode  }, this.transaction);
        this.db.Close();
    }
	
    public void Update(ClientInfo info)
    {
        //this.db.Open();

        string query = " UPDATE [dbo].[Client] SET  "
		+ " [ClientName] = @ClientName " 
		+ ", [Address] = @Address " 
		+ ", [Phone] = @Phone " 
		+ ", [Fax] = @Fax " 
		+ ", [ContactPerson] = @ContactPerson " 
		+ ", [ContactPhone] = @ContactPhone " 
		+ ", [CreateUser] = @CreateUser " 
		+ ", [CreateDate] = @CreateDate " 
		+ ", [LastModifyUser] = @LastModifyUser " 
		+ ", [LastModifyDate] = @LastModifyDate " 
		+ " where ClientCode = @ClientCode ";

         
        this.db.Execute(query, info, this.transaction);
        //this.db.Close();
    }

    public void Insert(ClientInfo info)
    {
        //this.db.Open();

        string query = "INSERT INTO [dbo].[Client] ( [ClientCode] " 
		+ ",[ClientName] " 
		+ ",[Address] " 
		+ ",[Phone] " 
		+ ",[Fax] " 
		+ ",[ContactPerson] " 
		+ ",[ContactPhone] " 
		+ ",[CreateUser] " 
		+ ",[CreateDate] " 
		+ ",[LastModifyUser] " 
		+ ",[LastModifyDate] " 
		+") "
		+ "VALUES ( @ClientCode "
		+ ",@ClientName " 
		+ ",@Address " 
		+ ",@Phone " 
		+ ",@Fax " 
		+ ",@ContactPerson " 
		+ ",@ContactPhone " 
		+ ",@CreateUser " 
		+ ",@CreateDate " 
		+ ",@LastModifyUser " 
		+ ",@LastModifyDate " 
		+") ";


        this.db.Execute(query, info, this.transaction);
        //this.db.Close();
    }
	#endregion 

}