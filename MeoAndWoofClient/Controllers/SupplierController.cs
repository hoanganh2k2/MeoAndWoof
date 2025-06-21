using BusinessObject.Models;
using BusinessObject.Models.DTO;
using BusinessObject.ObjectView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MeoAndWoofClient.Controllers
{
    public class SupplierController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly PostgresContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public SupplierController(HttpClient httpClient, PostgresContext db, IWebHostEnvironment hostEnvironment)
        {
            _httpClient = httpClient;
            _db = db;
            _hostEnvironment = hostEnvironment;
            _httpClient.BaseAddress = new Uri("https://localhost:7086/api/");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //localhost:7086/api/service/filter?userId=2&serviceTypeId=2
        }
        private IActionResult Filter()
        {
            string? admin = HttpContext.Session.GetString("IsServiceProvider");
            if (admin != "true") return RedirectToAction("Login", "Authen");

            return null;
        }
        private int? GetUserId() => HttpContext.Session.GetInt32("UserId");
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Confirm(int id, int status)
        {
            IActionResult filterResult = Filter();
            if (filterResult != null) return filterResult;

            HttpResponseMessage response = await _httpClient.GetAsync($"order/status/{id}/{status}");

            return RedirectToAction("ListOrder", "Supplier");
        }
        public async Task<IActionResult> OrderDetail(int id)
        {
            IActionResult filterResult = Filter();
            if (filterResult != null) return filterResult;

            OrderDetailView view = new();


            HttpResponseMessage orderResponse = await _httpClient.GetAsync($"order/{id}");
            if (orderResponse.IsSuccessStatusCode)
            {
                string orderContent = await orderResponse.Content.ReadAsStringAsync();
                OrderDTO order = JsonConvert.DeserializeObject<OrderDTO>(orderContent);
                view.orderDTO = order;
            }
            else
            {
                return StatusCode((int)orderResponse.StatusCode);
            }

            HttpResponseMessage response = await _httpClient.GetAsync($"order/details/{id}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                view.orderDetailDTOs = JsonConvert.DeserializeObject<List<OrderDetailDTO>>(content);
            }

            ViewBag.Id = id;
            return View(view);
        }
        public async Task<IActionResult> ListBooking()
        {
            IActionResult filterResult = Filter();
            if (filterResult != null) return filterResult;

            ListBooking view = new();

            HttpResponseMessage response = await _httpClient.GetAsync($"booking?$filter=service/userid eq {GetUserId()}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                List<BookingDTO> bookingDTOs = JsonConvert.DeserializeObject<List<BookingDTO>>(content);
                bookingDTOs = bookingDTOs.OrderBy(b => b.Statuspaid).ToList();
                view.bookingDTOs = bookingDTOs;
            }

            return View(view);
        }
        public async Task<IActionResult> ListOrder()
        {
            IActionResult filterResult = Filter();
            if (filterResult != null) return filterResult;

            ListOrder view = new();

            HttpResponseMessage response = await _httpClient.GetAsync($"order/supplier/{GetUserId()}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                List<OrderDTO> orderDTOs = JsonConvert.DeserializeObject<List<OrderDTO>>(content);
                orderDTOs = orderDTOs.OrderByDescending(b => b.OrderId).ToList();
                view.orderDTOs = orderDTOs;
            }

            return View(view);
        }
        public async Task<IActionResult> Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                HttpResponseMessage response1 = await _httpClient.GetAsync($"service/count?userId={userId}&serviceTypeId=1");

                if (response1.IsSuccessStatusCode)
                {
                    string content1 = await response1.Content.ReadAsStringAsync();
                    int dashboardData1 = JsonConvert.DeserializeObject<int>(content1);

                    ViewData["DashboardData1"] = dashboardData1;
                }
                else
                {
                    ViewData["DashboardData1"] = null;
                }

                HttpResponseMessage response2 = await _httpClient.GetAsync($"service/count?userId={userId}&serviceTypeId=2");

                if (response2.IsSuccessStatusCode)
                {
                    string content2 = await response2.Content.ReadAsStringAsync();
                    int dashboardData2 = JsonConvert.DeserializeObject<int>(content2);

                    ViewData["DashboardData2"] = dashboardData2;
                }
                else
                {
                    ViewData["DashboardData2"] = null;
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewData["DashboardData1"] = null;
                ViewData["DashboardData2"] = null;
                return RedirectToAction("Error");
            }
        }
        public IActionResult Chart()
        {
            return View();
        }
        public async Task<IActionResult> CuaHangThuCung()
        {
            IActionResult filterResult = Filter();
            if (filterResult != null) return filterResult;

            int? userId = HttpContext.Session.GetInt32("UserId");


            int serviceTypeId = 1;

            HttpResponseMessage response = await _httpClient.GetAsync($"service/filter?userId={userId}&serviceTypeId={serviceTypeId}");

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

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int serviceTypeId = 2;

            HttpResponseMessage response = await _httpClient.GetAsync($"service/filter?userId={userId}&serviceTypeId={serviceTypeId}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                List<ServiceDTO> services = JsonConvert.DeserializeObject<List<ServiceDTO>>(content);
                HttpContext.Session.SetInt32("ServiceId", services.FirstOrDefault()?.Serviceid ?? 0);
                return View(services);
            }

            return View(new List<ServiceDTO>());
        }
        public async Task<IActionResult> ViewProducts(int serviceId)
        {
            HttpContext.Session.Remove("ServiceId");
            HttpContext.Session.SetInt32("ServiceId", serviceId);
            HttpResponseMessage response = await _httpClient.GetAsync($"product/filter?serviceid={serviceId}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                List<ServicestoreDTO> products = JsonConvert.DeserializeObject<List<ServicestoreDTO>>(content);

                return View("ProductCuaHangThuCung", products);
            }

            return View("ProductCuaHangThuCung", new List<ServicestoreDTO>());
        }

        public async Task<IActionResult> DeleteProductCuaHangThuCung(int productId, int serviceId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"product/{productId}/{serviceId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ViewProducts", new { serviceId = serviceId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        public async Task<IActionResult> DeleteProductLuuTruThucung(int productId, int serviceId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"product/{productId}/{serviceId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ViewProducts", new { serviceId = serviceId });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> DeleteCuaHangThuCung(int serviceId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"service?id={serviceId}");


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("CuaHangThuCung", "Supplier");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> DeleteLuuTruThuCung(int serviceId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"service?id={serviceId}");


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("LuuTruThuCung", "Supplier");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


        public async Task<IActionResult> CreateProductCuaHangThuCung()
        {
            int? serviceId = HttpContext.Session.GetInt32("ServiceId");

            if (serviceId != null)
            {
                ViewBag.ServiceId = serviceId;
            }

            HttpResponseMessage categoryResponse = await _httpClient.GetAsync("product/categories");

            if (categoryResponse.IsSuccessStatusCode)
            {
                string categoryContent = await categoryResponse.Content.ReadAsStringAsync();
                List<Productcategory> categories = JsonConvert.DeserializeObject<List<Productcategory>>(categoryContent);
                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Categories = new List<Productcategory>();
            }

            return View();
        }
        public async Task<IActionResult> CreateProductLuuTruThuCung()
        {
            int? serviceId = HttpContext.Session.GetInt32("ServiceId");

            if (serviceId != null)
            {
                ViewBag.ServiceId = serviceId;
            }

            HttpResponseMessage categoryResponse = await _httpClient.GetAsync("product/categories");

            if (categoryResponse.IsSuccessStatusCode)
            {
                string categoryContent = await categoryResponse.Content.ReadAsStringAsync();
                List<Productcategory> categories = JsonConvert.DeserializeObject<List<Productcategory>>(categoryContent);
                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Categories = new List<Productcategory>();
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProductCuaHangThuCung(ServicestoreDTO servicestoreDTO)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("product", servicestoreDTO);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ServicestoreDTO createdProduct = JsonConvert.DeserializeObject<ServicestoreDTO>(apiResponse);

                    return RedirectToAction("ViewProducts", new { serviceId = createdProduct.Serviceid });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    return RedirectToAction("CreateProductCuaHangThuCung", new { serviceId = servicestoreDTO.Serviceid });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                return RedirectToAction("CreateProductCuaHangThuCung", new { serviceId = servicestoreDTO.Serviceid });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateProductLuuTruThuCung(ServicestoreDTO servicestoreDTO)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("product", servicestoreDTO);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ServicestoreDTO createdProduct = JsonConvert.DeserializeObject<ServicestoreDTO>(apiResponse);

                    return RedirectToAction("ViewProducts", new { serviceId = createdProduct.Serviceid });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    return RedirectToAction("CreateProductLuuTruThuCung", new { serviceId = servicestoreDTO.Serviceid });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                return RedirectToAction("CreateProductLuuTruThuCung", new { serviceId = servicestoreDTO.Serviceid });
            }
        }
        private async Task<ServicestoreDTO> GetProductById(int productId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"product/{productId}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                ServicestoreDTO product = JsonConvert.DeserializeObject<ServicestoreDTO>(content);
                return product;
            }

            return null;
        }
        public async Task<IActionResult> EditProductCuaHangThuCung(int productId)
        {
            int? serviceId = HttpContext.Session.GetInt32("ServiceId");

            if (serviceId != null)
            {
                ViewBag.ServiceId = serviceId;
            }

            HttpResponseMessage categoryResponse = await _httpClient.GetAsync("product/categories");

            if (categoryResponse.IsSuccessStatusCode)
            {
                string categoryContent = await categoryResponse.Content.ReadAsStringAsync();
                List<Productcategory> categories = JsonConvert.DeserializeObject<List<Productcategory>>(categoryContent);
                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Categories = new List<Productcategory>();
            }

            ServicestoreDTO product = await GetProductById(productId);

            if (product != null)
            {
                return View(product);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return View();
            }
        }
        public async Task<IActionResult> EditProductLuuTruThuCung(int productId)
        {
            int? serviceId = HttpContext.Session.GetInt32("ServiceId");

            if (serviceId != null)
            {
                ViewBag.ServiceId = serviceId;
            }

            HttpResponseMessage categoryResponse = await _httpClient.GetAsync("product/categories");

            if (categoryResponse.IsSuccessStatusCode)
            {
                string categoryContent = await categoryResponse.Content.ReadAsStringAsync();
                List<Productcategory> categories = JsonConvert.DeserializeObject<List<Productcategory>>(categoryContent);
                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Categories = new List<Productcategory>();
            }

            ServicestoreDTO product = await GetProductById(productId);

            if (product != null)
            {
                return View(product);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> PutCuaHangThuCung(int id, ServicestoreDTO servicestoreDTO)
        {

            if (servicestoreDTO == null || id != servicestoreDTO.Productid)
            {
                return BadRequest();
            }

            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"product/{id}", servicestoreDTO);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ViewProducts", new { serviceId = servicestoreDTO.Serviceid });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to update product. Please try again later.");
                return RedirectToAction("EditProductCuaHangThuCung", new { productId = servicestoreDTO.Productid });
            }
        }
        [HttpPost]
        public async Task<IActionResult> PutLuuTruThuCung(int id, ServicestoreDTO servicestoreDTO)
        {

            if (servicestoreDTO == null || id != servicestoreDTO.Productid)
            {
                return BadRequest();
            }

            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"product/{id}", servicestoreDTO);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ViewProducts", new { serviceId = servicestoreDTO.Serviceid });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to update product. Please try again later.");
                return RedirectToAction("EditProductLuuTruThuCung", new { productId = servicestoreDTO.Productid });
            }
        }

        public async Task<IActionResult> Chat()
        {
            IActionResult filterResult = Filter();
            if (filterResult != null) return filterResult;

            ChatSupplier view = new();

            HttpResponseMessage response = await _httpClient.GetAsync($"booking?$filter=service/userid eq {GetUserId()}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                List<BookingDTO> bookingDTOs = JsonConvert.DeserializeObject<List<BookingDTO>>(content);
                bookingDTOs = bookingDTOs.OrderBy(b => b.Statuspaid).ToList();
                view.bookingDTOs = bookingDTOs;
            }

            return View(view);
        }
        public IActionResult StoreProduct()
        {
            return View();
        }
        public async Task<IActionResult> CreateCuaHangThuCungAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                ViewBag.UserId = userId;
            }
            HttpResponseMessage petTypesResponse = await _httpClient.GetAsync("pettype");

            if (petTypesResponse.IsSuccessStatusCode)
            {
                string petTypesData = await petTypesResponse.Content.ReadAsStringAsync();
                List<PetTypeDTO> petTypes = JsonConvert.DeserializeObject<List<PetTypeDTO>>(petTypesData);
                ViewBag.PetTypes = petTypes;
            }
            else
            {
                ;
                ViewBag.PetTypes = new List<PetTypeDTO>();
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCuaHangThuCung(ServiceDTO serviceDTO, List<IFormFile> images, List<int> PetTypeIds)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return BadRequest("UserId not found in session");
            }

            serviceDTO.Userid = userId.Value;
            int serviceTypeId = 1;

            Service service = new()
            {
                Servicename = serviceDTO.Servicename,
                Servicetypeid = serviceTypeId,
                Description = serviceDTO.Description,
                Userid = serviceDTO.Userid,
                Address = serviceDTO.Address,
                Status = 1
            };

            _db.Services.Add(service);
            _db.SaveChanges();

            foreach (IFormFile image in images)
            {
                if (image.Length > 0)
                {
                    string imageUrl = await SaveImageAndGetUrlAsync(image);

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        Serviceimage serviceImage = new()
                        {
                            Imagetxt = imageUrl,
                            Serviceid = service.Serviceid
                        };

                        _db.Serviceimages.Add(serviceImage);
                    }
                }
            }

            _db.SaveChanges();
            foreach (int petTypeId in PetTypeIds)
            {
                Serviceprice price = new()
                {
                    Serviceid = service.Serviceid,
                    Pettypeid = petTypeId,
                    Startdate = serviceDTO.Serviceprice.Startdate,
                    Enddate = serviceDTO.Serviceprice.Enddate,
                    Numberprice = serviceDTO.Serviceprice.Numberprice ?? 0,
                };

                _db.Serviceprices.Add(price);
            }

            _db.SaveChanges();
            List<Servicepettype> servicePetTypes = PetTypeIds.Select(petTypeId => new Servicepettype
            {
                Serviceid = service.Serviceid,
                Pettypeid = petTypeId
            }).ToList();

            _db.Servicepettypes.AddRange(servicePetTypes);
            _db.SaveChanges();

            return RedirectToAction("CuaHangThuCung", "Supplier");
        }
        private async Task<string> SaveImageAndGetUrlAsync(IFormFile image)
        {
            if (image == null || image.Length <= 0)
            {
                return null;
            }

            string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img", "store");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (FileStream fileStream = new(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            string imageUrl = "/img/store/" + uniqueFileName;

            return imageUrl;
        }
        public async Task<IActionResult> CreateLuuTruThuCung()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                ViewBag.UserId = userId;
            }
            HttpResponseMessage petTypesResponse = await _httpClient.GetAsync("pettype");

            if (petTypesResponse.IsSuccessStatusCode)
            {
                string petTypesData = await petTypesResponse.Content.ReadAsStringAsync();
                List<PetTypeDTO> petTypes = JsonConvert.DeserializeObject<List<PetTypeDTO>>(petTypesData);
                ViewBag.PetTypes = petTypes;
            }
            else
            {
                ;
                ViewBag.PetTypes = new List<PetTypeDTO>();
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateLuuTruThuCung(ServiceDTO serviceDTO, List<IFormFile> images, List<int> PetTypeIds)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return BadRequest("UserId not found in session");
            }

            serviceDTO.Userid = userId.Value;
            int serviceTypeId = 2;

            Service service = new()
            {
                Servicename = serviceDTO.Servicename,
                Servicetypeid = serviceTypeId,
                Description = serviceDTO.Description,
                Userid = serviceDTO.Userid,
                Address = serviceDTO.Address,
                Status = 1
            };

            _db.Services.Add(service);
            _db.SaveChanges();

            foreach (IFormFile image in images)
            {
                if (image.Length > 0)
                {
                    string imageUrl = await SaveImageAndGetUrlAsync(image);

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        Serviceimage serviceImage = new()
                        {
                            Imagetxt = imageUrl,
                            Serviceid = service.Serviceid
                        };

                        _db.Serviceimages.Add(serviceImage);
                    }
                }
            }

            _db.SaveChanges();
            foreach (int petTypeId in PetTypeIds)
            {
                Serviceprice price = new()
                {
                    Serviceid = service.Serviceid,
                    Pettypeid = petTypeId,
                    Startdate = serviceDTO.Serviceprice.Startdate,
                    Enddate = serviceDTO.Serviceprice.Enddate,
                    Numberprice = serviceDTO.Serviceprice.Numberprice ?? 0,
                };

                _db.Serviceprices.Add(price);
            }

            _db.SaveChanges();
            List<Servicepettype> servicePetTypes = PetTypeIds.Select(petTypeId => new Servicepettype
            {
                Serviceid = service.Serviceid,
                Pettypeid = petTypeId
            }).ToList();

            _db.Servicepettypes.AddRange(servicePetTypes);
            _db.SaveChanges();

            return RedirectToAction("LuuTruThuCung", "Supplier");
        }
    }
}
