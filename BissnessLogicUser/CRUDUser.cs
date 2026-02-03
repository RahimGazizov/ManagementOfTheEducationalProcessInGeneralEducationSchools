using Microsoft.AspNetCore.Identity;
using InformationSystemOfASchoolIducationalPortal.Models;
using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.EntityFrameworkCore;
namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class CRUDUser
    {
        private readonly UserManager<Users> _users;
        private readonly AppDbContext _context;
        public CRUDUser(UserManager<Users> users, AppDbContext context)
        {
            _users = users;
            _context = context;
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
            try
            {
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
                }
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
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
                var result = await _users.DeleteAsync(user);
                if (!result.Succeeded)
                    return Errors(result);
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
        public async Task<OperationResult> Edit(EditUserDTO dto)
        {
            try
            {
                var findUser = await _users.FindByIdAsync(dto.UserId);
                if (findUser == null)
                    return OperationResult.Fail("Пользователь не найден");
                var roles = await _users.GetRolesAsync(findUser);
                var removeRoles = await _users.RemoveFromRolesAsync(findUser, roles);
                if (!removeRoles.Succeeded)
                    return Errors(removeRoles);
                var aadRole = await _users.AddToRoleAsync(findUser, dto.Role);
                if (!aadRole.Succeeded)
                    return Errors(aadRole);
                findUser.FullName = dto.FullName;
                findUser.UserName = dto.Login;
                findUser.BirthDate = dto.BirthDate;
                findUser.PhoneNumber = dto.PhoneNumber;
                if (dto.Role.Contains("Ученик"))
                {
                    var student = await _context.Students.FirstOrDefaultAsync(u => u.UserId == findUser.Id);
                    if (student != null)
                    {
                        student.ClassId = dto.StudentClassId;
                        _context.Students.Update(student);
                        await _context.SaveChangesAsync();
                    }
                    else
                        return OperationResult.Fail("Пользователь не найден");
                }
                var result = await _users.UpdateAsync(findUser);
                if (!result.Succeeded)
                    return Errors(result);
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
        public async Task<OperationResult> ResetPassword(string login, string newPassword)
        {
            try
            {
                var user = await _users.FindByNameAsync(login);
                if (user == null)
                    return OperationResult.Fail("Пользователь не найден");
                var token = await _users.GeneratePasswordResetTokenAsync(user);
                var result = await _users.ResetPasswordAsync(user, token, newPassword);
                if (!result.Succeeded)
                    return Errors(result);
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
        private OperationResult Errors(IdentityResult result) =>
                 OperationResult.Fail(string.Join(", ", result.Errors.Select(r => r.Description)));


    }
}
