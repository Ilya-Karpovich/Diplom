using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BsTYCallboard.Data;
using BsTYCallboard.Entity;
using Microsoft.EntityFrameworkCore;

namespace BsTYCallboard.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Создание нового пользователя
        public async Task<User> CreateUserAsync(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.name) || string.IsNullOrWhiteSpace(user.email))
            {
                throw new ArgumentException("Недопустимые данные пользователя.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Получение всех пользователей
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.role)
                .Include(u => u.speciality)
                .Include(u => u.faculty)
                .ToListAsync();
        }

        // Получение пользователя по ID
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.role)
                .Include(u => u.speciality)
                .Include(u => u.faculty)
                .FirstOrDefaultAsync(u => u.id == id);
        }

        // Обновление существующего пользователя
        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.name) || string.IsNullOrWhiteSpace(user.email))
            {
                throw new ArgumentException("Недопустимые данные пользователя.");
            }

            var existingUser = await _context.Users.FindAsync(user.id);
            if (existingUser == null)
            {
                return false;
            }

            existingUser.name = user.name;
            existingUser.email = user.email;
            existingUser.roleId = user.roleId;
            existingUser.specialityId = user.specialityId;
            existingUser.facultyId = user.facultyId;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            return true;
        }

        // Удаление пользователя по ID
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
