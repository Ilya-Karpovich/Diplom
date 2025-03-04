using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BsTYCallboard.Data;
using BsTYCallboard.Entity;
using Microsoft.EntityFrameworkCore;

namespace BsTYCallboard.Services
{
    public class FacultyService
    {
        private readonly ApplicationDbContext _context;

        public FacultyService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Создание нового факультета
        public async Task<Faculty> CreateFacultyAsync(Faculty faculty)
        {
            if (faculty == null || string.IsNullOrWhiteSpace(faculty.name))
            {
                throw new ArgumentException("Недопустимые данные факультета.");
            }

            _context.Faculties.Add(faculty);
            await _context.SaveChangesAsync();
            return faculty;
        }

        // Получение всех факультетов
        public async Task<List<Faculty>> GetAllFacultiesAsync()
        {
            return await _context.Faculties.ToListAsync();
        }

        // Получение факультета по ID
        public async Task<Faculty> GetFacultyByIdAsync(int id)
        {
            return await _context.Faculties.FindAsync(id);
        }

        // Обновление существующего факультета
        public async Task<bool> UpdateFacultyAsync(Faculty faculty)
        {
            if (faculty == null || string.IsNullOrWhiteSpace(faculty.name))
            {
                throw new ArgumentException("Недопустимые данные факультета.");
            }

            var existingFaculty = await _context.Faculties.FindAsync(faculty.id);
            if (existingFaculty == null)
            {
                return false;
            }

            existingFaculty.name = faculty.name;
            existingFaculty.description = faculty.description;

            _context.Faculties.Update(existingFaculty);
            await _context.SaveChangesAsync();
            return true;
        }

        // Удаление факультета по ID
        public async Task<bool> DeleteFacultyAsync(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null)
            {
                return false;
            }

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
