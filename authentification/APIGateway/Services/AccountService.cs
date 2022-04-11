using APIGateway.Data;
using APIGateway.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace APIGateway.Services
{
    public class AccountService
    {
        private AppDbContext _db;

        public AccountService(AppDbContext appDbContext)
        {
            _db = appDbContext;

            if (!_db.Roles.Any(r => r.Name == RoleNames.ADMIN))
            {
                var admin = new User("admin", "admin");
                var role = new UserRole(RoleNames.ADMIN);
                admin.Roles = new();
                admin.Roles.Add(role);
                _db.Users.Add(admin);
                _db.SaveChanges();
            }
        }

        public Result<User> Register(RegisterModel registerModel)
        {
            if (!registerModel.IsValid())
                return Error("not valid");

            if (_db.Users.Any(u => u.Username == registerModel.Username))
                return Error("user already exists");

            var user = new User(
                registerModel.Username,
                registerModel.Password
            );

            var role = new UserRole(RoleNames.USER);
            user.Roles = new();
            user.Roles.Add(role);

            _db.Users.Add(user);
            _db.SaveChanges();

            return Success(user);
        }

        public Result<User> Login(LoginModel loginModel)
        {
            if (!loginModel.IsValid())
                return Error("not valid");

            var user = _db.Users
                .Include(user => user.Roles)
                .FirstOrDefault(u => u.Username == loginModel.Username);

            if (user == null || !user.ValidatePassword(loginModel.Password))
                return Error("invalid username or password");

            return Success(user);
        }

        private Result<User> Error(string message)
        {
            return Result<User>.CreateError(message);
        }

        private Result<User> Success(User user)
        {
            return Result<User>.CreateSuccess(user);
        }
    }
}
