using BusinessObject.Models.DTO;

namespace BusinessObject.ObjectView
{
    public class MyInforView
    {
        public UserDTO userDTO { get; set; }
        public List<PetDTO> petDTOs { get; set; }
    }
}
