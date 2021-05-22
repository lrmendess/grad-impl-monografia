using Microsoft.AspNetCore.Identity;
using SCAP.Models;

namespace SCAP.Data.Seeds
{
    public static class UserSeeder
    {
        public static void SeedDev(UserManager<Pessoa> userManager)
        {
            var users = new Pessoa[]
            {
                new Secretario
                {
                    UserName = "2016101236",
                    Email = "lucasribeiromsr@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "27999999999",
                    PhoneNumberConfirmed = true,
                    Nome = "Lucas",
                    Sobrenome = "Silva",
                    Ativo = true
                }
            };

            AddUsers(userManager, users);
        }

        public static void SeedStagingOrProduction(UserManager<Pessoa> userManager)
        {
            var users = new Pessoa[]
            {
                new Secretario
                {
                    UserName = "2016101236",
                    Email = "lucasribeiromsr@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "27999999999",
                    PhoneNumberConfirmed = true,
                    Nome = "Lucas",
                    Sobrenome = "Silva",
                    Ativo = true
                }
            };

            AddUsers(userManager, users);
        }

        private static void AddUsers(UserManager<Pessoa> userManager, Pessoa[] users)
        {
            foreach (var user in users)
            {
                if (userManager.FindByNameAsync(user.UserName).Result != null)
                {
                    continue;
                }

                var result = userManager.CreateAsync(user, "senha@123").Result;

                if (result.Succeeded)
                {
                    if (user is Secretario)
                    {
                        userManager.AddToRoleAsync(user, "Secretario").Wait();
                    }

                    if (user is Professor)
                    {
                        userManager.AddToRoleAsync(user, "Professor").Wait();
                    }
                }
            }
        }
    }
}
