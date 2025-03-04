using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BsTYCallboard.Entity
{
    [Table("groups")]
    public class Group
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("faculty_id")]
        public int facultyId { get; set; }

        [Column("speciality_id")]
        public int specialityId { get; set; }

        [Column("teacher_id")]
        public int teacherId { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(100)]
        public string name { get; set; }

        // Навигационные свойства для связей с другими таблицами
        [ForeignKey("FacultyId")]
        public Faculty faculty { get; set; }

        [ForeignKey("SpecialityId")]
        public Speciality speciality { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher teacher { get; set; }

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
