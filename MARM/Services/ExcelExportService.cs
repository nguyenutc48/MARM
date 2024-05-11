using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace MARM.Services
{ 
    public class ExcelExportService : IExcelExportService
    {
        public async Task<byte[]> ExportToExcelAsync(List<BoatUnitMissionExportResult> data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                    sheets.Append(sheet);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Header cụ thể
                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    headerRow.Append(
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("STT") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Đơn Vị") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Thời gian") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Số vết đạn") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Vị trí trúng") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue("Ghi chú") }
     
                        // Thêm các header cụ thể khác tại đây
                    );
                    sheetData.AppendChild(headerRow);

                    // Dữ liệu từ danh sách đối tượng
                    foreach (var item in data)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row dataRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        dataRow.Append(
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(item.Index.ToString()) },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(item.BoatName) },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(item.ShotTime.ToString()) },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(item.ShotTotal) },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(item.ShotPosition.ToString()) },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(item.Note) }
                            // Thêm các dữ liệu cụ thể khác tại đây
                        );
                        sheetData.AppendChild(dataRow);
                    }
                }

                return stream.ToArray();
            }
        }
    }
}
