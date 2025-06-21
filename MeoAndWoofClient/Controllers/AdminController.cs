using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MeoAndWoofClient.Controllers
{
	public class AdminController : Controller
	{
		private readonly HttpClient _httpClient;

		public AdminController(HttpClient httpClient)
		{
			_httpClient = httpClient;
			_httpClient.BaseAddress = new Uri("https://localhost:7086/api/");
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			//localhost:7086/api/service/filter?userId=2&serviceTypeId=2
		}
		private IActionResult Filter()
		{
			string? admin = HttpContext.Session.GetString("IsAdmin");
			if (admin != "true") return RedirectToAction("Login", "Authen");

			return null;
		}

		public IActionResult Chart()
		{
			return View();
		}
		public IActionResult Chat()
		{
			return View();
		}
		public async Task<IActionResult> CuaHangThuCung()
		{
			IActionResult filterResult = Filter();
			if (filterResult != null) return filterResult;

			int serviceTypeId = 1;

			HttpResponseMessage response = await _httpClient.GetAsync($"service?$filter=serviceTypeId eq {serviceTypeId}");

			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				List<ServiceDTO> services = JsonConvert.DeserializeObject<List<ServiceDTO>>(content);
				HttpContext.Session.SetInt32("ServiceId", services.FirstOrDefault()?.Serviceid ?? 0);
				return View(services);
			}

			return View(new List<ServiceDTO>());
		}
		public async Task<IActionResult> LuuTruThuCung()
		{
			IActionResult filterResult = Filter();
			if (filterResult != null) return filterResult;

			int serviceTypeId = 2;

			HttpResponseMessage response = await _httpClient.GetAsync($"service?$filter=serviceTypeId eq {serviceTypeId}");

			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				List<ServiceDTO> services = JsonConvert.DeserializeObject<List<ServiceDTO>>(content);
				HttpContext.Session.SetInt32("ServiceId", services.FirstOrDefault()?.Serviceid ?? 0);
				return View(services);
			}

			return View(new List<ServiceDTO>());
		}
		public IActionResult Dashboard()
		{
			return View();
		}
		public IActionResult StoreProduct()
		{
			return View();
		}
	}
}
