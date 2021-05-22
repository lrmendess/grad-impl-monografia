using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SCAP.Extensions;
using SCAP.Models;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using SCAP.ViewModels;

namespace SCAP.Controllers
{
    [Authorize, ActiveUser]
    public class UsersController : BaseController
    {
        private readonly IProfessorService _professorService;
        private readonly ISecretarioService _secretarioService;
        private readonly IEmailService _emailService;

        public UsersController(IProfessorService professorService, ISecretarioService secretarioService,
            IEmailService emailService,
            IUserService<Pessoa> userService, IMapper mapper, INotificator notificator)
            : base(userService, mapper, notificator)
        {
            _professorService = professorService;
            _secretarioService = secretarioService;
            _emailService = emailService;
        }

        [Route("usuarios")]
        public IActionResult Index()
        {
            var userViewModelCollection = new UserViewModelCollection
            {
                Professores = Mapper.Map<IEnumerable<UserViewModel>>(_professorService.GetAll()),
                Secretarios = Mapper.Map<IEnumerable<UserViewModel>>(_secretarioService.GetAll())
            };

            return View(userViewModelCollection);
        }

        [Route("usuarios/{id:guid}")]
        public IActionResult Details(string id)
        {
            var user = UserService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            if (user is Professor)
            {
                return View(Mapper.Map<UserViewModel>(_professorService.GetWithParentescos(user.Id)));
            }

            return View(Mapper.Map<UserViewModel>(user));
        }

        [Authorize(Roles = "Secretario")]
        [Route("usuarios/novo")]
        public ActionResult Create()
        {
            return PartialView("Modals/_CreateUserModal");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Secretario")]
        [Route("usuarios/novo")]
        public ActionResult Create(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Modals/_CreateUserModal", userViewModel);
            }

            Pessoa user;

            if (userViewModel.UserType == 1)
            {
                user = new Professor
                {
                    UserName = userViewModel.UserName,
                    Email = userViewModel.Email,
                    EmailConfirmed = true,
                    PhoneNumber = userViewModel.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    Nome = userViewModel.Nome,
                    Sobrenome = userViewModel.Sobrenome,
                    Ativo = true
                };
            }
            else if (userViewModel.UserType == 2)
            {
                user = new Secretario
                {
                    UserName = userViewModel.UserName,
                    Email = userViewModel.Email,
                    EmailConfirmed = true,
                    PhoneNumber = userViewModel.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    Nome = userViewModel.Nome,
                    Sobrenome = userViewModel.Sobrenome,
                    Ativo = true
                };
            }
            else
            {
                return BadRequest();
            }

            UserService.Add(user);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_CreateUserModal", userViewModel);
            }

            TempData["Success"] = "Usuário criado com sucesso!";

            _emailService.SendEmailDefinePassword(user).Wait();

            return JsonOk(Url.Action(nameof(Details), new { id = UserService.GetByEmail(user.Email).Id }));
        }

        [Authorize(Roles = "Secretario")]
        [Route("usuarios/{id:guid}/editar")]
        public IActionResult Edit(string id)
        {
            var user = UserService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return PartialView("Modals/_EditUserModal", Mapper.Map<UserViewModel>(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Secretario")]
        [Route("usuarios/{id:guid}/editar")]
        public IActionResult Edit(string id, UserViewModel userViewModel)
        {
            if (id != userViewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return PartialView("Modals/_EditUserModal", userViewModel);
            }

            var user = UserService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Nome = userViewModel.Nome;
            user.Sobrenome = userViewModel.Sobrenome;
            user.Email = userViewModel.Email;
            user.PhoneNumber = userViewModel.PhoneNumber;
            user.Ativo = userViewModel.Ativo;

            UserService.Update(user);

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_EditUserModal", userViewModel);
            }

            TempData["Success"] = "Usuário editado com sucesso!";

            return JsonOk(Url.Action(nameof(Details), new { id }));
        }

        [Authorize(Roles = "Secretario")]
        [Route("usuarios/{id:guid}/excluir")]
        public IActionResult Delete(string id)
        {
            var user = UserService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return PartialView("Modals/_DeleteUserModal", Mapper.Map<UserViewModel>(user));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Secretario")]
        [Route("usuarios/{id:guid}/excluir")]
        public IActionResult DeleteConfirmed(string id)
        {
            var user = UserService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            if (user is Professor)
            {
                _professorService.Remove(user.Id);
            }
            else
            {
                UserService.Remove(user.Id);
            }

            if (ErrorsOccurred())
            {
                return PartialView("Modals/_DeleteUserModal", Mapper.Map<UserViewModel>(user));
            }

            TempData["Success"] = "Usuário excluído com sucesso!";

            return JsonOk(Url.Action(nameof(Index)));
        }

        [AllowAnonymous]
        public IActionResult DefinePassword(string encodedEmail, string encodedToken)
        {
            if (encodedEmail == null || encodedToken == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            string email = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedEmail));
            string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));

            var user = UserService.GetByEmail(email);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            return View(new UserDefinePasswordViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Token = token
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult DefinePassword(UserDefinePasswordViewModel model)
        {
            if (model.Id == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = UserService.Get(model.Id);

            if (user == null)
            {
                return NotFound();
            }

            UserService.ResetPassword(user, model.Token, model.Password);

            if (ErrorsOccurred())
            {
                return View(model);
            }

            TempData["Success"] = "Senha definida com sucesso!";

            return View("DefinePasswordConfirmed");
        }
    }
}
