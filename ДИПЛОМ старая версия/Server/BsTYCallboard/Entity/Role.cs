using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BsTYCallboard.Entity
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("name")]
        public string name { get; set; }

        public string SerializeObject()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
