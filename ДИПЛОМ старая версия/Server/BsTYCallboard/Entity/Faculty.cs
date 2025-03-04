using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BsTYCallboard.Entity
{
    [Table("faculties")]
    public class Faculty
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(100)]
        public string name { get; set; }

        [Column("description")]
        [MaxLength(500)]
        public string description { get; set; }

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
