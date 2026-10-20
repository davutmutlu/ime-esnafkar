using Microsoft.AspNetCore.Identity;

namespace EsnafKar.Data
{
    public class TurkceIdentityHatalari : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() =>
            new() { Code = nameof(DefaultError), Description = "Bilinmeyen bir hata oluştu." };

        public override IdentityError DuplicateUserName(string userName) =>
            new() { Code = nameof(DuplicateUserName), Description = "Bu e-posta adresi zaten kayıtlı." };

        public override IdentityError DuplicateEmail(string email) =>
            new() { Code = nameof(DuplicateEmail), Description = "Bu e-posta adresi zaten kayıtlı." };

        public override IdentityError InvalidUserName(string? userName) =>
            new() { Code = nameof(InvalidUserName), Description = "Geçersiz e-posta adresi." };

        public override IdentityError InvalidEmail(string? email) =>
            new() { Code = nameof(InvalidEmail), Description = "Geçersiz e-posta adresi." };

        public override IdentityError PasswordTooShort(int length) =>
            new() { Code = nameof(PasswordTooShort), Description = $"Şifre en az {length} karakter olmalıdır." };

        public override IdentityError PasswordRequiresDigit() =>
            new() { Code = nameof(PasswordRequiresDigit), Description = "Şifrede en az bir rakam (0-9) olmalıdır." };

        public override IdentityError PasswordRequiresLower() =>
            new() { Code = nameof(PasswordRequiresLower), Description = "Şifrede en az bir küçük harf olmalıdır." };

        public override IdentityError PasswordRequiresUpper() =>
            new() { Code = nameof(PasswordRequiresUpper), Description = "Şifrede en az bir büyük harf olmalıdır." };

        public override IdentityError PasswordRequiresNonAlphanumeric() =>
            new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Şifrede en az bir özel karakter (!, @, # gibi) olmalıdır." };

        public override IdentityError PasswordMismatch() =>
            new() { Code = nameof(PasswordMismatch), Description = "Şifre hatalı." };
    }
}