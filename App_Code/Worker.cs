using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Dapper;


public class Worker
{
    #region Standar Function
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
    SqlTransaction transaction;

    public List<GeneralCodeDesc> GetWorkerIDList(string WorkerID)
    {
        db.Open();
        String query = "select top 10 WorkerID Code, WorkerID [Desc] from Worker where (@WorkerID = '' or WorkerID like '%' + @WorkerID + '%') order by WorkerID";
        var obj = (List<GeneralCodeDesc>)db.Query<GeneralCodeDesc>(query, new { WorkerID = WorkerID });
        db.Close();
        return obj;
    }

    public List<GeneralCodeDesc> GetWorkerIDListByHKIDName(string input, string clientCode, int BU)
    {
        db.Open();
        String query = @"

select top 10 WorkerID Code, EnglishName + ' (' + HKID1 + HKID2 + HKID3 + ')'  [Desc] 
from Worker 
where exists (select 1 from WorkerClientList 
where Worker.WorkerID = WorkerClientList.WorkerID and WorkerClientList.ClientCode = @ClientCode and WorkerClientList.BU = @BU)
and ((HKID1 + HKID2 + HKID3) like '%' + @Input + '%' or EnglishName like '%' + @Input + '%')
order by WorkerID

";
        var obj = (List<GeneralCodeDesc>)db.Query<GeneralCodeDesc>(query, new { Input = input, ClientCode = clientCode, BU = BU });
        db.Close();
        return obj;
    }

    public bool IsExisted(WorkerInfo info)
    {
        //db.Open();
        String query = "select count(*)  from Worker "
        + " where WorkerID = @WorkerID ";
        var obj = (List<int>)db.Query<int>(query, info, this.transaction);
        //db.Close();
        return obj[0] > 0;
    }

    public string IsStaffNoDuplicated(WorkerClientListInfo info)
    {
        //db.Open();
        String query = @"

select top 1 * from WorkerClientList
where ClientCode =  @ClientCode 
and StaffNo = @StaffNo
and (@EffectiveFrom <= EffectiveTo and @EffectiveTo >= EffectiveFrom)
and WorkerID != @WorkerID

";
        var obj = (List<WorkerClientListInfo>)db.Query<WorkerClientListInfo>(query, info, this.transaction);
        //db.Close();
        return obj.Count > 0 ? obj[0].WorkerID : "";
    }

    public string Save(WorkerInfo info)
    {
        this.db.Open();
        this.transaction = this.db.BeginTransaction();
        try
        {
            if (this.IsExisted(info))
                this.Update(info);
            else
                this.Insert(info);

            WorkerClientList clientListObj = new WorkerClientList(this.db, this.transaction);
            clientListObj.Save(info.ClientList, info.WorkerID);

            int rowno = 0;
            WorkerSkill WorkerSkill = new WorkerSkill(this.db, this.transaction); 
            if (info.SkillList != null)
                foreach (WorkerSkillInfo skill in info.SkillList)
                {
                    skill.WorkerID = info.WorkerID;
                    skill.RowNo = ++rowno;
                    WorkerSkill.Save(skill);

                }
            WorkerSkill.Delete(info.WorkerID, ++rowno);


            WorkerAdjustment WorkerAdjustment = new WorkerAdjustment(this.db, this.transaction);
            rowno = 0;
            if (info.AdjustmentList != null)
                foreach (WorkerAdjustmentInfo adjust in info.AdjustmentList)
                {
                    adjust.WorkerID = info.WorkerID;
                    adjust.RowNo = ++rowno;
                    WorkerAdjustment.Save(adjust);

                }
            WorkerAdjustment.Delete(info.WorkerID, ++rowno);



            WorkerAttachment WorkerAttachment = new WorkerAttachment(this.db, this.transaction);
            rowno = 0;
            List<int> existingRowNo = new List<int>();
            if (info.AttachmentList != null)
                foreach (WorkerAttachmentInfo adjust in info.AttachmentList)
                {
                    existingRowNo.Add(adjust.RowNo);

                    adjust.WorkerID = info.WorkerID; 
                    WorkerAttachment.Save(adjust);

                }
            WorkerAttachment.Delete(info.WorkerID, existingRowNo);


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

        return info.WorkerID;

    }

    public WorkerInfo Get(string WorkerID)
    {
        db.Open();
        WorkerClientList clientListObj = new WorkerClientList();
        WorkerSkill WorkerSkill = new WorkerSkill();
        WorkerAdjustment WorkerAdjustment = new WorkerAdjustment();
        WorkerAttachment WorkerAttachment = new WorkerAttachment();

        string query = "select * from Worker "
        + " where WorkerID = @WorkerID ";

        try
        {
            var obj = (List<WorkerInfo>)db.Query<WorkerInfo>(query, new { WorkerID = WorkerID });
            WorkerInfo info;
            if (obj.Count > 0)
            {
                info = obj[0];
                info.ClientList = clientListObj.Get(WorkerID);
                info.SkillList = WorkerSkill.GetSkillList(WorkerID);
                info.AdjustmentList = WorkerAdjustment.GetList(WorkerID);
                info.AttachmentList = WorkerAttachment.GetList(WorkerID);
                return info;
            }
            else
                return null;
        }
        finally
        {
            db.Close();
        }
    }

    public WorkerInfo GetWorkerByHKID(string HKID, string name)
    {
        db.Open();

        string query = "select * from Worker "
        + " where HKID = @HKID ";

        try
        {
            var result = (List<WorkerInfo>)this.db.Query<WorkerInfo>(query, new { HKID = HKID });

            if (result.Count > 0)
                return result[0];
            else
                return null;
        }
        finally
        {
            db.Close();
        }
    }

    public WorkerInfo CheckWorkerWithBU(string clientCode, int BU, string workerID)
    {

        db.Open();

        string query = @"

select *
from Worker
where WorkerID = @WorkerID 
and exists (select * from WorkerClientList
where ClientCode = @ClientCode and BU=@BU and WorkerID =  @WorkerID )";

        try
        {
            var result = (List<WorkerInfo>)this.db.Query<WorkerInfo>(query, new { WorkerID = workerID, BU = BU, ClientCode = clientCode });
            if (result.Count > 0)
            {
                return result[0];
            }
            else
                return null;
        }
        finally
        {
            db.Close();
        }

    }

    public void Delete(string WorkerID)
    {
        db.Open();

        string query = "delete  from Worker "
        + " where WorkerID = @WorkerID ";

        db.Execute(query, new { WorkerID = WorkerID }, this.transaction);
        db.Close();
    }

    public void Update(WorkerInfo info)
    {
        //db.Open();

        string query = " UPDATE [dbo].[Worker] SET  "
        + " [ChineseName] = @ChineseName "
        + ", [EnglishName] = @EnglishName "
        + ", [Introducer] = @Introducer "
        + ", [Address] = @Address "
        + ", [HKID1] = @HKID1 "
        + ", [HKID2] = @HKID2 "
        + ", [HKID3] = @HKID3 "
        + ", [DOB] = @DOB "
        + ", [Gender] = @Gender "
        + ", [PhoneNo] = @PhoneNo "
        + ", [PayrollGroup] = @PayrollGroup "
        + ", [PayrollRemarks] = @PayrollRemarks "
        + ", [BankCode] = @BankCode "
        + ", [BankACNo] = @BankACNo "
        + ", [BankName] = @BankName "
        + ", [MPFOption] = @MPFOption "
        + ", [DateJoin] = @DateJoin "
        + ", [WorkArea] = @WorkArea "
        + ", [Remarks] = @Remarks "

        + ", [BeneficialName] = @BeneficialName "
        + ", [District] = @District "
        + ", [PositionGrade] = @PositionGrade "
        + ", [WorkerStatus] = @WorkerStatus "
        + ", [Position] = @Position "
        + ", [AppraisalGrade] = @AppraisalGrade "
        + ", [PayrollMethod] = @PayrollMethod "
        + ", [CardStatus] = @CardStatus "
        + ", [ReturnDate] = @ReturnDate "

        //+ ", [CreateUser] = @CreateUser "
            //+ ", [CreateDate] = @CreateDate "
        + ", [LastModifyUser] = @LastModifyUser "
        + ", [LastModifyDate] = @LastModifyDate "
        + " where WorkerID = @WorkerID ";


        db.Execute(query, info, this.transaction);
        //db.Close();
    }

    public void Insert(WorkerInfo info)
    {
        //db.Open(); 
        var MaxWorkerList = this.db.Query("select max(WorkerID) MaxWorkerID from Worker", null, this.transaction);


        foreach (var id in MaxWorkerList)
        {
            info.WorkerID = (Convert.ToInt32(id.MaxWorkerID) + 1).ToString();
        }
         

        string query = "INSERT INTO [dbo].[Worker] ( [WorkerID] "
        + ",[ChineseName] "
        + ",[EnglishName] "
        + ",[Introducer] "
        + ",[Address] "
        + ",[HKID1] "
        + ",[HKID2] "
        + ",[HKID3] "
        + ",[DOB] "
        + ",[Gender] "
        + ",[PhoneNo] "
        + ",[PayrollGroup] "
        + ",[PayrollRemarks] "
        + ",[BankCode] "
        + ",[BankACNo] "
        + ",[BankName] "
        + ",[MPFOption] "
        + ",[DateJoin] "
        + ",[WorkArea] "
        + ",[Remarks] "


        + ",[BeneficialName] "
        + ",[District] "
        + ",[PositionGrade] "
        + ",[WorkerStatus] "
        + ",[Position] "
        + ",[AppraisalGrade] "
        + ",[PayrollMethod] "
        + ",[CardStatus] "
        + ",[ReturnDate] "

        + ",[CreateUser] "
        + ",[CreateDate] "
            //+ ",[LastModifyUser] "
            //+ ",[LastModifyDate] "
        + ") "
        + "VALUES ( @WorkerID "
        + ",@ChineseName "
        + ",@EnglishName "
        + ",@Introducer "
        + ",@Address "
        + ",@HKID1 "
        + ",@HKID2 "
        + ",@HKID3 "
        + ",@DOB "
        + ",@Gender "
        + ",@PhoneNo "
        + ",@PayrollGroup "
        + ",@PayrollRemarks "
        + ",@BankCode "
        + ",@BankACNo "
        + ",@BankName "
        + ",@MPFOption "
        + ",@DateJoin "
        + ",@WorkArea "
        + ",@Remarks "


        + ",@BeneficialName "
        + ",@District "
        + ",@PositionGrade "
        + ",@WorkerStatus "
        + ",@Position "
        + ",@AppraisalGrade "
        + ",@PayrollMethod "
        + ",@CardStatus "
        + ",@ReturnDate "


        + ",@CreateUser "
        + ",@CreateDate "
            //+ ",@LastModifyUser "
            //+ ",@LastModifyDate "
        + ") ";


        db.Execute(query, info, this.transaction);
        //db.Close();
    }

    public bool BeneficialNameExists(string name, string workerID)
    {


        db.Open();
        String query = @"
select count(*) from Worker
where BeneficialName = @BeneficialName
and WorkerID != @WorkerID
            ";
        var obj = (List<int>)db.Query<int>(query, new { BeneficialName = name, WorkerID = workerID });

        db.Close();
        return obj[0] > 0;
    }
    #endregion

}