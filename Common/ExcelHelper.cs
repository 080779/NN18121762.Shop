using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SDMS.Common
{
    public class ExcelHelper
    {
        #region 导出excel
        public static NpoiMemoryStream ExportExcel<T>(T[] lists, string sheetName)
        {
            IWorkbook wb = new XSSFWorkbook();

            ICellStyle style1 = wb.CreateCellStyle();//样式
            IFont font1 = wb.CreateFont();//字体
            font1.FontName = "楷体";
            font1.Boldweight = (short)FontBoldWeight.Normal;//字体加粗样式
            style1.SetFont(font1);//样式里的字体设置具体的字体样式
            style1.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//文字水平对齐方式
            style1.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//文字垂直对齐方式
            //设置边框
            style1.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style1.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style1.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style1.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

            ISheet sheet = wb.CreateSheet(sheetName);
            IRow row;
            ICell cell;

            Type t1 = typeof(T);
            PropertyInfo[] props = t1.GetProperties();

            row = sheet.CreateRow(0);
            for (int j = 0; j < props.Count(); j++)
            {
                cell = row.CreateCell(j);//创建第j列
                cell.CellStyle = style1;
                ExcelHelper.SetCellValue(cell, props[j].CustomAttributes.First().ToString().Split('"')[1]);
                sheet.AutoSizeColumn(0);
            }
            int rowIndex = 1;
            foreach (var list in lists)
            {
                row = sheet.CreateRow(rowIndex);//创建第j列
                for (int i = 0; i < props.Count(); i++)
                {
                    cell = row.CreateCell(i);
                    cell.CellStyle = style1;
                    ExcelHelper.SetCellValue(cell,props[i].GetValue(list)==null?"": props[i].GetValue(list));
                    sheet.AutoSizeColumn(i);
                }
                rowIndex++;
            }

            var ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            wb.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            return ms;
        }
        #endregion

        public class x2003
        {
            #region Excel2003
            /// <summary>
            /// 将Excel文件中的数据读出到DataTable中(xls)
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            public static DataTable ExcelToTableForXLS(string file)
            {
                DataTable dt = new DataTable();
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook(fs);
                    ISheet sheet = hssfworkbook.GetSheetAt(0);

                    //表头
                    IRow row = sheet.GetRow(sheet.FirstRowNum);
                    List<int> columns = new List<int>();
                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        object obj = GetValueTypeForXLS(row.GetCell(i) as HSSFCell);
                        if (obj == null || obj.ToString() == string.Empty)
                        {
                            dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                            //continue;
                        }
                        else
                            dt.Columns.Add(new DataColumn(obj.ToString()));
                        columns.Add(i);
                    }
                    //IRow row = sheet.GetRow(sheet.FirstRowNum + 1);
                    //数据
                    for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row != null)
                        {
                            DataRow dr = dt.NewRow();
                            bool hasValue = false;
                            foreach (int j in columns)
                            {
                                if (row.GetCell(j) == null)
                                {
                                    hasValue = true;
                                }
                                else
                                {
                                    dr[j] = GetValueTypeForXLS(row.GetCell(j) as HSSFCell);
                                    if (dr[j] != null && dr[j].ToString() != string.Empty)
                                    {
                                        hasValue = true;
                                    }
                                }
                            }
                            if (hasValue)
                            {
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }
                return dt;
            }

            /// <summary>
            /// 将DataTable数据导出到Excel文件中(xls)
            /// </summary>
            /// <param name="dt"></param>
            /// <param name="file"></param>
            public static void TableToExcelForXLS(DataTable dt, string file)
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ISheet sheet = hssfworkbook.CreateSheet("Test");

                //表头
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(dt.Columns[i].ColumnName);
                }

                //数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row1 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = row1.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][j].ToString());
                    }
                }

                //转为字节数组
                MemoryStream stream = new MemoryStream();
                hssfworkbook.Write(stream);
                var buf = stream.ToArray();

                //保存为Excel文件
                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                }
            }

            /// <summary>
            /// 获取单元格类型(xls)
            /// </summary>
            /// <param name="cell"></param>
            /// <returns></returns>
            private static object GetValueTypeForXLS(HSSFCell cell)
            {
                if (cell == null)
                    return null;
                switch (cell.CellType)
                {
                    case CellType.Blank: //BLANK:
                        return null;
                    case CellType.Boolean: //BOOLEAN:
                        return cell.BooleanCellValue;
                    case CellType.Numeric: //NUMERIC:
                        return cell.NumericCellValue;
                    case CellType.String: //STRING:
                        return cell.StringCellValue;
                    case CellType.Error: //ERROR:
                        return cell.ErrorCellValue;
                    case CellType.Formula: //FORMULA:
                    default:
                        return "=" + cell.CellFormula;
                }
            }
            #endregion
        }

        public class x2007
        {
            #region Excel2007
            /// <summary>
            /// 将Excel文件中的数据读出到DataTable中(xlsx)
            /// </summary>
            /// <param name="file"></param>
            /// <returns></returns>
            public static DataTable ExcelToTableForXLSX(string file)
            {
                DataTable dt = new DataTable();
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook(fs);
                    ISheet sheet = xssfworkbook.GetSheetAt(0);

                    //表头
                    IRow row = sheet.GetRow(sheet.FirstRowNum);
                    List<int> columns = new List<int>();
                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        object obj = GetValueTypeForXLSX(row.GetCell(i) as XSSFCell);
                        if (obj == null || obj.ToString() == string.Empty)
                        {
                            dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                            //continue;
                        }
                        else
                            dt.Columns.Add(new DataColumn(obj.ToString()));
                        columns.Add(i);
                    }
                    //数据
                    for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);
                        if (row != null)
                        {
                            DataRow dr = dt.NewRow();
                            bool hasValue = false;
                            foreach (int j in columns)
                            {
                                dr[j] = GetValueTypeForXLSX(row.GetCell(j) as XSSFCell);
                                if (dr[j] != null && dr[j].ToString() != string.Empty)
                                {
                                    hasValue = true;
                                }
                            }
                            if (hasValue)
                            {
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }
                return dt;
            }

            /// <summary>
            /// 将DataTable数据导出到Excel文件中(xlsx)
            /// </summary>
            /// <param name="dt"></param>
            /// <param name="file"></param>
            public static void TableToExcelForXLSX(DataTable dt, string file)
            {
                XSSFWorkbook xssfworkbook = new XSSFWorkbook();
                ISheet sheet = xssfworkbook.CreateSheet("Test");

                //表头
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(dt.Columns[i].ColumnName);
                }

                //数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row1 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = row1.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][j].ToString());
                    }
                }

                //转为字节数组
                MemoryStream stream = new MemoryStream();
                xssfworkbook.Write(stream);
                var buf = stream.ToArray();

                //保存为Excel文件
                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                }
            }

            /// <summary>
            /// 获取单元格类型(xlsx)
            /// </summary>
            /// <param name="cell"></param>
            /// <returns></returns>
            private static object GetValueTypeForXLSX(XSSFCell cell)
            {
                if (cell == null)
                    return null;
                switch (cell.CellType)
                {
                    case CellType.Blank: //BLANK:
                        return null;
                    case CellType.Boolean: //BOOLEAN:
                        return cell.BooleanCellValue;
                    case CellType.Numeric: //NUMERIC:
                        return cell.NumericCellValue;
                    case CellType.String: //STRING:
                        return cell.StringCellValue;
                    case CellType.Error: //ERROR:
                        return cell.ErrorCellValue;
                    case CellType.Formula: //FORMULA:
                    default:
                        return "=" + cell.CellFormula;
                }
            }
            #endregion
        }

        //读取Excel
        public static DataTable GetDataTable(string filepath)
        {
            var dt = new DataTable("xls");
            if (filepath.Last() == 's')
            {
                //Office 2003及以前的版本
                dt = x2003.ExcelToTableForXLS(filepath);
            }
            else
            {   //Office 2007及以上的版本
                dt = x2007.ExcelToTableForXLSX(filepath);
            }
            return dt;
        }

        //根据数据类型设置不同类型的cell
        public static void SetCellValue(ICell cell, object obj)
        {
            if (obj.GetType() == typeof(int))
            {
                cell.SetCellValue((int)obj);
            }
            else if (obj.GetType() == typeof(double))
            {
                cell.SetCellValue((double)obj);
            }
            else if (obj.GetType() == typeof(IRichTextString))
            {
                cell.SetCellValue((IRichTextString)obj);
            }
            else if (obj.GetType() == typeof(string))
            {
                cell.SetCellValue(obj.ToString());
            }
            else if (obj.GetType() == typeof(DateTime))
            {
                cell.SetCellValue((DateTime)obj);
            }
            else if (obj.GetType() == typeof(bool))
            {
                cell.SetCellValue((bool)obj);
            }
            else
            {
                cell.SetCellValue(obj.ToString());
            }
        }
    }
}
