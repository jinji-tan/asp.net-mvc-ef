using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using mvc_ef.Data;
using mvc_ef.DTOs.User;
using mvc_ef.Helpers.interfaces;
using mvc_ef.Models;

namespace mvc_ef.Helpers
{
    public class AuthHelper : IAuthHelper
    {
        private readonly MyAppContext _context;
        public AuthHelper(MyAppContext context)
        {
            _context = context;
        }

        public async Task<bool> Register(RegisterDto registerDto)
        {
            using var hmac = new HMACSHA512();

            byte[] passwordSalt = hmac.Key;
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));

            var user = new User
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        public async Task<bool> UserExists(string email) =>
            await _context.Users.AnyAsync(u => u.Email == email);

        public async Task<User?> GetUserByEmail(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> Login(LoginDto loginDto)
        {
            var user = await GetUserByEmail(loginDto.Email);

            if (user != null && VerifyPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                return user;

            return null;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
