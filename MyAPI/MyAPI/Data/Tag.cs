using System.ComponentModel.DataAnnotations;

namespace MyAPI.Data
{
    public class Tag
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public int StoryCount { get; set; }

        public ICollection<Story> Stories { get; set; }
    }
}
