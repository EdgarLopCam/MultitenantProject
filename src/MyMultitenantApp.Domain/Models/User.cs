namespace MyMultitenantApp.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
