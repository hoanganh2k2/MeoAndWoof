namespace BusinessObject.Models.DTO
{
    public class ServiceTypeDTO
    {
        public int Servicetypeid { get; set; }

        public string Servicetypename { get; set; } = null!;

        public int? Status { get; set; }

        public string? Description { get; set; }

        public string? Serviceimage { get; set; }
    }
}
