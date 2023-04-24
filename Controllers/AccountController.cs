using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using SimplifiedNorthwind.Models;
using SimplifiedNorthwind.Models.ConData;

namespace SimplifiedNorthwind.Controllers
{
    [Route("Account/[action]")]
    public partial class AccountController : Controller
    {
        private readonly ConDataService _conDataService;//creation of private variable for ConData Service

        public AccountController(ConDataService conDataService)
        {
            _conDataService = conDataService;//injected ConDataService into AccountController
        }

        public IActionResult Login()
        {
            var redirectUrl = Url.Content("~/");

            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Logout()
        {
            var redirectUrl = Url.Content("~/");

            return SignOut(new AuthenticationProperties { RedirectUri = redirectUrl }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpPost]
        public async Task<ApplicationAuthenticationState> CurrentUser()
        {
            var solutionUser = await ValidateCurrentUser();
            if (solutionUser != null)
            {
                return new ApplicationAuthenticationState
                {
                    IsAuthenticated = User.Identity.IsAuthenticated,
                    Name = User.Identity.Name,
                    Claims = User.Claims.Select(c => new ApplicationClaim { Type = c.Type, Value = c.Value })
                };
            }
            else
            {
                return new ApplicationAuthenticationState
                {
                    IsAuthenticated = false,
                    Name = null,
                    Claims = new List<ApplicationClaim>()
                };
            }
                
        }

        private async Task<SolutionUser> ValidateCurrentUser()
        {
            SolutionUser userFromAD = await _conDataService.GetSolutionUserByEmail(User.Identity.Name);
            return userFromAD;
        }
    }
}