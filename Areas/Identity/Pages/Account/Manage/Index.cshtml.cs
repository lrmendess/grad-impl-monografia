using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SCAP.Extensions;
using SCAP.Models;

namespace SCAP.Areas.Identity.Pages.Account.Manage
{
    [ActiveUser]
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<Pessoa> _userManager;
        private readonly SignInManager<Pessoa> _signInManager;

        public IndexModel(
            UserManager<Pessoa> userManager,
            SignInManager<Pessoa> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [DisplayName("Número de matrícula")]
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [StringLength(128, ErrorMessage = "A {0} deve ter no máximo {1} caracteres.")]
            [DisplayName("Nome")]
            public string Nome { get; set; }

            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [StringLength(128, ErrorMessage = "A {0} deve ter no máximo {1} caracteres.")]
            [DisplayName("Sobrenome")]
            public string Sobrenome { get; set; }

            [Phone]
            [Required(ErrorMessage = "O campo {0} é obrigatório.")]
            [RegularExpression("([0-9]+)", ErrorMessage = "O campo {0} deve conter apenas dígitos")]
            [Display(Name = "Telefone")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(Pessoa user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Nome = user.Nome,
                Sobrenome = user.Sobrenome,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Ocorreu um erro inesperado ao mudar o número de telefone, tente novamente.";
                    return RedirectToPage();
                }
            }

            if (Input.Nome != user.Nome || Input.Sobrenome != user.Sobrenome)
            {
                user.Nome = Input.Nome;
                user.Sobrenome = Input.Sobrenome;
            }

            var nameChange = await _userManager.UpdateAsync(user);

            if (!nameChange.Succeeded)
            {
                StatusMessage = "Ocorreu um erro inesperado ao mudar seu nome, tente novamente.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Seu perfil foi atualizado";
            return RedirectToPage();
        }
    }
}
