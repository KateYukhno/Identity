using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppIdentity.Models;
using AppIdentity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AppIdentity.Controllers
{
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _applicationContext;

        public AccountController(UserManager<User> userManager, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _applicationContext = applicationContext;
        }

        [HttpPost]
        [Route("registration")]
        public async Task<IActionResult> Registration([FromBody] RegisterViewModel request)
        {
            var identity = await GetIdentityReg(request.Name, request.Login, request.Password);
            if (identity == null)
            {
                return BadRequest(new
                {
                    errorText = "Не удалось создать пользователя! " +
                                "Пожалуйста, проверьте данные пользователя и попробуйте еще раз."
                });
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.Key, SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            return Json(response);
        }

        private async Task<ClaimsIdentity> GetIdentityReg(string Name, string Login, string Password)
        {
            var userExists = await _userManager.FindByNameAsync(Login);
            if (userExists != null) return null;
            User user = new User()
            {
                Name = Name,
                UserName = Login
            };
            var result = await _userManager.CreateAsync(user, Password);
            if (!result.Succeeded) return null;
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName)
            };
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel request)
        {
            var identity = await GetIdentity(request.Login, request.Password);
            if (identity == null)
            {
                return BadRequest(new {errorText = "Неправильное имя пользователя или пароль."});
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.Key, SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            return Json(response);
        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return null;
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid) return null;
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName)
            };
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        [Authorize]
        [HttpGet]
        [Route("users")]
        public async Task<IEnumerable<UserViewModel>> GetAllUsers([FromRoute] string request)
        {
            var result = await _applicationContext.Users
                .AsNoTracking()
                .Select(e => new UserViewModel
                {
                    Name = e.Name,
                    Login = e.UserName
                }).ToListAsync();
            
            return result;
        }
    }
}