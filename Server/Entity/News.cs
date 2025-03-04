using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Entity
{
    [Index("id", IsUnique = true)]
    public class News
    {
        [Column("id")]
        [Required]
        [Key]
        public int id { get; set; }
        [Column("title")]
        [Required]
        public string title {  get; set; }
        [Column("image_sourse")]
        public string newsImageSourse {  get; set; }
        [Column("description")]
        [Required]
        public string description {  get; set; }

    }
}
