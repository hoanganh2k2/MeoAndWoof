using BusinessObject.Models.DTO;
using BusinessObject.ObjectView;
using MeoAndWoofClient.Payment;
using MeoAndWoofClient.Payment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;

namespace MeoAndWoofClient.Controllers
{
	public class ServiceController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly IVnPayService _vnPayService;
		public ServiceController(IVnPayService vnPayService)
		{
			_httpClient = new HttpClient
			{
				BaseAddress = new Uri("https://localhost:7086/api/")
			};
			_vnPayService = vnPayService;
		}
		private int? GetUserId() => HttpContext.Session.GetInt32("UserId");
		public async Task<IActionResult> ServiceDetail(int id)
		{
			HttpResponseMessage response = await _httpClient.GetAsync($"service?$filter=servicetypeid eq {id} & status eq 1");
			List<ServiceDTO> serviceDTOs = new();
			if (response.StatusCode == HttpStatusCode.OK)
			{
				string content = await response.Content.ReadAsStringAsync();
				serviceDTOs = JsonConvert.DeserializeObject<List<ServiceDTO>>(content);
			}
			ViewBag.ServiceCount = serviceDTOs.Count;
			ViewBag.AllStar = serviceDTOs.Average(s => s.Servicestar);
			ViewBag.AllReview = serviceDTOs.Sum(s => s.ServiceNumberReview);

			return View(serviceDTOs);
		}
		public async Task<IActionResult> ShopDetail1(int id)
		{
			HttpResponseMessage serviceResponse = await _httpClient.GetAsync($"service/{id}");
			HttpResponseMessage productsResponse = await _httpClient.GetAsync($"product?$filter=serviceid eq {id} and statusid eq 1");
			ShopDetail1View shopDetailView = new();

			if (serviceResponse.StatusCode == HttpStatusCode.OK)
			{
				string serviceContent = await serviceResponse.Content.ReadAsStringAsync();
				ServiceDTO serviceDTO = JsonConvert.DeserializeObject<ServiceDTO>(serviceContent);
				shopDetailView.serviceDTO = serviceDTO;
			}

			if (productsResponse.StatusCode == HttpStatusCode.OK)
			{
				string productContent = await productsResponse.Content.ReadAsStringAsync();
				List<ServicestoreDTO> servicestoreDTOs = JsonConvert.DeserializeObject<List<ServicestoreDTO>>(productContent);
				shopDetailView.productDTOs = servicestoreDTOs;
			}

			HttpResponseMessage userResponse = await _httpClient.GetAsync($"user/{shopDetailView.serviceDTO.Userid}");
			if (userResponse.StatusCode == HttpStatusCode.OK)
			{
				string userContent = await userResponse.Content.ReadAsStringAsync();
				UserDTO userDTO = JsonConvert.DeserializeObject<UserDTO>(userContent);

				shopDetailView.userDTO = userDTO;
			}

			HttpResponseMessage anotherUserResponse = await _httpClient.GetAsync($"user");
			if (anotherUserResponse.StatusCode == HttpStatusCode.OK)
			{
				string anotherUserContent = await anotherUserResponse.Content.ReadAsStringAsync();
				List<UserDTO> allUsersDTO = JsonConvert.DeserializeObject<List<UserDTO>>(anotherUserContent);

				shopDetailView.allUsersDTO = allUsersDTO;
			}

			HttpResponseMessage commentsResponse = await _httpClient.GetAsync($"comment/filter?serviceId={id}");
			if (commentsResponse.StatusCode == HttpStatusCode.OK)
			{
				string commentsContent = await commentsResponse.Content.ReadAsStringAsync();
				List<CommentDTO> commentDTOs = JsonConvert.DeserializeObject<List<CommentDTO>>(commentsContent);
				shopDetailView.CommentDTOs = commentDTOs;
			}

			int? loggedInUserId = HttpContext.Session.GetInt32("UserId");
			if (loggedInUserId != null)
			{
				HttpResponseMessage loggedInUserResponse = await _httpClient.GetAsync($"user/{loggedInUserId}");
				if (loggedInUserResponse.StatusCode == HttpStatusCode.OK)
				{
					string loggedInUserContent = await loggedInUserResponse.Content.ReadAsStringAsync();
					UserDTO loggedInUserDTO = JsonConvert.DeserializeObject<UserDTO>(loggedInUserContent);
					shopDetailView.loggedInUserDTO = loggedInUserDTO;
				}
			}

			ViewBag.ServiceId = id;
			return View(shopDetailView);
		}
		public async Task<IActionResult> ShopDetail2(int id)
		{
			HttpResponseMessage serviceResponse = await _httpClient.GetAsync($"service/{id}");
			HttpResponseMessage imagesResponse = await _httpClient.GetAsync($"image?$filter=serviceid eq {id}");
			HttpResponseMessage petResponse = await _httpClient.GetAsync($"pet?$filter=userid eq {GetUserId()} and status eq 1");
			HttpResponseMessage priceResponse = await _httpClient.GetAsync($"Price?$filter=serviceid eq {id} and enddate eq null");
			ShopDetail2View shopDetailView = new();

			if (serviceResponse.StatusCode == HttpStatusCode.OK)
			{
				string serviceContent = await serviceResponse.Content.ReadAsStringAsync();
				ServiceDTO serviceDTO = JsonConvert.DeserializeObject<ServiceDTO>(serviceContent);
				shopDetailView.serviceDTO = serviceDTO;
			}

			if (imagesResponse.StatusCode == HttpStatusCode.OK)
			{
				string imagesContent = await imagesResponse.Content.ReadAsStringAsync();
				shopDetailView.imageDTOs = JsonConvert.DeserializeObject<List<ServiceImageDTO>>(imagesContent);
			}

			HttpResponseMessage userResponse = await _httpClient.GetAsync($"user/{shopDetailView.serviceDTO.Userid}");
			if (userResponse.StatusCode == HttpStatusCode.OK)
			{
				string userContent = await userResponse.Content.ReadAsStringAsync();
				UserDTO userDTO = JsonConvert.DeserializeObject<UserDTO>(userContent);

				shopDetailView.userDTO = userDTO;
			}

			if (petResponse.StatusCode == HttpStatusCode.OK)
			{
				string pettypesContent = await petResponse.Content.ReadAsStringAsync();
				shopDetailView.petDTOs = JsonConvert.DeserializeObject<List<PetDTO>>(pettypesContent);
			}

			if (priceResponse.StatusCode == HttpStatusCode.OK)
			{
				string priceContent = await priceResponse.Content.ReadAsStringAsync();
				shopDetailView.servicePriceDTOs = JsonConvert.DeserializeObject<List<ServicePriceDTO>>(priceContent);
			}
			ViewBag.ServiceId = id;
			return View(shopDetailView);
		}

		private async Task<IActionResult> CreateOrder(List<CartItemDTO> listItems)
		{
			HttpResponseMessage orderID = await _httpClient.GetAsync("order/orderid");
			string orderIdContent = await orderID.Content.ReadAsStringAsync();
			long orderId = JsonConvert.DeserializeObject<long>(orderIdContent);

			foreach (CartItemDTO item in listItems)
			{
				string numericString = item.price.Replace(".", "").Replace("₫", "").Trim();
				int price = int.Parse(numericString);
				OrderDetailDTO orderDetailDTO = new()
				{
					ProductId = item.productid,
					OrderId = (int)orderId,
					Quantity = item.quantity,
					UnitPrice = price,
					Discount = 0
				};
				HttpResponseMessage Detailresponse = await _httpClient.PostAsJsonAsync("orderdetail", orderDetailDTO);
			}

			return RedirectToAction("History", "User", new { id = GetUserId() });
		}

		[HttpPost]
		public async Task<IActionResult> Book1(ShopDetail1View view, int ServiceId, string listItemsJson, int sum)
		{
			if (GetUserId() == null) return RedirectToAction("Login", "Authen");

			List<CartItemDTO> listItems = JsonConvert.DeserializeObject<List<CartItemDTO>>(listItemsJson);
			if (listItems.Count == 0) return RedirectToAction("ShopDetail1", "Service", new { id = ServiceId });

			view.postOrder.OrderDate = DateTime.Now;
			view.postOrder.AddressShip = view.userDTO.Address;
			view.postOrder.UserId = GetUserId();
			view.postOrder.Total = sum;
			view.postOrder.StoreId = ServiceId;
			HttpResponseMessage response;

			if (view.postOrder.Status == 1)
			{
				response = await _httpClient.PostAsJsonAsync("order/PostCOD", view.postOrder);

			}
			else if (view.postOrder.Status == 2)
			{
				HttpContext.Session.SetString("ShopDetail1View", JsonConvert.SerializeObject(view));
				HttpContext.Session.SetString("ListItems", JsonConvert.SerializeObject(listItems));
				VnPaymentRequestModel vnPayModel = new()
				{
					Amount = (double)view.postOrder.Total,
					CreatedDate = DateTime.Now,
					Description = $"thanh toán hóa đơn",
					FullName = view.userDTO.Fullname,
				};
				return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
			}
			else
			{
				return RedirectToAction("ShopDetail1", "Service", new { id = ServiceId });
				response = await _httpClient.PostAsJsonAsync("order/PostPayOS", view.postOrder);

			}

			if (response.StatusCode == HttpStatusCode.Created)
			{
				CreateOrder(listItems);
			}
			return RedirectToAction("ShopDetail1", "Service", new { id = ServiceId });
		}

		[HttpPost]
		public async Task<IActionResult> Book2(ShopDetail2View view, int dayNumber, string time, string day, int ServiceId)
		{
			if (GetUserId() == null) return RedirectToAction("Login", "Authen");
			ModelState.Remove("postBooking.Pet");
			ModelState.Remove("postBooking.Service");
			if (ModelState.IsValid)
			{
				string dateTimeString = day + " " + time;
				string format = "MM/dd/yyyy h:mm tt";
				string formatIWant = "yyyy-MM-dd HH:mm:ss";

				DateTime dateTime = DateTime.ParseExact(dateTimeString, format, CultureInfo.InvariantCulture);
				string formattedDateTime = dateTime.ToString(formatIWant);

				DateTime startDate = DateTime.ParseExact(formattedDateTime, formatIWant, CultureInfo.InvariantCulture);
				DateTime endDate = startDate.AddDays(dayNumber);

				DateTime currentTimeWithOffset = DateTime.Now;
				string currentDateTime = currentTimeWithOffset.ToString(formatIWant);
				DateTime parsedDateTime = DateTime.ParseExact(currentDateTime, formatIWant, CultureInfo.InvariantCulture);

				view.postBooking.Bookingdate = parsedDateTime;
				view.postBooking.Startbooking = startDate;
				view.postBooking.Endbooking = endDate;

				view.postBooking.Serviceid = ServiceId;
				view.postBooking.Userid = GetUserId();
				view.postBooking.Statuspaid = 1;

				HttpResponseMessage priceResponse = await _httpClient.GetAsync($"booking/{view.postBooking.Petid},{ServiceId}/getprice");
				if (priceResponse.StatusCode == HttpStatusCode.OK)
				{
					string priceContent = await priceResponse.Content.ReadAsStringAsync();
					double price = JsonConvert.DeserializeObject<double>(priceContent);
					int? totalPrice = (int?)(price * dayNumber);
					view.postBooking.Totalprice = totalPrice;
				}

				HttpResponseMessage response = await _httpClient.PostAsJsonAsync("booking", view.postBooking);
				if (response.StatusCode == HttpStatusCode.Created)
				{
					return RedirectToAction("HistoryBooking", "User", new { id = GetUserId() });
				}
			}
			return RedirectToAction("ShopDetail2", "Service", new { id = ServiceId });
		}

		[HttpPost]
		public async Task<IActionResult> PostMessage(CommentDTO commentDTO)
		{
			int? loggedInUserId = HttpContext.Session.GetInt32("UserId");
			if (loggedInUserId == null)
			{
				return Unauthorized();
			}

			commentDTO.Userid = loggedInUserId.Value;
			commentDTO.CreateAt = DateTime.Now;
			commentDTO.UpdateAt = DateTime.Now;

			HttpResponseMessage response = await _httpClient.PostAsJsonAsync("comment", commentDTO);

			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction("ShopDetail1", new { id = commentDTO.Serviceid });
			}
			else
			{
				string errorContent = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Failed to post comment. Server error: {errorContent}");

				ModelState.AddModelError(string.Empty, $"Failed to post comment. Server error: {errorContent}");
				return View("Error");
			}
		}
		public async Task<IActionResult> GetComments(int serviceId)
		{
			int? userId = GetUserId();
			if (userId == null) return RedirectToAction("Login", "Authen");

			HttpResponseMessage response = await _httpClient.GetAsync($"comment/filter?serviceId={serviceId}&userId={userId}");
			List<CommentDTO> commentDTOs = new();
			if (response.StatusCode == HttpStatusCode.OK)
			{
				string content = await response.Content.ReadAsStringAsync();
				commentDTOs = JsonConvert.DeserializeObject<List<CommentDTO>>(content);
			}

			return View(commentDTOs);
		}

		[HttpGet]
		public async Task<IActionResult> DeleteComment(int commentId, int serviceId)
		{
			HttpResponseMessage response = await _httpClient.DeleteAsync($"comment/{commentId}");

			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction("ShopDetail1", new { id = serviceId });
			}

			string errorContent = await response.Content.ReadAsStringAsync();
			ModelState.AddModelError(string.Empty, $"Failed to delete comment. Server error: {errorContent}");
			return View("Error");
		}

		[Authorize]
		public IActionResult PaymentFail()
		{
			return View("PaymentNotification/PaymentFail");
		}

		[Authorize]
		public IActionResult PaymentSuccess()
		{
			return View("PaymentNotification/PaymentSuccess");
		}

		[Authorize]
		public async Task<IActionResult> PaymentCallBack()
		{
			VnPaymentResponseModel? response = _vnPayService.PaymentExecute(Request.Query);

			if (response == null || response.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}

			string? viewString = HttpContext.Session.GetString("ShopDetail1View");
			string? listItemsString = HttpContext.Session.GetString("ListItems");

			if (string.IsNullOrEmpty(viewString) || string.IsNullOrEmpty(listItemsString))
			{
				TempData["Message"] = "Lỗi: Không thể lấy dữ liệu từ session.";
				return RedirectToAction("PaymentFail");
			}

			ShopDetail1View view = JsonConvert.DeserializeObject<ShopDetail1View>(viewString);
			List<CartItemDTO> listItems = JsonConvert.DeserializeObject<List<CartItemDTO>>(listItemsString);

			HttpResponseMessage responseAPI = await _httpClient.PostAsJsonAsync("order/PostVnPay", view.postOrder);
			if (responseAPI.StatusCode == HttpStatusCode.Created)
			{
				CreateOrder(listItems);
			}

			TempData["Message"] = $"Thanh toán Vn Pay thành công";
			return RedirectToAction("PaymentSuccess");

			return View();
		}
	}
}
