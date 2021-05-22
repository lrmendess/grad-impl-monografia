using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SCAP.Data.Seeds
{
    public static class RoleSeeder
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = "Secretario",
                    NormalizedName = "SECRETARIO"
                },
                new IdentityRole
                {
                    Name = "Professor",
                    NormalizedName = "PROFESSOR"
                }
            );
        }
    }
}
