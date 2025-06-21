using BusinessObject.Models.DTO;

namespace BusinessObject.ObjectView
{
    public class OrderDetailView
    {
        public List<OrderDetailDTO> orderDetailDTOs { get; set; }
        public OrderDTO orderDTO { get; set; }
    }
}
