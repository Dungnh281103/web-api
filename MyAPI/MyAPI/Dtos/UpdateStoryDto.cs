namespace MyAPI.Dtos
{
    public class UpdateStoryDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Translator { get; set; }
        public string CoverUrl { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool? IsRecommended { get; set; }
        public List<string> CategoryIds { get; set; }
        public List<string> TagIds { get; set; }
        public List<string> CategoryNames { get; set; }
        public List<string> TagNames { get; set; }
    }
}
