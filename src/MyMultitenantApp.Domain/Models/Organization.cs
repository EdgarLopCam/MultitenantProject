namespace MyMultitenantApp.Domain.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SlugTenant { get; set; }
    }
}
