using BusinessObject.Models.DTO;

namespace BusinessObject.ObjectView
{
    public class PetView
    {
        public UserDTO UserDTO { get; set; }
        public List<PetDTO> Pets { get; set; }
        public List<PetTypeDTO> PetTypes { get; set; }
        public PetView()
        {
            PetTypes = new List<PetTypeDTO>();
            Pets = new List<PetDTO>();
        }
    }
}