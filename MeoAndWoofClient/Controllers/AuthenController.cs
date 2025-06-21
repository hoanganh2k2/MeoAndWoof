using BusinessObject.Models;
using BusinessObject.ObjectView;
using MeoAndWoofClient.Email;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace MeoAndWoofClient.Controllers
{
    public class AuthenController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public AuthenController(IConfiguration configuration, IEmailService emailService)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7086/api/")
            };
            _configuration = configuration;
            _emailService = emailService;
        }
        public IActionResult Login()
        {
            return View();
        }

        // POST: Members/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError(string.Empty, "Username and Password are required.");
                return View(user);
            }

            HttpResponseMessage response = await _httpClient.GetAsync($"user/Login?username={user.Username}&pass={user.Password}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                User? loggedInUser = JsonConvert.DeserializeObject<User>(content);

                if (loggedInUser.Roleid == 1)
                {
                    HttpContext.Session.SetString("IsAdmin", "true");
                    HttpContext.Session.SetString("NameUser", loggedInUser.Fullname);
                    HttpContext.Session.SetString("PhoneUser", loggedInUser.Sdt);
                    HttpContext.Session.SetInt32("UserId", loggedInUser.Userid);
                    return RedirectToAction("Dashboard", "Admin");

                }
                else if (loggedInUser.Roleid == 2)
                {
                    HttpContext.Session.SetString("IsServiceProvider", "true");
                    HttpContext.Session.SetString("NameUser", loggedInUser.Fullname);
                    HttpContext.Session.SetString("PhoneUser", loggedInUser.Sdt);
                    HttpContext.Session.SetInt32("UserId", loggedInUser.Userid);
                    return RedirectToAction("Dashboard", "Supplier");

                }
                else
                {
                    HttpContext.Session.SetString("NameUser", loggedInUser.Fullname);
                    HttpContext.Session.SetString("PhoneUser", loggedInUser.Sdt);
                    HttpContext.Session.SetInt32("UserId", loggedInUser.Userid);
                    return RedirectToAction("Index", "Home");

                }
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Invalid Username or password. Please try again.");
            }

            return View(user);
        }

        public IActionResult Register()
        {
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user, [FromForm] string RePassword)
        {
            ModelState.Remove("Role");
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                if (user.Password != RePassword)
                {
                    ModelState.AddModelError(string.Empty, "Passwords do not match!");
                    return View(user);
                }

                HttpResponseMessage response;
                if (user.Roleid == 3)
                {
                    response = await _httpClient.PostAsJsonAsync("user/PostCustomer", user);
                }
                else if (user.Roleid == 2)
                {
                    response = await _httpClient.PostAsJsonAsync("user/PostServiceProvider", user);
                    HttpContext.Session.SetString("IsServiceProvider", "true");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid role selected.");
                    return View(user);
                }

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    HttpContext.Session.SetString("NameUser", user.Fullname);
                    HttpContext.Session.SetString("PhoneUser", user.Sdt);
                    HttpContext.Session.SetInt32("UserId", user.Userid);

                    return RedirectToAction("Index", "Home");
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Dictionary<string, string[]>? modelState = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(responseContent);
                    if (modelState != null)
                        foreach (KeyValuePair<string, string[]> kvp in modelState)
                            foreach (string error in kvp.Value)
                                ModelState.AddModelError(kvp.Key, error);
                }
            }
            return View(user);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([Bind("Email")] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is required.");
                return View(user);
            }

            HttpResponseMessage response = await _httpClient.GetAsync($"user/GetByEmail?email={user.Email}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                User existingUser = JsonConvert.DeserializeObject<User>(content);

                if (existingUser != null)
                {
                    string otp = GenerateOtp();
                    MeoAndWoofClient.Email.Message message = new(new string[] { user.Email }, "Your OTP Code", $"Your OTP code is {otp}");
                    _emailService.SendEmail(message);

                    TempData["Email"] = user.Email;
                    return RedirectToAction("ResetPassword");
                }
            }

            ModelState.AddModelError(string.Empty, "Email not found. Please try again.");
            return View(user);
        }

        public IActionResult ResetPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = TempData["Email"] as string;
                if (email == null)
                {
                    ModelState.AddModelError(string.Empty, "Email verification failed. Please try again.");
                    return View(model);
                }

                HttpResponseMessage response = await _httpClient.GetAsync($"user/GetByEmail?email={email}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    User user = JsonConvert.DeserializeObject<User>(content);

                    if (user != null && VerifyOtp(user.Email, model.Otp))
                    {
                        user.Password = model.NewPassword;
                        HttpResponseMessage updateResponse = await _httpClient.PutAsJsonAsync("user/UpdatePassword", user);

                        if (updateResponse.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("Login");
                        }
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid OTP or other error.");
            return View(model);
        }

        private string GenerateOtp()
        {
            Random rand = new();
            return rand.Next(100000, 999999).ToString();
        }

        private bool VerifyOtp(string email, string otp)
        {
            return true;
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}



