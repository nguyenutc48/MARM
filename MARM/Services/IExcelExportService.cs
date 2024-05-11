namespace MARM.Services
{
    public interface IExcelExportService
    {
        Task<byte[]> ExportToExcelAsync(List<BoatUnitMissionExportResult> data);
    }
}
