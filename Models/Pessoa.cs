using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SCAP.Models
{
    public class Pessoa : IdentityUser
    {
        [Required]
        [StringLength(128)]
        public string Nome { get; set; }

        [Required]
        [StringLength(128)]
        public string Sobrenome { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool Ativo { get; set; }

        // IdentityUser já possui uma classe de Matricula, porém esta será chamada de UserName
        // public string Matricula { get; set; }

        // IdentityUser já possui uma classe de Email
        // public string Email { get; set; }

        // IdentityUser já possui uma classe de Telefone, porém esta é chamada de PhoneNumber
        // public string Telefone { get; set; }
    }
}
