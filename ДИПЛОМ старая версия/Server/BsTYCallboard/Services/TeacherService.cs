using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BsTYCallboard.Data;
using BsTYCallboard.Entity;
using Microsoft.EntityFrameworkCore;

namespace BsTYCallboard.Services
{
    public class TeacherService
    {
        private readonly ApplicationDbContext _context;

        public TeacherService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Создание нового преподавателя
        public async Task<Teacher> CreateTeacherAsync(Teacher teacher)
        {
            if (teacher == null || string.IsNullOrWhiteSpace(teacher.name) || string.IsNullOrWhiteSpace(teacher.email))
            {
                throw new ArgumentException("Недопустимые данные преподавателя.");
            }

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        // Получение всех преподавателей
        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            return await _context.Teachers
                .Include(t => t.role)
                .Include(t => t.faculty)
                .ToListAsync();
        }

        // Получение преподавателя по ID
        public async Task<Teacher> GetTeacherByIdAsync(int id)
        {
            return await _context.Teachers
                .Include(t => t.role)
                .Include(t => t.faculty)
                .FirstOrDefaultAsync(t => t.id == id);
        }

        // Обновление существующего преподавателя
        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            if (teacher == null || string.IsNullOrWhiteSpace(teacher.name) || string.IsNullOrWhiteSpace(teacher.email))
            {
                throw new ArgumentException("Недопустимые данные преподавателя.");
            }

            var existingTeacher = await _context.Teachers.FindAsync(teacher.id);
            if (existingTeacher == null)
            {
                return false;
            }

            existingTeacher.name = teacher.name;
            existingTeacher.email = teacher.email;
            existingTeacher.roleId = teacher.roleId;
            existingTeacher.facultyId = teacher.facultyId;

            _context.Teachers.Update(existingTeacher);
            await _context.SaveChangesAsync();
            return true;
        }

        // Удаление преподавателя по ID
        public async Task<bool> DeleteTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return false;
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
