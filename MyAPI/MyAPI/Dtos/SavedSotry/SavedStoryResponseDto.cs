namespace MyAPI.Dtos.SavedStory
{
    public class SavedStoryResponseDto
    {
        public int Id { get; set; }
        public string StoryId { get; set; }

        // Thêm 2 trường mới
        public string Title { get; set; }
        public string Author { get; set; }

        public DateTime SavedAt { get; set; }
    }
}
