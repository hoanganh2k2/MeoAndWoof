using BusinessObject.Models.DTO;

namespace BusinessObject.ObjectView
{
    public class ShopDetail1View
    {
        public ServiceDTO? serviceDTO { get; set; }
        public UserDTO? userDTO { get; set; }
        public List<ServicestoreDTO> productDTOs { get; set; }
        public OrderDTO? postOrder { get; set; }
        public List<CommentDTO> CommentDTOs { get; set; }
        public UserDTO? loggedInUserDTO { get; set; }
        public List<UserDTO> allUsersDTO { get; set; }
    }
}
