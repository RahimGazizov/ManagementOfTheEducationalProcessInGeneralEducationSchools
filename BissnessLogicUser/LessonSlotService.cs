using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class LessonSlotService
    {
        private readonly AppDbContext _context;
        private readonly ActionLogService _actionLogService;
        public LessonSlotService(AppDbContext context, ActionLogService actionLogService)
        {
            _context = context;
            _actionLogService = actionLogService;
        }
        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public static OperationResult Ok() => new OperationResult { Success = true };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
        }
        public async Task<OperationResult> AddLessonSlot(LessonSlot fm)
        {
            var exists = await _context.LessonSlots.AnyAsync(l => l.LessonNumber == fm.LessonNumber &&
           l.StartTime == fm.StartTime && l.EndTime == fm.EndTime);
            if (exists)
                return OperationResult.Fail("Такая запись уже существует!");
            if (fm.StartTime > fm.EndTime)
                return OperationResult.Fail("Время начало урока должно быть меньше времени конца урока");
            if ((fm.EndTime - fm.StartTime).TotalMinutes != 45)
                return OperationResult.Fail("Урок должен длиться 45 минут");
            _context.LessonSlots.Add(fm);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
                "Добаление времени урока",
                "Время уроков",
                fm.Id,
                $"Добавлено время урока: Номер урока: {fm.LessonNumber}. Время начало. {fm.StartTime.ToString(@"hh\:mm")} " +
                $"Время конца: {fm.EndTime.ToString(@"hh\:mm")}"
                );
            return OperationResult.Ok();
        }
        public async Task<OperationResult> EditLessonSlot(LessonSlot fm)
        {
            var exists = await _context.LessonSlots.AnyAsync(l => l.LessonNumber == fm.LessonNumber &&
l.StartTime == fm.StartTime && l.EndTime == fm.EndTime && l.Id != fm.Id);
            if (exists)
                return OperationResult.Fail("Такая запись уже существует!");
            if (fm.StartTime > fm.EndTime)
                return OperationResult.Fail("Время начало урока должна быть меньше конца урока");
            if ((fm.EndTime - fm.StartTime).TotalMinutes != 45)
                return OperationResult.Fail("Урок должен длиться 45 минут");
            var lessonSlot = await _context.LessonSlots.FirstOrDefaultAsync(s => s.Id == fm.Id);
            if (lessonSlot == null)
                return OperationResult.Fail("Не найдена запись, редоктирование невозможно");
            var oldLesson = new LessonSlot
            {
                LessonNumber = lessonSlot.LessonNumber,
                StartTime = lessonSlot.StartTime,
                EndTime = lessonSlot.EndTime,
            };
            lessonSlot.LessonNumber = fm.LessonNumber;
            lessonSlot.StartTime = fm.StartTime;
            lessonSlot.EndTime = fm.EndTime;
            _context.LessonSlots.Update(lessonSlot);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
               "Редактирование времени урока",
               "Время уроков",
               fm.Id,
               $"Редактировано время урока: Номер урока: {oldLesson.LessonNumber} → {fm.LessonNumber}. Время начало. {oldLesson.StartTime.ToString(@"hh\:mm")} → {fm.StartTime.ToString(@"hh\:mm")} " +
               $"Время конца: {oldLesson.EndTime.ToString(@"hh\:mm")} → {fm.EndTime.ToString(@"hh\:mm")}"
               );
            return OperationResult.Ok();
        }
        public async Task<OperationResult> Delete(string id)
        {
            if (id == null)
                return OperationResult.Fail("Айди обекта пуст удаление невозможно");
            var exists = await _context.LessonSlots.FirstOrDefaultAsync(x => x.Id == id);
            if (exists == null)
                return OperationResult.Fail("Обьект не найден удаление не возможно");
            _context.LessonSlots.Remove(exists);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
               "Удаление времени урока",
               "Время уроков",
              id,
               $"Удалено время урока: Номер урока: {exists.LessonNumber}. Время начало. {exists.StartTime.ToString(@"hh\:mm")} " +
               $"Время конца: {exists.EndTime.ToString(@"hh\:mm")}"
               );
            return OperationResult.Ok();
        }

    }
}

