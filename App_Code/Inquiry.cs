﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using System.Reflection;

/// <summary>
/// Summary description for Inquiry
/// </summary>
public class Inquiry
{
    SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);

    public Inquiry()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string View_Search(JArray criterias, string[] fields, string view)
    {
        string result = "";
        List<string> conditions = new List<string>();
        //foreach (JObject obj in criterias)
        //{
        //}

        string[] fieldType;
        foreach (JObject content in criterias.Children<JObject>())
        {
            foreach (JProperty prop in content.Properties())
            {
                fieldType = prop.Name.Split('.');
                switch (fieldType[1])
                {
                    case "Period":
                        DateTime start = DateTime.ParseExact("01/" + prop.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        conditions.Add(string.Format("({0} >= '{1}' and {0} < '{2}')", fieldType[0], start.ToString("yyyy-MM-dd"), start.AddMonths(1).ToString("yyyy-MM-dd")));
                        break;
                    case "StartDate":
                        DateTime startDate = DateTime.ParseExact(prop.Value.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        conditions.Add(string.Format("{0} >= '{1}'", fieldType[0], startDate.ToString("yyyy-MM-dd")));
                        break;
                    case "EndDate":
                        DateTime endDate = DateTime.ParseExact(prop.Value.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        conditions.Add(string.Format("{0} <= '{1}'", fieldType[0], endDate.ToString("yyyy-MM-dd")));
                        break;
                    case "Like":
                        conditions.Add(string.Format("{0} like '%{1}%'", fieldType[0], prop.Value));
                        break;
                    default:
                        conditions.Add(string.Format("{0} = '{1}'", fieldType[0], prop.Value));
                        break;


                }
            }
        }


        string query = string.Format(@"select distinct {1} from {0}", view, fields != null ? string.Join(", ", fields) : "*");
        if (conditions.Count > 0)
        {
            query += " where " + string.Join(" and ", conditions);
        }

        db.Open();

        var datas = db.Query(query);
        db.Close();
        Newtonsoft.Json.Converters.IsoDateTimeConverter IsoDateTimeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = GlobalSetting.DateFormat };
        result = Newtonsoft.Json.JsonConvert.SerializeObject(datas, IsoDateTimeConverter);

        return result;
    }



    public MemoryStream Export_Search(JArray criterias, string[] headers1, string[] fields1, string view1, 
        string[] headers2 = null, string[] fields2= null, string view2= null
        )
    {
        string result = "";
        List<string> conditions = new List<string>();
        //foreach (JObject obj in criterias)
        //{
        //}

        string[] fieldType;
        foreach (JObject content in criterias.Children<JObject>())
        {
            foreach (JProperty prop in content.Properties())
            {
                fieldType = prop.Name.Split('.');
                switch (fieldType[1])
                {
                    case "Period":
                        DateTime start = DateTime.ParseExact("01/" + prop.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        conditions.Add(string.Format("({0} >= '{1}' and {0} < '{2}')", fieldType[0], start.ToString("yyyy-MM-dd"), start.AddMonths(1).ToString("yyyy-MM-dd")));
                        break;
                    case "StartDate":
                        DateTime startDate = DateTime.ParseExact(prop.Value.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        conditions.Add(string.Format("{0} >= '{1}'", fieldType[0], startDate.ToString("yyyy-MM-dd")));
                        break;
                    case "EndDate":
                        DateTime endDate = DateTime.ParseExact(prop.Value.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        conditions.Add(string.Format("{0} <= '{1}'", fieldType[0], endDate.ToString("yyyy-MM-dd")));
                        break;
                    case "StartString":
                        conditions.Add(string.Format("{0} >= '{1}'", fieldType[0], prop.Value));
                        break;
                    case "EndString":
                        conditions.Add(string.Format("{0} <= '{1}'", fieldType[0], prop.Value));
                        break;
                    case "Like":
                        conditions.Add(string.Format("{0} like '%{1}%'", fieldType[0], prop.Value));
                        break;
                    default:
                        conditions.Add(string.Format("{0} = '{1}'", fieldType[0], prop.Value));
                        break;


                }
            }
        }


        // dll refered NPOI.dll and NPOI.OOXML
        IWorkbook workbook = new HSSFWorkbook();
        int viewCount = -1;
        string[] headers, fields;
        foreach (var view in new string[] { view1, view2 })
        {
            if (string.IsNullOrEmpty(view)) continue;

            viewCount++;
            headers = new List<string[]> { headers1, headers2 }[viewCount];
            fields = new List<string[]> { fields1, fields2 }[viewCount];



            string query = string.Format(@"select distinct {1} from {0}", view, fields != null ? string.Join(", ", fields) : "*");
            if (conditions.Count > 0)
            {
                query += " where " + string.Join(" and ", conditions);
            }

            db.Open();

            var datas = db.Query<dynamic>(query);
            db.Close();


            ISheet sheet1 = workbook.CreateSheet(view);
            //make a header row
            IRow row1 = sheet1.CreateRow(0);
            int j = 0; ICell cell;
            int row = 0;
            IDataFormat dataFormatCustom = workbook.CreateDataFormat();

            ICellStyle cellStyle = workbook.CreateCellStyle();
            ICreationHelper createHelper = workbook.GetCreationHelper();
            cellStyle.DataFormat = createHelper.CreateDataFormat().GetFormat("dd MMM yyyy");


            //cell.CellStyle = styles["cell"];

            //string[] headers = new string[] { "Worker ID", "Client", "Client Staff No", "Attendance Date", "Total Hours", "TimeIn", "Time Out", "Hour Rate", "Amount", };
            //string[] headers = new string[] { "Worker ID", "Payroll Group Desc", "Client", "Client Staff No", "Chinese Name", "Attendance Date", "Hours", "OT Hours", "Time In", "Time Out", "Hour Rate", "OT Hour Rate", "Amount", "Bank Name", "Beneficial Name", "Bank AC No", };
            row1 = sheet1.CreateRow(row++);
            for (int i = 0; i < headers.Length; i++)
            {
                cell = row1.CreateCell(i);
                cell.SetCellValue(headers[i]);
                sheet1.AutoSizeColumn(i);
            }

            foreach (var data in datas)
            {
                j = 0;

                row1 = sheet1.CreateRow(row++);
                foreach (KeyValuePair<string, dynamic> kvp in data)
                {
                    //Console.WriteLine("{0} = {1}", kvp.Key, kvp.Value);

                    cell = row1.CreateCell(j++);
                    if (kvp.Value != null)
                    {
                        if (kvp.Value is decimal)
                        {
                            cell.SetCellValue((double)kvp.Value);
                        }
                        else if (kvp.Value is DateTime)
                        {
                            cell.CellStyle = cellStyle;
                            cell.SetCellValue(kvp.Value);
                        }
                        else
                        {
                            cell.SetCellValue(kvp.Value);
                        }

                    }
                }

            }

        }
         
        using (var exportData = new MemoryStream())
        {
            workbook.Write(exportData);
            return exportData;
        }
    }
}