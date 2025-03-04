using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BsTYCallboard.Data;
using BsTYCallboard.Entity;
using Microsoft.EntityFrameworkCore;

namespace BsTYCallboard.Services
{
    public class RoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Создание новой роли
        public async Task<Role> CreateRoleAsync(Role role)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.name))
            {
                throw new ArgumentException("Недопустимые данные роли.");
            }

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        // Получение всех ролей
        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        // Получение роли по ID
        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        // Обновление существующей роли
        public async Task<bool> UpdateRoleAsync(Role role)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.name))
            {
                throw new ArgumentException("Недопустимые данные роли.");
            }

            var existingRole = await _context.Roles.FindAsync(role.id);
            if (existingRole == null)
            {
                return false;
            }

            existingRole.name = role.name;

            _context.Roles.Update(existingRole);
            await _context.SaveChangesAsync();
            return true;
        }

        // Удаление роли по ID
        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return false;
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
