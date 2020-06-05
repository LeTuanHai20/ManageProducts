using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NG_Core_Auth.Helpers;
using NG_Core_Auth.ViewModels;

namespace NG_Core_Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppSettings _appSettings;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IOptions<AppSettings> appSettings)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._appSettings = appSettings.Value;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel formdata)
        {
          
                List<string> errorList = new List<string>();
                var user = new IdentityUser()
                {
                    Email = formdata.Email,
                    UserName = formdata.Username,
                    SecurityStamp = new Guid().ToString()
                };
                var result = await _userManager.CreateAsync(user, formdata.Password);
                if (result.Succeeded)
                {
                   await _userManager.AddToRoleAsync(user, "Customer");
                    return Ok(new { username = user.UserName, email = user.Email, status = 1, message = "Registration success" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        errorList.Add(error.Description);
                    }
                    return BadRequest(new JsonResult(errorList));
                }
        
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel formdata)
        {
            var user = await _userManager.FindByNameAsync(formdata.Username);
            var role = await _userManager.GetRolesAsync(user);
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));
            var expiresTime = Convert.ToDouble(_appSettings.ExpireTime);
            if(user != null && await _userManager.CheckPasswordAsync(user, formdata.Password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor() {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, formdata.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, new Guid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, role.FirstOrDefault()),
                        new Claim("LoggedOn", DateTime.Now.ToString())
                    }),
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                    Issuer = _appSettings.Site,
                    Audience = _appSettings.Audience,
                    Expires = DateTime.UtcNow.AddMinutes(expiresTime)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                 return Ok(new { token = tokenHandler.WriteToken(token),expiration = token.ValidTo, username = user.UserName, role = role.FirstOrDefault()});
            }
            ModelState.AddModelError("", "user name/ password was not found");
            return Unauthorized(new { loginError = "Please check the Login credentials - invalid Username/ password was entered" });

        }

    }
}