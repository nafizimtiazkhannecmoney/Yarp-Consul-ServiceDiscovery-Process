using Microsoft.Reporting.NETCore;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Text;

namespace SaGoAMLReporting.Helper
{
    public static class ReportHelper
    {
        public static byte[] GetReportToBytes(string ReportPath, DataTable ReportData, string FileType = "Excel", string ContentType = "application/vnd.ms-excel")
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = ReportPath;


            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "ReportDB";
            reportDataSource.Value = ReportData;
            localReport.DataSources.Add(reportDataSource);


            string reportType = FileType;
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType            
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx            
            string deviceInfo = "<DeviceInfo>" +
                "  <OutputFormat>" + FileType + "</OutputFormat>" +
                "  <PageWidth>16.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>1in</MarginLeft>" +
                "  <MarginRight>1in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            return renderedBytes;
            //return File(renderedBytes, ContentType, string.Format("PoleUpdateInformation.{0}", fileNameExtension));
        }
        public static List<T> ConvertDatatableToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                    }
                }
                return objT;
            }).ToList();
        }
        public static DataTable ListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        public static DataTable GetDataTableFromList(IEnumerable<object> data, DataTable tbl)
        {

            foreach (var itm in data)
            {
                if (itm != null)
                {
                    var tType = itm.GetType();
                    var tpro = tType.GetProperties();

                    var dr = tbl.NewRow();
                    foreach (DataColumn col in tbl.Columns)
                    {
                        var cname = col.ColumnName;

                        var tp = tpro.Where(p => p.Name == cname).FirstOrDefault();
                        if (tp != null)
                        {
                            var value = tp.GetValue(itm);
                            if (value != null)
                            {
                                dr[cname] = value;
                            }
                        }
                    }
                    tbl.Rows.Add(dr);
                }
            }

            return tbl;

        }
        public static DataTable GetDataTableFromList(object data, DataTable tbl)
        {

            //  foreach (var itm in data)
            //   {
            var tType = data.GetType();
            var tpro = tType.GetProperties();

            var dr = tbl.NewRow();
            foreach (DataColumn col in tbl.Columns)
            {
                var cname = col.ColumnName;

                var tp = tpro.Where(p => p.Name == cname).FirstOrDefault();
                if (tp != null)
                {
                    var value = tp.GetValue(data);
                    if (value != null)
                    {
                        dr[cname] = value;
                    }
                }

            }
            tbl.Rows.Add(dr);
            // }

            return tbl;

        }
        public static async Task<DataTable> GetDataTableFromListAsync(object data, DataTable tbl)
        {
            return await Task.Run(() =>
            {
                var tType = data.GetType();
                var tpro = tType.GetProperties();

                var dr = tbl.NewRow();
                foreach (DataColumn col in tbl.Columns)
                {
                    var cname = col.ColumnName;

                    var tp = tpro.Where(p => p.Name == cname).FirstOrDefault();
                    if (tp != null)
                    {
                        var value = tp.GetValue(data);
                        if (value != null)
                        {
                            dr[cname] = value;
                        }
                    }
                }
                tbl.Rows.Add(dr);

                return tbl;
            });
        }
        
        public static string ConvertToBase64(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(bytes);

        }

    }
}
