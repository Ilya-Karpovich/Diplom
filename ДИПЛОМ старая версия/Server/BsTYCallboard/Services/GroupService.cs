using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BsTYCallboard.Data;
using BsTYCallboard.Entity;
using Microsoft.EntityFrameworkCore;

namespace BsTYCallboard.Services
{
    public class GroupService
    {
        private readonly ApplicationDbContext _context;

        public GroupService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Создание новой группы
        public async Task<Group> CreateGroupAsync(Group group)
        {
            if (group == null || string.IsNullOrWhiteSpace(group.name))
            {
                throw new ArgumentException("Недопустимые данные группы.");
            }

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        // Получение всех групп
        public async Task<List<Group>> GetAllGroupsAsync()
        {
            return await _context.Groups
                .Include(g => g.faculty)
                .Include(g => g.speciality)
                .Include(g => g.teacher)
                .ToListAsync();
        }

        // Получение группы по ID
        public async Task<Group> GetGroupByIdAsync(int id)
        {
            return await _context.Groups
                .Include(g => g.faculty)
                .Include(g => g.speciality)
                .Include(g => g.teacher)
                .FirstOrDefaultAsync(g => g.id == id);
        }

        // Обновление существующей группы
        public async Task<bool> UpdateGroupAsync(Group group)
        {
            if (group == null || string.IsNullOrWhiteSpace(group.name))
            {
                throw new ArgumentException("Недопустимые данные группы.");
            }

            var existingGroup = await _context.Groups.FindAsync(group.id);
            if (existingGroup == null)
            {
                return false;
            }

            existingGroup.name = group.name;
            existingGroup.facultyId = group.facultyId;
            existingGroup.specialityId = group.specialityId;
            existingGroup.teacherId = group.teacherId;

            _context.Groups.Update(existingGroup);
            await _context.SaveChangesAsync();
            return true;
        }

        // Удаление группы по ID
        public async Task<bool> DeleteGroupAsync(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return false;
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
