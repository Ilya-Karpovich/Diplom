using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BsTYCallboard.Data;
using BsTYCallboard.Entity;
using Microsoft.EntityFrameworkCore;

namespace BsTYCallboard.Services
{
    public class SpecialityService
    {
        private readonly ApplicationDbContext _context;

        public SpecialityService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Создание новой специальности
        public async Task<Speciality> CreateSpecialityAsync(Speciality speciality)
        {
            if (speciality == null || string.IsNullOrWhiteSpace(speciality.name))
            {
                throw new ArgumentException("Недопустимые данные специальности.");
            }

            _context.Specialities.Add(speciality);
            await _context.SaveChangesAsync();
            return speciality;
        }

        // Получение всех специальностей
        public async Task<List<Speciality>> GetAllSpecialitiesAsync()
        {
            return await _context.Specialities
                .Include(s => s.faculty)
                .ToListAsync();
        }

        // Получение специальности по ID
        public async Task<Speciality> GetSpecialityByIdAsync(int id)
        {
            return await _context.Specialities
                .Include(s => s.faculty)
                .FirstOrDefaultAsync(s => s.id == id);
        }

        // Обновление существующей специальности
        public async Task<bool> UpdateSpecialityAsync(Speciality speciality)
        {
            if (speciality == null || string.IsNullOrWhiteSpace(speciality.name))
            {
                throw new ArgumentException("Недопустимые данные специальности.");
            }

            var existingSpeciality = await _context.Specialities.FindAsync(speciality.id);
            if (existingSpeciality == null)
            {
                return false;
            }

            existingSpeciality.name = speciality.name;
            existingSpeciality.description = speciality.description;
            existingSpeciality.faculty = speciality.faculty;

            _context.Specialities.Update(existingSpeciality);
            await _context.SaveChangesAsync();
            return true;
        }

        // Удаление специальности по ID
        public async Task<bool> DeleteSpecialityAsync(int id)
        {
            var speciality = await _context.Specialities.FindAsync(id);
            if (speciality == null)
            {
                return false;
            }

            _context.Specialities.Remove(speciality);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
