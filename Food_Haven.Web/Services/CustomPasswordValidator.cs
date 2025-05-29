using Microsoft.AspNetCore.Identity;

namespace Food_Haven.Web.Services
{
    public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var errors = new List<IdentityError>();

            if (password.Length > 64)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordTooLong",
                    Description = "Mật khẩu không được vượt quá 64 ký tự."
                });
            }

            int categories = 0;
            if (password.Any(char.IsLower)) categories++;
            if (password.Any(char.IsUpper)) categories++;
            if (password.Any(char.IsDigit)) categories++;
            if (password.Any(ch => !char.IsLetterOrDigit(ch))) categories++;

            if (categories < 2)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordComplexity",
                    Description = "Mật khẩu phải chứa ít nhất 2 trong 4 loại: chữ hoa, chữ thường, số, ký tự đặc biệt."
                });
            }

            return Task.FromResult(errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success);
        }
    }

}
