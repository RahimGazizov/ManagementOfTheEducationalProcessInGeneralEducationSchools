using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
namespace InformationSystemOfASchoolIducationalPortal.Service
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

                var backupPath = Path.Combine(folder,
                    $"backup_{DateTime.Now:yyyy-MM-dd}.db");

                await _context.SaveChangesAsync();

                var connection = (SqliteConnection)_context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                await using var backup = new SqliteConnection($"Data Source={backupPath};Pooling=False");
                await backup.OpenAsync();

                connection.BackupDatabase(backup);

                await backup.CloseAsync();

                await _actionService.LogAsync(
                    "Создание резервной копии",
                    "Резервная копия",
                    backupPath,
                    $"Backup создан: {backupPath}"
                );

                return OperationResult.Ok("Резервная копия создана");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<OperationResult> RestoreBackup(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return OperationResult.Fail("Название файла пуст");

            var backupPath = Path.Combine("Backups", fileName);

            if (!File.Exists(backupPath))
                return OperationResult.Fail("Файл не найден");

            try
            {
                _systemStateService.IsMaintenanceMode = true;

                await _context.Database.CloseConnectionAsync();
                _context.ChangeTracker.Clear();

                await Task.Delay(1000); // даём завершиться запросам

                File.Copy(backupPath, _dbPath, true);

                SqliteConnection.ClearAllPools();

                _systemStateService.IsMaintenanceMode = false;

                return OperationResult.Ok("База восстановлена");
            }
            catch (Exception ex)
            {
                _systemStateService.IsMaintenanceMode = false;
                return OperationResult.Fail("Ошибка восстановление базы данных");
            }
        }
    }
}
