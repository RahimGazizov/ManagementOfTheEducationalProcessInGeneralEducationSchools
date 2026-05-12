using Microsoft.AspNetCore.Identity;

public class RussianErrorDescriber : IdentityErrorDescriber
{
    // ---------------- PASSWORD ----------------

    public override IdentityError PasswordTooShort(int length)
        => new() { Code = nameof(PasswordTooShort), Description = $"Пароль должен быть минимум {length} символов" };

    public override IdentityError PasswordRequiresDigit()
        => new() { Code = nameof(PasswordRequiresDigit), Description = "Пароль должен содержать хотя бы одну цифру" };

    public override IdentityError PasswordRequiresUpper()
        => new() { Code = nameof(PasswordRequiresUpper), Description = "Пароль должен содержать заглавную букву" };

    public override IdentityError PasswordRequiresLower()
        => new() { Code = nameof(PasswordRequiresLower), Description = "Пароль должен содержать строчную букву" };

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Пароль должен содержать спецсимвол" };

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        => new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"Пароль должен содержать минимум {uniqueChars} уникальных символов" };

    public override IdentityError PasswordMismatch()
        => new() { Code = nameof(PasswordMismatch), Description = "Неверный текущий пароль" };

    // ---------------- USERNAME (LOGIN) ----------------

    public override IdentityError DuplicateUserName(string userName)
        => new() { Code = nameof(DuplicateUserName), Description = "Логин уже занят" };

    public override IdentityError InvalidUserName(string userName)
        => new() { Code = nameof(InvalidUserName), Description = "Логин содержит недопустимые символы" };

    public override IdentityError InvalidEmail(string email)
        => new() { Code = nameof(InvalidEmail), Description = "Некорректный email" };

    // ---------------- GENERAL ----------------

    public override IdentityError DefaultError()
        => new() { Code = nameof(DefaultError), Description = "Произошла ошибка" };

    public override IdentityError ConcurrencyFailure()
        => new() { Code = nameof(ConcurrencyFailure), Description = "Конфликт данных, попробуйте снова" };
}