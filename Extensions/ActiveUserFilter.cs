using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SCAP.Models;

namespace SCAP.Extensions
{
    public class ActiveUserAttribute : TypeFilterAttribute
    {
        public ActiveUserAttribute() : base(typeof(ActiveUserFilter))
        {
            Arguments = new object[] { };
        }
    }

    public class ActiveUserFilter : IAuthorizationFilter
    {
        private readonly UserManager<Pessoa> _userManager;
        private readonly SignInManager<Pessoa> _signInManager;

        public ActiveUserFilter(UserManager<Pessoa> userManager, SignInManager<Pessoa> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = _userManager.GetUserAsync(context.HttpContext.User).Result;

            if (user != null && !user.Ativo)
            {
                _signInManager.SignOutAsync().Wait();
                context.Result = new ForbidResult();
            }
        }
    }
}
