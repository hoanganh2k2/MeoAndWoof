using BusinessObject.Models;
using BusinessObject.Models.DTO;
using BusinessObject.ObjectView;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
namespace MeoAndWoofClient.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UserController(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7086/api/")
            };
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }
        private int? GetUserId() => HttpContext.Session.GetInt32("UserId");
        public async Task<IActionResult> ChangePass(int id)
        {
            HttpResponseMessage userResponse = await _httpClient.GetAsync($"user/{id}");
            User user = new();
            if (userResponse.StatusCode == HttpStatusCode.OK)
            {
                string userContent = await userResponse.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<User>(userContent);
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePass(User user, int id, [FromForm] string OldPassword, [FromForm] string Password, [FromForm] string RePassword)
        {
            ModelState.Remove("Sdt");
            ModelState.Remove("Username");
            ModelState.Remove("Fullname");
            ModelState.Remove("Address");
            ModelState.Remove("Email");
            ModelState.Remove("Role");
            HttpResponseMessage userResponse = await _httpClient.GetAsync($"user/{id}/getpass");
            User userDTO = new();
            if (userResponse.StatusCode == HttpStatusCode.OK)
            {
                string userContent = await userResponse.Content.ReadAsStringAsync();
                userDTO = JsonConvert.DeserializeObject<User>(userContent);
                if (userDTO.Password != OldPassword)
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu hiện tại không chính xác");
                    return View(userDTO);
                }
                ViewBag.OldPassword = OldPassword;
                if (ModelState.IsValid)
                {
                    if (Password != RePassword)
                    {
                        ModelState.AddModelError(string.Empty, "Mật khẩu không khớp");
                        return View(userDTO);
                    }
                    userDTO.Password = Password;
                    userDTO.Userid = id;
                    HttpResponseMessage changePassResponse = await _httpClient.PutAsJsonAsync($"user/{id}/changepass", userDTO);
                    if (changePassResponse.StatusCode == HttpStatusCode.OK)
                    {
                        ModelState.Clear();
                        return Ok("User information updated successfully.");
                    }
                    else
                    {
                        return StatusCode((int)changePassResponse.StatusCode, $"Failed to update user. Status code: {changePassResponse.StatusCode}");
                    }
                }
            }

            return View(userDTO);
        }

        public async Task<IActionResult> HistoryBooking(int id)
        {
            HttpResponseMessage userResponse = await _httpClient.GetAsync($"booking?$filter=userid eq {id}");
            List<BookingDTO> bookingDTOs = new();
            if (userResponse.StatusCode == HttpStatusCode.OK)
            {
                string userContent = await userResponse.Content.ReadAsStringAsync();
                bookingDTOs = JsonConvert.DeserializeObject<List<BookingDTO>>(userContent);
            }

            return View(bookingDTOs);
        }
        public async Task<IActionResult> HistoryStore(int id)
        {
            HttpResponseMessage userResponse = await _httpClient.GetAsync($"order?$filter=userid eq {id}");
            List<OrderDTO> orderDTOs = new();
            if (userResponse.StatusCode == HttpStatusCode.OK)
            {
                string userContent = await userResponse.Content.ReadAsStringAsync();
                orderDTOs = JsonConvert.DeserializeObject<List<OrderDTO>>(userContent);
            }
            orderDTOs = orderDTOs.OrderByDescending(o => o.OrderId).ToList();
            return View(orderDTOs);
        }
        public async Task<IActionResult> Information(int id)
        {
            HttpResponseMessage userResponse = await _httpClient.GetAsync($"user/{id}");
            HttpResponseMessage petsResponse = await _httpClient.GetAsync($"pet?$filter=userid eq {id}");
            MyInforView view = new();
            if (userResponse.StatusCode == HttpStatusCode.OK)
            {
                string userContent = await userResponse.Content.ReadAsStringAsync();
                UserDTO userDTO = JsonConvert.DeserializeObject<UserDTO>(userContent);
                view.userDTO = userDTO;
            }
            if (petsResponse.StatusCode == HttpStatusCode.OK)
            {
                string petContent = await petsResponse.Content.ReadAsStringAsync();
                List<PetDTO> PetDTOs = JsonConvert.DeserializeObject<List<PetDTO>>(petContent);
                view.petDTOs = PetDTOs;
            }

            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> Information(UserDTO userDTO, int id, IFormFile Userimage, string OldUserImage)
        {
            userDTO.Userid = id;

            if (Userimage != null && Userimage.Length > 0)
            {
                try
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img", "avatar");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Userimage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!string.IsNullOrEmpty(OldUserImage))
                    {
                        string oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, OldUserImage.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    using (FileStream fileStream = new(filePath, FileMode.Create))
                    {
                        await Userimage.CopyToAsync(fileStream);
                    }
                    userDTO.Userimage = "/img/avatar/" + uniqueFileName;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }

            }
            HttpResponseMessage userResponse = await _httpClient.PutAsJsonAsync($"user/{id}", userDTO);

            if (userResponse.StatusCode == HttpStatusCode.OK || userResponse.StatusCode == HttpStatusCode.NoContent)
            {
                HttpContext.Session.SetString("CompleteProfile", "false");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Failed to update user. Status code: {userResponse.StatusCode}");
                return View(userDTO);
            }
        }
        public async Task<IActionResult> EditPet(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"pet/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string petContent = await response.Content.ReadAsStringAsync();
                PetDTO petDTO = JsonConvert.DeserializeObject<PetDTO>(petContent);

                HttpResponseMessage petTypesResponse = await _httpClient.GetAsync("pettype");
                if (petTypesResponse.IsSuccessStatusCode)
                {
                    string petTypesData = await petTypesResponse.Content.ReadAsStringAsync();
                    List<PetTypeDTO> petTypes = JsonConvert.DeserializeObject<List<PetTypeDTO>>(petTypesData);
                    ViewBag.PetTypes = petTypes;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách loại thú cưng.");
                }

                return View(petDTO);
            }
            else
            {
                string errorMessage = $"Failed to retrieve pet. Status code: {response.StatusCode}";
                return StatusCode((int)response.StatusCode, errorMessage);
            }
        }
        public async Task<IActionResult> CreatePet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["UserId"] = userId;
            HttpResponseMessage response = await _httpClient.GetAsync("pettype");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                List<PetTypeDTO> petTypes = JsonConvert.DeserializeObject<List<PetTypeDTO>>(data);

                PetView petView = new()
                {
                    PetTypes = petTypes
                };

                return View(petView);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi lấy danh sách loại thú cưng.");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePet(int id)
        {
            if (id <= 0) return BadRequest("Invalid pet ID.");

            HttpResponseMessage response = await _httpClient.DeleteAsync($"pet/{id}");

            if (response.StatusCode == HttpStatusCode.NoContent && response.IsSuccessStatusCode)
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                return RedirectToAction("Information", new { id = userId });
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound("Pet not found.");
            }
            else
            {
                return StatusCode((int)response.StatusCode, $"Failed to delete pet. Status code: {response.StatusCode}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePet(PetDTO petDTO, IFormFile Petimage, int PetTypeId)
        {
            try
            {
                if (petDTO == null)
                {
                    ModelState.AddModelError("", "Dữ liệu thú cưng không hợp lệ.");
                    return View();
                }

                if (Petimage != null && Petimage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img", "pets");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Petimage.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (FileStream fileStream = new(filePath, FileMode.Create))
                    {
                        await Petimage.CopyToAsync(fileStream);
                    }

                    petDTO.Petimage = "/img/pets/" + uniqueFileName;
                }
                petDTO.Pettypeid = PetTypeId;
                string jsonString = JsonConvert.SerializeObject(petDTO);
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("https://localhost:7086/api/pet", petDTO);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Information", new { id = petDTO.Userid });
                }
                else
                {
                    ModelState.AddModelError("", $"{response.StatusCode}");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"{ex.Message}");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPet(PetDTO petDTO, IFormFile Petimage, int PetTypeId, string OldImage)
        {
            try
            {
                if (petDTO == null)
                {
                    ModelState.AddModelError("", "Dữ liệu thú cưng không hợp lệ.");
                    return View();
                }

                if (Petimage != null && Petimage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img", "pets");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Petimage.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!string.IsNullOrEmpty(OldImage))
                    {
                        string oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, OldImage.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    using (FileStream fileStream = new(filePath, FileMode.Create))
                    {
                        await Petimage.CopyToAsync(fileStream);
                    }

                    petDTO.Petimage = "/img/pets/" + uniqueFileName;
                }
                else { petDTO.Petimage = OldImage; }
                petDTO.Pettypeid = PetTypeId;
                string jsonString = JsonConvert.SerializeObject(petDTO);
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("https://localhost:7086/api/pet/" + petDTO.Petid, petDTO);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Information", new { id = GetUserId() });
                }
                else
                {
                    ModelState.AddModelError("", $"{response.StatusCode}");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"{ex.Message}");
                return View();
            }
        }
        public async Task<List<PetDTO>> GetPet()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("pet");
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            List<PetDTO>? pet = JsonConvert.DeserializeObject<List<PetDTO>>(content);

            return pet;
        }

    }
}



