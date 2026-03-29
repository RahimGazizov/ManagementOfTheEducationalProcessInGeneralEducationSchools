using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class AcademicYearLogic
    {
        private readonly AppDbContext _context;
     
        public AcademicYearLogic(AppDbContext context)
        {
            _context = context;
        }
        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }

            public static OperationResult Ok() => new OperationResult { Success = true };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
        }
        public async Task<OperationResult> AddAcademicYear(AcademicYear fm)
        {
            var exists = await _context.AcademicYear.AnyAsync(a => a.Name == fm.Name);
            if (exists)
                return OperationResult.Fail($"Название учебного года {fm.Name} уже существует");
            
            if (fm.StartDateYear > fm.EndDateYear)
                return OperationResult.Fail($"Дата начало должна быть меньше даты конца");
            
            string[] date = fm.Name.Split("-");
            if (date[0].Length < 4 || date[1].Length < 4)
                return OperationResult.Fail($"Введите дату в 4-х значном формате");
            
            if (Convert.ToDouble(date[0]) > Convert.ToDouble(date[1]))
                return OperationResult.Fail($"Название даты начало {date[0]} не должно быть больше чем название даты конца {date[1]}");
            
            _context.AcademicYear.Add(fm);
            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }
        public async Task<OperationResult> EditAcademicYear(AcademicYear fm)
        {
            var exists = await _context.AcademicYear.AnyAsync(a => a.Name == fm.Name && a.Id != fm.Id);
            if (exists)
                return OperationResult.Fail($"Название учебного года {fm.Name} уже существует");

            if (fm.StartDateYear > fm.EndDateYear)
                return OperationResult.Fail($"Дата начало должна быть меньше даты конца");

            string[] date = fm.Name.Split("-");
            if (date[0].Length < 4 || date[1].Length < 4)
                return OperationResult.Fail($"Введите дату в 4-х значном формате");

            if (Convert.ToDouble(date[0]) > Convert.ToDouble(date[1]))
                return OperationResult.Fail($"Название даты начало {date[0]} не должно быть больше чем название даты конца {date[1]}");

            _context.AcademicYear.Update(fm);
            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }
    }
}
