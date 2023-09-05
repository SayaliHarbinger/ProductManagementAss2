using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductManagementAss2.Models.Domain;
using ProductManagementAss2.Models.View;

namespace ProductManagementAss2.Data
{
    public class ProductDbContext : IdentityDbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public ProductDbContext(DbContextOptions<ProductDbContext>options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRolesAndUsers(builder);
        }
        
        private async void SeedRolesAndUsers(ModelBuilder builder)
        {
            string password = _configuration.GetValue<string>("SeedUserPass").Trim();

            //Creating Role ID
            var superAdminRoleId = Guid.NewGuid().ToString();
            var adminRoleId = Guid.NewGuid().ToString();
            var userRoleId = "f66f10b8-fb12-45cd-bcea-6e2eb49b65f1";
            //Creating User ID
            var superAdminUserId = Guid.NewGuid().ToString();
            var adminUserId = Guid.NewGuid().ToString();
            var userUserId = Guid.NewGuid().ToString();
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = superAdminRoleId, Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                  
                new IdentityRole { Id = userRoleId, Name = "User", NormalizedName = "USER" }
            );
            var superAdminUser = new ApplicationUser
            
            {
                Id = superAdminUserId,
                UserName = "superadmin@example.com",
                NormalizedUserName = "SUPERADMIN@EXAMPLE.COM",
                Email = "superadmin@example.com",
                NormalizedEmail = "SUPERADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                FirstName = "Steve",
                LastName = "Roy"
            };
           superAdminUser.PasswordHash=new PasswordHasher<IdentityUser>().HashPassword(superAdminUser, password);
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
              
                FirstName = "John",
                LastName = "Mellus"
            };
            adminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(adminUser, password);
            var userUser = new ApplicationUser
            {
                Id = userUserId,
                UserName = "user@example.com",
                NormalizedUserName = "USER@EXAMPLE.COM",
                Email = "user@example.com",
                NormalizedEmail = "USER@EXAMPLE.COM",
                EmailConfirmed = true,
                FirstName = "Demo",
                LastName = "User"
            };
            userUser.PasswordHash=new PasswordHasher<IdentityUser>().HashPassword(userUser, password);
            builder.Entity<ApplicationUser>().HasData(
                superAdminUser,
                adminUser,
                userUser
            );
            var userRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string> { UserId = superAdminUserId, RoleId = superAdminRoleId},
                new IdentityUserRole<string> { UserId = superAdminUserId, RoleId = adminRoleId },
                new IdentityUserRole<string> { UserId = superAdminUserId, RoleId = userRoleId},
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId },
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = userRoleId },
                new IdentityUserRole<string> { UserId = userUserId, RoleId = userRoleId}
            };

            builder.Entity<IdentityUserRole<string>>().HasData(
                userRoles
            ); 
        }
    }
}
