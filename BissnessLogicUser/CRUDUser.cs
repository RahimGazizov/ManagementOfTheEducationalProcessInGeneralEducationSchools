using Microsoft.AspNetCore.Identity;
using InformationSystemOfASchoolIducationalPortal.Models;
namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class CRUDUser
    {
        private readonly UserManager<Users> _users;
        public CRUDUser(UserManager<Users> users)
        {
            _users = users;
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
                };
                var result = await _users.CreateAsync(user, createUser.Password);
                if (!result.Succeeded)
                    return Errors(result);
                var addRole = await _users.AddToRoleAsync(user, createUser.Role);
                if (!addRole.Succeeded)
                    return Errors(addRole);
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
        public async Task<OperationResult> Delete(string login)
        {
            try
            {
                var user = await _users.FindByNameAsync(login);
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
        public async Task<OperationResult> Edit(CreateUser user)
        {
            try
            {
                var findUser = await _users.FindByNameAsync(user.Login);
                if (findUser == null)
                    return OperationResult.Fail("Пользователь не найден");
                var roles = await _users.GetRolesAsync(findUser);
                var removeRoles = await _users.RemoveFromRolesAsync(findUser, roles);
                if (!removeRoles.Succeeded)
                    return Errors(removeRoles);
                var aadRole = await _users.AddToRoleAsync(findUser, user.Role);
                if (!aadRole.Succeeded)
                    return Errors(aadRole);
                findUser.FullName = user.FullName;
                findUser.UserName = user.Login;
                findUser.BirthDate = user.BirthDate;
                var result = await _users.UpdateAsync(findUser);
                if (!result.Succeeded)
                    return Errors(result);
                return OperationResult.Ok();
            }
            catch(Exception ex)
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
            catch(Exception ex)
            {
                return OperationResult.Fail(ex.Message);
            }
        }
        private OperationResult Errors(IdentityResult result) =>
                 OperationResult.Fail(string.Join(", ", result.Errors.Select(r => r.Description)));


    }
}
