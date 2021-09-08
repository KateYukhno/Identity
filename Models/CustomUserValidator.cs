using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AppIdentity.Models
{
    public class CustomUserValidator: IUserValidator<User>

    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new List<IdentityError>();
            string pattern = "^[a-w]+$";
            if (!Regex.IsMatch(user.Name, pattern, RegexOptions.IgnoreCase))
            {
                errors.Add(new IdentityError
                {
                    Description = "Имя пользователя должено состоять только из символов латинского алфавита"
                });
            }

            if (user.UserName.Length < 3)
            {
                errors.Add(new IdentityError
                {
                    Description = "Минимальная длина логина равна 3"
                });
            }
            return Task.FromResult(errors.Count == 0 ? 
                IdentityResult.Success : 
                IdentityResult.Failed(errors.ToArray()));
        }
    }
}