namespace mvc_ef.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordSalt { get; set; } = new byte[0];
        public DateTime CreatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // EF Navigation Property
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}
