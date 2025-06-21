using BusinessObject.Models.DTO;

namespace BusinessObject.ObjectView
{
    public class CreateStoreSupplierView
    {
        public UserDTO userDTO { get; set; }
        public ServiceDTO serviceDTO { get; set; }
        public PetTypeDTO PetTypeDTO { get; set; }
    }
}