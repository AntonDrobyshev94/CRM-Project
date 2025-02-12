using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinalProject_API.Models;
using FinalProject_API.AuthFinalProjectApp;

namespace FinalProject_API.ContextFolder
{
    /// <summary>
    /// Класс контекста базы данных, используемый для конфигурации 
    /// и взаимодействия с базой данных через ORM Entity Framework Core.
    /// Наследуется от <see cref="IdentityDbContext{TUser}"/>, что 
    /// позволяет интегрировать Identity для управления пользователями.
    /// </summary>
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Application> Requests { get; set; }
        public DbSet<TagModel> Tags { get; set; } 
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<TitleModel> Title { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<LinkModel> Links { get; set; }
        public DbSet<BlogModel> Blogs { get; set; }
        public DbSet<ImageModel> Images { get; set; }

        /// <summary>
        /// Конструктор класса DataContext
        /// </summary>
        /// <param name="options"></param>
        public DataContext(DbContextOptions<DataContext> options)
    : base(options)
        {
        }

        /// <summary>
        /// Метод добавления в БД моделей при
        /// миграции.
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
            CreateContactsAndTittles(builder);
        }

        /// <summary>
        /// Метод создания аккаунтов пользователей
        /// </summary>
        /// <param name="builder"></param>
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
        
        /// <summary>
        /// Метод создания контактов и навигационных меню
        /// при миграции
        /// </summary>
        /// <param name="builder"></param>
        private void CreateContactsAndTittles(ModelBuilder builder)
        {
            builder.Entity<TitleModel>().HasData
                (
                new TitleModel() {
                    Id = 1,
                    Title = "Оставить заявку или задать вопрос",
                    BlogTitle = "Блог",
                    ProjectsTitle = "Проекты",
                    ServicesTitle = "Услуги",
                    MainTitle = "Главная",
                    ContactsTitle = "Контакты",
                });

            builder.Entity<Contacts>().HasData
                (
                new Contacts() {
                    Id = 1,
                    Address = "Какой-либо адрес",
                    Email = "Какой-либо Email",
                    Fax = "Какой-либо факс",
                    Telephone = "Какой-либо телефон"
                });
        }
    }
}
