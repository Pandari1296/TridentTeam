namespace BaseApplication.Entity
{
    public class ClientRegistration
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PointOfContacts { get; set; }
        public bool? Status { get; set; }
    }
}
