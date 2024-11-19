using ClosedXML.Excel;

namespace Lab9.Services.Implementations;

public class VersionedService : IVersionedService
{
    public Task<int> GetIntegerAsync()
    {
        return Task.FromResult(Random.Shared.Next());
    }


    public Task<string> GetTextAsync()
    {
        return Task.FromResult("Hello, this is version 2.0 response!");
    }


    public Task<byte[]> GenerateExcelAsync()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sheet1");
        worksheet.Cell(1, 1).Value = "Column 1";
        worksheet.Cell(1, 2).Value = "Column 2";

        worksheet.Cell(2, 1).Value = "Value 1";
        worksheet.Cell(2, 2).Value = "Value 2";

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }
}
