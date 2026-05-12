using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Identity;

namespace InformationSystemOfASchoolIducationalPortal.Service
{
    public class AuthorizService
    {
        private readonly UserManager<Users> _users;
        private readonly SignInManager<Users> _signInManager;
        private readonly ActionLogService _actionLogService;
        public AuthorizService(SignInManager<Users> signInManager, ActionLogService actionLogService, UserManager<Users> users)
        {
            _signInManager = signInManager;
            _actionLogService = actionLogService;
            _users = users;
        }
        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string ActionName { get; set; }
            public string ControllerName { get; set; }
            public static OperationResult Ok(string actionName, string controllerName) => new OperationResult { Success = true, ActionName = actionName, ControllerName = controllerName };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
        }
        public async Task<OperationResult> Authoriz(Users user, string password)
        {
            try
            {
                if (user == null)
                    return OperationResult.Fail("Пользователь с таким логином не существует");
                if (password == null)
                    return OperationResult.Fail("Вы не вели пароль");
                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, false);
                if (result.Succeeded)
                {
                    await _actionLogService.LogAsync(
                        "Вход пользователя",
                        "User",
                        user.Id,
                        $"Пользователь вошел в систему."
                        );
                    if (!user.IsCredentialsChanged)
                        return OperationResult.Ok("CredentialsChange", "UpdateCredential");
                    if (await _users.IsInRoleAsync(user, "Админ"))
                        return OperationResult.Ok("Index", "AdminPersonalAccount");
                    if (await _users.IsInRoleAsync(user, "Учитель"))
                        return OperationResult.Ok("Index", "TeacherPerAcc");
                    if (await _users.IsInRoleAsync(user, "Ученик"))
                        return OperationResult.Ok("Index", "StudentPerAcc");
                    if (await _users.IsInRoleAsync(user, "Родитель"))
                        return OperationResult.Ok("Index", "ParentPerAccount");
                    if (await _users.IsInRoleAsync(user, "АдминистрацияШколы"))
                        return OperationResult.Ok("Index", "AdministrationSchoolPerAcc");
                    return OperationResult.Ok("Index", "Authoriz");
                }
                else
                    return OperationResult.Fail("Не верный логин или пароль"); ;

            }
            catch (Exception ex)
            {
                return OperationResult.Fail("Ошибка авторзации");
            }
        }
    }
}
