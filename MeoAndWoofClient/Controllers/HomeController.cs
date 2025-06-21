using BusinessObject.Models.DTO;
using MeoAndWoofClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace MeoAndWoofClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        public HomeController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7086/api/")
            };
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"ServiceType");
            List<ServiceTypeDTO> servicetypes = new();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                servicetypes = JsonConvert.DeserializeObject<List<ServiceTypeDTO>>(content);
            }

            return View(servicetypes);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult BlogDetail()
        {
            return View();
        }

        public async Task<IActionResult> Booking()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"ServiceType");
            List<ServiceTypeDTO> servicetypes = new();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                servicetypes = JsonConvert.DeserializeObject<List<ServiceTypeDTO>>(content);
            }

            return View(servicetypes);
        }
        public IActionResult Contact()
        {
            return View();
        }
        public async Task<IActionResult> Services()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"ServiceType");
            List<ServiceTypeDTO> servicetypes = new();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                servicetypes = JsonConvert.DeserializeObject<List<ServiceTypeDTO>>(content);
            }

            return View(servicetypes);
        }
        public IActionResult Price()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}