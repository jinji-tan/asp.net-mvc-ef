namespace mvc_ef.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }

        // EF Navigation Property
        public User User { get; set; } = null!;
    }
}
