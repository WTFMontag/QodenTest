using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace WebApp
{
    [Route("api")]
    public class LoginController : Controller
    {
        private readonly IAccountDatabase _db;

        public LoginController(IAccountDatabase db)
        {
            _db = db;
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> Login(string userName)
        {
            var account = await _db.FindByUserNameAsync(userName);

            //TODO 2: return 404 if user not found
            if (account == null)
            {
                return NotFound();
            }

            var claims = new List<Claim>
            {
                 //TODO 1: Generate auth cookie for user 'userName' with external id
                new Claim(ClaimsIdentity.DefaultNameClaimType, account.ExternalId),
                new Claim(ClaimTypes.Role, account.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return Ok();
        }

    }
}