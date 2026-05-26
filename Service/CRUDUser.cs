using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace InformationSystemOfASchoolIducationalPortal.Service
{
    public class CRUDUser
    {
        private readonly UserManager<Users> _users;
        private readonly AppDbContext _context;
        private readonly ActionLogService _actionLogService;
        public CRUDUser(UserManager<Users> users, AppDbContext context, ActionLogService actionLogService)
        {
            _users = users;
            _context = context;
            _actionLogService = actionLogService;
        }
        public class OperationResult
        {
            public bool Succeeded { get; set; }
            public string Message { get; set; }
            public static OperationResult Ok() => new OperationResult { Succeeded = true };
            public static OperationResult Fail(string message) => new OperationResult { Succeeded = false, Message = message };
        }
        public async Task<OperationResult> AddUser(CreateUser createUser)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (createUser == null)
                    return OperationResult.Fail("Данные пользователя отсутствуют");

                if (string.IsNullOrWhiteSpace(createUser.Role))
                    return OperationResult.Fail("Роль пользователя не указана");

                var dayToday = DateTime.Now;

                Class? studentClass = null;
                AcademicYear? currentYear = null;
                Term? currentTerm = null;

                // Если создаём ученика — заранее проверяем класс, учебный год и четверть
                if (createUser.Role == "Ученик")
                {
                    studentClass = await _context.Classes
                        .Include(c => c.Students)
                        .FirstOrDefaultAsync(c => c.Id == createUser.ClassId);

                    if (studentClass == null)
                        return OperationResult.Fail("Класс не найден");

                    if (studentClass.Students.Count >= 2)
                        return OperationResult.Fail($"Класс {studentClass.NumClass}-{studentClass.LetterClass} полный. Больше нельзя добавлять");

                    currentYear = await _context.AcademicYear
                        .FirstOrDefaultAsync(d => d.StartDateYear <= dayToday && dayToday <= d.EndDateYear);

                    if (currentYear == null)
                        return OperationResult.Fail("Не найден текущий учебный год");

                    currentTerm = await _context.Term
                        .FirstOrDefaultAsync(d => d.DateStartTerm <= dayToday && dayToday <= d.DateEndTerm);

                    if (currentTerm == null)
                        return OperationResult.Fail("Не найдена текущая четверть");
                }

                var user = new Users
                {
                    FullName = createUser.FullName,
                    UserName = createUser.Login,
                    BirthDate = createUser.BirthDate,
                    PhoneNumber = createUser.PhoneNumber,
                };

                var result = await _users.CreateAsync(user, createUser.Password);

                if (!result.Succeeded)
                    return Errors(result);

                var addRole = await _users.AddToRoleAsync(user, createUser.Role);

                if (!addRole.Succeeded)
                    return Errors(addRole);

                if (createUser.Role == "Ученик")
                {
                    var student = new Students
                    {
                        UserId = user.Id,
                        ClassId = createUser.ClassId
                    };

                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    var studentHistory = new StudentsHistory
                    {
                        StudentId = student.Id,
                        ClassId = createUser.ClassId,
                        AcademicYearId = currentYear!.Id,
                        TermId = currentTerm!.Id
                    };

                    _context.StudentsHistory.Add(studentHistory);
                    await _context.SaveChangesAsync();
                }
                else if (createUser.Role == "Учитель")
                {
                    var teacher = new Teachers
                    {
                        UserId = user.Id
                    };

                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();
                }
                else if (createUser.Role == "Админ")
                {
                    var admin = new Admins
                    {
                        UserId = user.Id
                    };

                    _context.Admins.Add(admin);
                    await _context.SaveChangesAsync();
                }
                else if (createUser.Role == "Родитель")
                {
                    var parent = new Parents
                    {
                        UserId = user.Id
                    };

                    _context.Parent.Add(parent);
                    await _context.SaveChangesAsync();
                }
                else if (createUser.Role == "АдминистрацияШколы")
                {
                    var schoolAdmin = new SchoolAdministrations
                    {
                        UserId = user.Id
                    };

                    _context.Administrations.Add(schoolAdmin);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return OperationResult.Fail("Неизвестная роль пользователя");
                }

                await _actionLogService.LogAsync(
                    action: "Создание пользователя",
                    entityName: "Пользователи",
                    entityId: user.Id,
                    details: $"Создан пользователь: {user.FullName}. Логин: {user.UserName}. Назначена роль: {createUser.Role}."
                );

                await transaction.CommitAsync();

                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return OperationResult.Fail(ex.Message);
            }
        }
        public async Task<OperationResult> Delete(string id)
        {
            try
            {
                var user = await _users.FindByIdAsync(id);

                if (user == null)
                    return OperationResult.Fail("Пользователь не найден");

                // Сохраняем данные ДО удаления
                var userId = user.Id;
                var fullName = user.FullName;
                var userName = user.UserName;

                var roles = await _users.GetRolesAsync(user);
                var roleText = roles.Any()
                    ? string.Join(", ", roles)
                    : "Роль не указана";

                var result = await _users.DeleteAsync(user);

                if (!result.Succeeded)
                    return Errors(result);

                await _actionLogService.LogAsync(
                    action: "Удаление пользователя",
                    entityName: "Пользователи",
                    entityId: userId,
                    details: $"Удалён пользователь: {fullName}. Логин: {userName}. Роль: {roleText}."
                );

                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
        public async Task<OperationResult> Edit(EditUserDTO dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (dto == null)
                    return OperationResult.Fail("Данные пользователя не переданы");

                var findUser = await _users.FindByIdAsync(dto.UserId);

                if (findUser == null)
                    return OperationResult.Fail("Пользователь не найден");

                // Сохраняем старые данные ДО изменения
                var oldFullName = findUser.FullName;
                var oldUserName = findUser.UserName;
                var oldBirthDate = findUser.BirthDate;
                var oldPhoneNumber = findUser.PhoneNumber;

                var oldRoles = await _users.GetRolesAsync(findUser);
                var oldRoleText = oldRoles.Any()
                    ? string.Join(", ", oldRoles)
                    : "Роль не указана";

                string? oldClassName = null;
                string? newClassName = null;
                string? oldEmail = null;
                Students? student = null;

                // Если редактируем ученика — заранее проверяем учебный год, четверть и ученика
                if (dto.Role == "Ученик")
                {
                    string academicId = await GetAcademic();

                    if (academicId == null)
                        return OperationResult.Fail("Невозможно редактировать ученика: не найден учебный год, проверьте даты");

                    string termId = await GetTerm();

                    if (termId == null)
                        return OperationResult.Fail("Невозможно редактировать ученика: не найдена четверть, проверьте даты");

                    student = await _context.Students
                        .Include(s => s.User)
                        .Include(s => s.Class)
                        .FirstOrDefaultAsync(s => s.UserId == findUser.Id);

                    if (student == null)
                        return OperationResult.Fail("Ученик не найден");
                    
                    oldClassName = student.ClassId != null ? $"{student.Class.NumClass}-{student.Class.LetterClass}": "-";

                    var newClass = await _context.Classes
                        .FirstOrDefaultAsync(c => c.Id == dto.StudentClassId);

                    if (newClass == null)
                        return OperationResult.Fail("Новый класс не найден");

                    newClassName = $"{newClass.NumClass}-{newClass.LetterClass}";

                    student.ClassId = dto.StudentClassId;

                    _context.Students.Update(student);

                    var exists = await _context.StudentsHistory
                        .AnyAsync(h =>
                            h.StudentId == student.Id &&
                            h.ClassId == dto.StudentClassId &&
                            h.AcademicYearId == academicId &&
                            h.TermId == termId);

                    if (!exists)
                    {
                        var studentHistory = new StudentsHistory
                        {
                            StudentId = student.Id,
                            ClassId = dto.StudentClassId,
                            AcademicYearId = academicId,
                            TermId = termId
                        };

                        _context.StudentsHistory.Add(studentHistory);
                    }
                }
                if (dto.Role == "Родитель")
                {
                    var parent = await _context.Parent.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
                    if (parent == null)
                        return OperationResult.Fail("Родитель не найден");
                    oldEmail = parent.Email;
                    parent.Email = dto.Email;
                    _context.Update(parent);
                }
                // Меняем роль
                if (oldRoles.Any())
                {
                    var removeRoles = await _users.RemoveFromRolesAsync(findUser, oldRoles);

                    if (!removeRoles.Succeeded)
                        return Errors(removeRoles);
                }

                var addRole = await _users.AddToRoleAsync(findUser, dto.Role);

                if (!addRole.Succeeded)
                    return Errors(addRole);

                // Меняем данные пользователя
                findUser.FullName = dto.FullName;
                findUser.UserName = dto.Login;
                findUser.BirthDate = dto.BirthDate;
                findUser.PhoneNumber = dto.PhoneNumber;

                var result = await _users.UpdateAsync(findUser);

                if (!result.Succeeded)
                    return Errors(result);

                await _context.SaveChangesAsync();

                var details = $"Изменён пользователь. " +
                              $"ФИО: {oldFullName} → {findUser.FullName}. " +
                              $"\nЛогин: {oldUserName} → {findUser.UserName}. " +
                              $"\nРоль: {oldRoleText} → {dto.Role}. " +
                              $"\nДата рождения: {oldBirthDate:dd.MM.yyyy} → {findUser.BirthDate:dd.MM.yyyy}. " +
                              $"\nТелефон: {oldPhoneNumber} → {findUser.PhoneNumber}.";

                if (dto.Role == "Ученик" && newClassName != null)
                {
                    details += $" Класс: {oldClassName} → {newClassName}.";
                }
                if (dto.Role == "Родитель" && dto.Email != null)
                {
                    details += $"Почта: {oldEmail} → {dto.Email}";
                }
                await _actionLogService.LogAsync(
                    action: "Редактирование пользователя",
                    entityName: "Пользователи",
                    entityId: findUser.Id,
                    details: details
                );

                await transaction.CommitAsync();

                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return OperationResult.Fail(ex.Message);
            }
        }
        public async Task<OperationResult> ResetPassword(string id, string newPassword)
        {
            try
            {
                var user = await _users.FindByIdAsync(id);
                if (user == null)
                    return OperationResult.Fail("Пользователь не найден");
                var roles = await _users.GetRolesAsync(user);

                var token = await _users.GeneratePasswordResetTokenAsync(user);
                var result = await _users.ResetPasswordAsync(user, token, newPassword);
                if (!result.Succeeded)
                    return Errors(result);
                await _actionLogService.LogAsync(
    action: "Сброс пароля пользователя",
    entityName: "Пользователи",
    entityId: user.Id,
    details: $"Администратор сбросил пароль пользователя: {user.FullName}. Логин: {user.UserName}. Роль: {roles.FirstOrDefault()}"
);
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
        private OperationResult Errors(IdentityResult result) =>
                 OperationResult.Fail(string.Join(", ", result.Errors.Select(r => r.Description)));
        private async Task<string?> GetAcademic()
        {
            return await _context.AcademicYear
                        .Where(d => d.StartDateYear <= DateTime.Now
                        && d.EndDateYear >= DateTime.Now)
                        .Select(s => s.Id).FirstOrDefaultAsync() ?? null;
        }
        private async Task<string?> GetTerm()
        {
            return await _context.Term
                        .Where(d => d.DateStartTerm <= DateTime.Now
                        && d.DateEndTerm >= DateTime.Now)
                        .Select(s => s.Id).FirstOrDefaultAsync() ?? null;
        }
    }
}
