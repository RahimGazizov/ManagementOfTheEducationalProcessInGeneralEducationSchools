using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class BackupsService
    {
        private readonly string _dbPath = "school.db";
        private readonly ActionLogService _actionService;
        private readonly SystemStateService _systemStateService;
        private readonly AppDbContext _context;
        public BackupsService(ActionLogService actionService, SystemStateService systemStateService,
            AppDbContext context)
        {
            _actionService = actionService;
            _systemStateService = systemStateService;
            _context = context;
        }
        public class OperationResult
        {
            public string Message { get; set; }
            public bool Success { get; set; }
            public static OperationResult Ok(string? message) => new OperationResult { Success = true, Message = message };
            public static OperationResult Fail(string? message) => new OperationResult { Success = false, Message = message };
        }
        public async Task<OperationResult> CreateBackup()
        {
            try
            {
                var folder = "Backups";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var bachupsPath = Path.Combine(folder,
                    $"backups_{DateTime.Now:yyyy-MM-dd}.db");

                using (var source = new FileStream(_dbPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var dest = new FileStream(bachupsPath, FileMode.Create))
                {
                    await source.CopyToAsync(dest);
                }
                await _actionService.LogAsync(
                    "Создание резервной копии бд",
                    null,
                    null,
                    $"Создана резервная копия: {bachupsPath}"
                    );
                return OperationResult.Ok("Резервное копирование создано");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
        public async Task<OperationResult> RestoreBackup(string fileName)
        {
            if (fileName == null)
                return OperationResult.Fail("Название файла пуст");
            var backupPath = Path.Combine("Backups", fileName);
            if (!File.Exists(backupPath))
                return OperationResult.Fail("Файл не найден");
            try
            {
                _systemStateService.IsMaintenanceMode = true;

                await _context.Database.CloseConnectionAsync();

                File.Copy(backupPath, _dbPath, true);

                _systemStateService.IsMaintenanceMode = false;

                return OperationResult.Ok("База восстановлена");
            }
            catch (Exception ex)
            {
                _systemStateService.IsMaintenanceMode = false;
                return OperationResult.Fail(ex.Message);
            }
        }
    }
}
