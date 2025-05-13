using System.ComponentModel.DataAnnotations;

namespace MyAPI.Data
{
    public class Category
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }


        public ICollection<Story> Stories { get; set; }
    }
}
