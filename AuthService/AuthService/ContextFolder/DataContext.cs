using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthService.AuthModels;
using Microsoft.AspNetCore.Identity;

namespace AuthService.ContextFolder
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData
                (
                new IdentityRole() { Id = "1", Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                new IdentityRole() { Id = "2", Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" }
                );

            string password = "admin";
            var passwordHasher = new PasswordHasher<User>();
            string hashedPassword = passwordHasher.HashPassword(null, password);
            string passwordUser = "user";
            var passwordHasherUser = new PasswordHasher<User>();
            string hashedPasswordUser = passwordHasherUser.HashPassword(null, passwordUser);

            builder.Entity<User>().HasData
                (
                new User() { Id = "1", UserName = "admin", PasswordHash = hashedPassword, NormalizedUserName = "ADMIN", Email = "Admin@mail.ru" },
                new User() { Id = "2", UserName = "user", PasswordHash = hashedPasswordUser, NormalizedUserName = "USER", Email = "User@mail.ru" }
                );

            builder.Entity<IdentityUserRole<string>>().HasData
                (
                new IdentityUserRole<string> { UserId = "1", RoleId = "1" },
                new IdentityUserRole<string> { UserId = "2", RoleId = "2" }
                );
        }
    }
}
