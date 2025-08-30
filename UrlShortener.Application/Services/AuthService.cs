using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Domain.Models;

namespace UrlShortener.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        private const string DefaultRoleName = "User";

        public AuthService(
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            IUserRoleRepository userRoleRepo,
            IConfiguration configuration)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _userRoleRepo = userRoleRepo;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<string?> LoginAsync(string userName, string password)
        {
            var user = await _userRepo.GetByUserNameAsync(userName);
            if (user == null) return null;

            var isValidPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password)
                                  == PasswordVerificationResult.Success;
            return isValidPassword ? GenerateJwtToken(user) : null;
        }

        public async Task<User> RegisterAsync(string userName, string password)
        {
            if (await _userRepo.GetByUserNameAsync(userName) is not null)
                throw new Exception("This name is already taken");

            var user = new User
            {
                UserName = userName,
                PasswordHash = _passwordHasher.HashPassword(null!, password)
            };

            await _userRepo.AddAsync(user);

            var defaultRole = await _roleRepo.GetByNameAsync(DefaultRoleName)
                              ?? throw new Exception($"Role '{DefaultRoleName}' not found. Seed roles into the database.");

            await _userRoleRepo.AddAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = defaultRole.Id
            });

            return await _userRepo.GetByUserNameAsync(user.UserName);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            claims.AddRange(user.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role.Name)));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
