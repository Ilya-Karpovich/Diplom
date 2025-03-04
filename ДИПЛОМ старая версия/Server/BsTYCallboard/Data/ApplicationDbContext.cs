using Microsoft.EntityFrameworkCore;
using BsTYCallboard.Entity;

namespace BsTYCallboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Другие DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Group> Groups { get; set; }

        // Конструктор и методы конфигурации
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Метод OnConfiguring и OnModelCreating, если необходимо
    }
}
