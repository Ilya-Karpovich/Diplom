using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BsTYCallboard.Entity
{
    [Table("teachers")]
    public class Teacher
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Required]
        [ForeignKey("Role")]
        [Column("role_id")]
        public int roleId { get; set; }

        [Required]
        [ForeignKey("Faculty")]
        [Column("faculty_id")]
        public int facultyId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string name { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        [Column("email")]
        public string email { get; set; }

        [Required]
        [Column("password")]
        public string password { get; set; } // Храним хэш и соль

        // Навигационные свойства
        public Role role { get; set; }
        public Faculty faculty { get; set; }

        // Метод для установки пароля с хэшированием
        public void SetPassword(string password)
        {
            // Генерация соли
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Хэширование пароля
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Сохранение соли и хэша
            this.password = $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        // Метод для проверки пароля
        public bool VerifyPassword(string password)
        {
            var parts = this.password.Split(':');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = parts[1];

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed == storedHash;
        }

        // Метод для сериализации объекта
        public string SerializeObject()
        {
            try
            {
                return JsonSerializer.Serialize(this);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Ошибка сериализации: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
