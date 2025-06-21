namespace BusinessObject.Models.DTO
{
    public class ExternalLoginDTO
    {
        public int Id { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}