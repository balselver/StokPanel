using Microsoft.AspNetCore.Identity;

namespace StokPanel.Infrastructure.Identity
{
    internal sealed class TurkishIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email) => Error(nameof(DuplicateEmail), $"{email} adresi zaten kayıtlı");
        public override IdentityError DuplicateUserName(string userName) => Error(nameof(DuplicateUserName), $"{userName} adresi zaten kayıtlı");
        public override IdentityError InvalidEmail(string? email) => Error(nameof(InvalidEmail), "Geçerli bir e-posta adresi girin");
        public override IdentityError InvalidUserName(string? userName) => Error(nameof(InvalidUserName), "Geçerli bir e-posta adresi girin");
        public override IdentityError PasswordTooShort(int length) => Error(nameof(PasswordTooShort), $"Şifre en az {length} karakter olmalı");
        public override IdentityError PasswordRequiresDigit() => Error(nameof(PasswordRequiresDigit), "Şifrede en az bir rakam olmalı");
        public override IdentityError PasswordRequiresUpper() => Error(nameof(PasswordRequiresUpper), "Şifrede en az bir büyük harf olmalı");
        public override IdentityError PasswordRequiresLower() => Error(nameof(PasswordRequiresLower), "Şifrede en az bir küçük harf olmalı");

        private static IdentityError Error(string code, string description) => new() { Code = code, Description = description };
    }
}