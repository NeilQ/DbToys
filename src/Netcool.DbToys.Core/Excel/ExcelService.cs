using ClosedXML.Excel;
using Table = Netcool.DbToys.Core.Database.Table;

namespace Netcool.DbToys.Core.Excel;

public class ExcelService : IExcelService
{
    public static readonly int SheetNameMaxLength = 31;
    public static List<string> Headers = new()
    {
        "#",
        "Name",
        "Description",
        "AutoIncrement",
        "PrimaryKey",
        "Type",
        "Length",
        "Nullable",
        "DefaultValue"
    };

    public void GenerateDatabaseDictionary(IList<Table> tableList, string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);
        var styleOptions = ExcelStyleOptions.Default;
        var wb = new XLWorkbook(XLEventTracking.Disabled);

        if (tableList.Count > 0)
        {
            var indexWorksheet = wb.Worksheets.Add("IndexSheet");
            indexWorksheet.ShowGridLines = false;
            indexWorksheet.Style.Font.FontName = styleOptions.FontFamily;
            indexWorksheet.RowHeight = 20;
            for (var k = 0; k < tableList.Count; k++)
            {
                var tableSheetName = tableList[k].Name.Length <= SheetNameMaxLength ? tableList[k].Name : tableList[k].Name[..25] + "..." + k;

                var tableWorksheet = wb.Worksheets.Add(tableSheetName);
                tableWorksheet.ShowGridLines = false;
                tableWorksheet.Style.Font.FontName = styleOptions.FontFamily;
                tableWorksheet.RowHeight = 30;

                indexWorksheet.Cell(k + 1, 1).Value = tableList[k].DisplayName;
                indexWorksheet.Cell(k + 1, 1).SetHyperlink(new XLHyperlink($"'{tableSheetName}'!A1", tableList[k].DisplayName));

                // title
                tableWorksheet.Row(1).Height = 30;
                var titleCell = tableWorksheet.Cell(1, 1);
                titleCell.Value = tableList[k].DisplayName;
                titleCell.Style.Font.FontColor = styleOptions.TitleFontColor;
                titleCell.Style.Font.FontSize = styleOptions.TitleFontSize;
                titleCell.Style.Font.Bold = true;
                titleCell.Style.Alignment.Horizontal = styleOptions.TitleHorizontalAlignment;
                titleCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                titleCell.Style.Fill.BackgroundColor = styleOptions.TitleBackgroundColor;
                tableWorksheet.Range(1, 1, 1, Headers.Count).Merge();

                // header
                for (var i = 0; i < Headers.Count; i++)
                {
                    var headerCell = tableWorksheet.Cell(2, i + 1);
                    headerCell.Value = Headers[i];
                    headerCell.Style.Font.FontSize = styleOptions.HeaderFontSize;
                    headerCell.Style.Fill.BackgroundColor = styleOptions.HeaderBackgroundColor;
                    headerCell.Style.Alignment.Horizontal = styleOptions.HeaderHorizontalAlignment;
                    headerCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    headerCell.Style.Font.FontColor = styleOptions.HeaderFontColor;
                }

                // column data 
                for (var i = 0; i < tableList[k].Columns.Count; i++)
                {
                    var column = tableList[k].Columns[i];
                    var rowNo = i + 3;
                    var row = tableWorksheet.Row(rowNo);
                    row.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    row.Style.Font.FontSize = styleOptions.ValueFontSize;
                    tableWorksheet.Cell(rowNo, 1).SetValue(i + 1);
                    tableWorksheet.Cell(rowNo, 2).SetValue(column.Name);
                    tableWorksheet.Cell(rowNo, 3).SetValue(column.Description);
                    tableWorksheet.Cell(rowNo, 4).SetValue(column.IsAutoIncrement ? "√" : "");
                    tableWorksheet.Cell(rowNo, 5).SetValue(column.IsPk ? "√" : "");
                    tableWorksheet.Cell(rowNo, 6).SetValue(column.DbType);
                    tableWorksheet.Cell(rowNo, 7).SetValue(column.Length);
                    tableWorksheet.Cell(rowNo, 8).SetValue(column.IsNullable ? "√" : "");
                    tableWorksheet.Cell(rowNo, 9).SetValue(column.DefaultValue);
                    for (var index = 0; index < Headers.Count; index++)
                    {
                        var cell = tableWorksheet.Cell(rowNo, index + 1);
                        if (rowNo % 2 == 0)
                        {
                            cell.Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        }
                    }

                    tableWorksheet.ColumnsUsed().AdjustToContents();
                }

                indexWorksheet.ColumnsUsed().AdjustToContents();
            }
        }
        wb.SaveAs(fileName);
        wb.Dispose();
    }
}