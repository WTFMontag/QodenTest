using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp
{
    // TODO 4: unauthorized users should receive 401 status code
    [Authorize]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize] 
        [HttpGet]
        public ValueTask<Account> Get()
        {
            string userId = User.FindFirstValue(ClaimsIdentity.DefaultNameClaimType);
            return _accountService.LoadOrCreateAsync(userId /* TODO 3: Get user id from cookie */);
        }

        //TODO 5: Endpoint should works only for users with "Admin" Role
        [Authorize(Roles = "Admin")]
        [Authorize]
        [HttpGet("{id}")]
        public Account GetByInternalId([FromRoute] int id)
        {
            return _accountService.GetFromCache(id);
        }

        [Authorize]
        [HttpPost("counter")]
        public async Task UpdateAccount()
        {
            //Update account in cache, don't bother saving to DB, this is not an objective of this task.
            var account = await Get();

            account.Counter++;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, account.ExternalId),
                new Claim(ClaimTypes.Role, account.Role),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

        }
    }
}






