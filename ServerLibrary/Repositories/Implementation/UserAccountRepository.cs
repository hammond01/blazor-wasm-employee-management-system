
using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServerLibrary.Data;
using ServerLibrary.Helper;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementation
{
    public class UserAccountRepository(IOptions<JWTSection> config, AppDbContext appDbContext) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAsync(Register register)
        {
            if (register is null) return new GeneralResponse(false, "Model is empty?");
            var checkUser = await FindUserbyEmail(register!.Email!);
            if (checkUser != null) return new GeneralResponse(false, "User registered already");

            var applicationUser = await SaveToDatabase(new ApplicationUser
            {
                FullName = register.FullName,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
            });

            //check register and assign role
            //Role admin
            var checkAdminRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(h => h.Name!.Equals(Constants.Admin));
            if (checkAdminRole is null)
            {
                var createAdminRole = await SaveToDatabase(new SystemRole()
                {
                    Name = Constants.Admin,
                });
                await SaveToDatabase(new UserRole()
                {
                    RoleId = createAdminRole.Id,
                    UserId = applicationUser.Id,
                });
                return new GeneralResponse(true, "Account created");
            }
            //Role User
            var checkUserRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(h => h.Name!.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole is null)
            {
                response = await SaveToDatabase(new SystemRole()
                {
                    Name = Constants.User,
                });
                await SaveToDatabase(new UserRole()
                {
                    RoleId = response.Id,
                    UserId = applicationUser.Id,
                });
            }
            else
            {
                await SaveToDatabase(new UserRole()
                {
                    RoleId = checkUserRole.Id,
                    UserId = applicationUser.Id,
                });
            }
            return new GeneralResponse(true, "Account created");

        }


        public async Task<LoginResponse> SignInAsync(Login login)
        {
            if (login == null) return new LoginResponse(false, "Model is empyty!");
            var applicationUser = await FindUserbyEmail(login.Email!);
            if (applicationUser == null) return new LoginResponse(false, "User is not found1");

            if (!BCrypt.Net.BCrypt.Verify(login.Password, applicationUser.Password))
                return new LoginResponse(false, "Email/Password is not valid!");
            throw new NotImplementedException();
        }

        private async Task<ApplicationUser?> FindUserbyEmail(string email)
        {
            return await appDbContext.ApplicationUsers
                .FirstOrDefaultAsync(h => h.Email!.ToLower() == email.ToLower());
        }

        private async Task<T> SaveToDatabase<T>(T model)
        {
            var data = appDbContext.Add(model!);
            await appDbContext.SaveChangesAsync();
            return (T)data.Entity;
        }

    }
}
