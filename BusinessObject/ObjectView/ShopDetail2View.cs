using BusinessObject.Models.DTO;

namespace BusinessObject.ObjectView
{
    public class ShopDetail2View
    {
        public ServiceDTO? serviceDTO { get; set; }
        public List<ServiceImageDTO>? imageDTOs { get; set; }
        public UserDTO? userDTO { get; set; }
        public List<PetDTO>? petDTOs { get; set; }
        public List<ServicePriceDTO>? servicePriceDTOs { get; set; }
        public BookingDTO? postBooking { get; set; }
    }
}
