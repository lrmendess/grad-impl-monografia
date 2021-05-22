using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SCAP.Models;
using SCAP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Extensions
{
    [HtmlTargetElement("*", Attributes = "supress-by-role")]
    public class SupressByRoleTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IProfessorService _professorService;
        private readonly IUserService<Pessoa> _userService;

        public SupressByRoleTagHelper(IHttpContextAccessor contextAccessor, IProfessorService professorService,
            IUserService<Pessoa> userService)
        {
            _contextAccessor = contextAccessor;
            _professorService = professorService;
            _userService = userService;
        }

        [HtmlAttributeName("supress-by-role")]
        public string UserRoles { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var user = _contextAccessor.HttpContext.User;
            var roles = UserRoles.Split(",").Select(r => r.Trim());

            var hasAccess = user.Identity.IsAuthenticated && roles.Any(u => user.IsInRole(u));

            if (roles.Contains("Chefe"))
            {
                var userId = _userService.GetId(user);
                var isChefe = _professorService.IsChefe(new Professor { Id = userId });
                
                hasAccess = hasAccess || isChefe;
            }

            if (hasAccess)
            {
                return;
            }

            output.SuppressOutput();
        }
    }
}
