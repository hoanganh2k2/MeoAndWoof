namespace BusinessObject.Models.DTO
{
    public class PetTypeDTO
    {
        public int Pettypeid { get; set; }

        public string Pettypename { get; set; } = null!;

        public int WeightFrom { get; set; }

        public int WeightTo { get; set; }
    }
}
