using BusinessObject.Models;
using Facebook;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace MeoAndWoofClient.Controllers
{
    public class LoginController : Controller
    {
        private readonly PostgresContext _db;

        public LoginController(PostgresContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            IEnumerable<Claim> claims = result.Principal.Identities.FirstOrDefault().Claims;

            string? email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string? fullName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            string? providerKey = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            string? picture = claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;


            if (email == null || providerKey == null)
            {
                return BadRequest("Email or provider key claim not found.");
            }

            User? existingUser = _db.Users.FirstOrDefault(u => u.Email == email);

            if (existingUser == null)
            {
                User newUser = new()
                {
                    Username = email,
                    Fullname = fullName,
                    Email = email,
                    Roleid = 3,
                    Status = 1,
                    Loginprovider = GoogleDefaults.AuthenticationScheme,
                    Password = GenerateRandomPassword(),
                    Address = "",
                    Sdt = "",
                    Userimage = picture,
                    Gender = 1
                };

                _db.Users.Add(newUser);
                _db.SaveChanges();

                ExternalLogin externalLogin = new()
                {
                    Provider = GoogleDefaults.AuthenticationScheme,
                    ProviderKey = providerKey,
                    UserId = newUser.Userid
                };

                _db.ExternalLogins.Add(externalLogin);
                _db.SaveChanges();

                SetSession(newUser);
                HttpContext.Session.SetString("AuthProvider", GoogleDefaults.AuthenticationScheme);

                return RedirectToAction("Information", "User", new { id = newUser.Userid });
            }
            else
            {
                if (!string.IsNullOrEmpty(fullName))
                {
                    existingUser.Fullname = fullName;
                    _db.SaveChanges();
                }

                ExternalLogin? existingExternalLogin = _db.ExternalLogins.FirstOrDefault(el =>
                    el.Provider == GoogleDefaults.AuthenticationScheme && el.ProviderKey == providerKey);

                if (existingExternalLogin == null)
                {
                    ExternalLogin externalLogin = new()
                    {
                        Provider = GoogleDefaults.AuthenticationScheme,
                        ProviderKey = providerKey,
                        UserId = existingUser.Userid
                    };

                    _db.ExternalLogins.Add(externalLogin);
                    _db.SaveChanges();
                }
                SetSession(existingUser);
                HttpContext.Session.SetString("AuthProvider", GoogleDefaults.AuthenticationScheme);

                if (string.IsNullOrEmpty(existingUser.Sdt) || string.IsNullOrEmpty(existingUser.Address) || string.IsNullOrEmpty(existingUser.Userimage))
                {
                    return RedirectToAction("Information", "User", new { id = existingUser.Userid });
                }
            }

            ClaimsIdentity identity = new(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, providerKey));
            identity.AddClaim(new Claim(ClaimTypes.Email, email));
            identity.AddClaim(new Claim(ClaimTypes.Name, fullName));

            ClaimsPrincipal principal = new(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            if (existingUser == null || string.IsNullOrEmpty(existingUser.Userimage) || string.IsNullOrEmpty(existingUser.Sdt) || string.IsNullOrEmpty(existingUser.Address))
            {
                HttpContext.Session.SetString("CompleteProfile", "true");
            }
            return RedirectToAction("Index", "Home");
        }
        private void SetSession(User user)
        {
            HttpContext.Session.SetString("IsAdmin", user.Roleid == 1 ? "true" : "false");
            HttpContext.Session.SetString("IsServiceProvider", user.Roleid == 2 ? "true" : "false");
            HttpContext.Session.SetString("NameUser", user.Fullname ?? string.Empty);
            HttpContext.Session.SetString("PhoneUser", user.Sdt ?? string.Empty);
            HttpContext.Session.SetInt32("UserId", user.Userid);
        }
        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            StringBuilder res = new();
            Random rnd = new();
            while (0 < length--)
            {
                res.Append(validChars[rnd.Next(validChars.Length)]);
            }
            return res.ToString();
        }
        private Uri RediredtUri
        {
            get
            {
                string scheme = Request.Scheme;
                string host = Request.Host.Host;
                int? port = Request.Host.Port;
                string path = Url.Action("FacebookCallback");

                // Log the values for debugging
                Console.WriteLine($"Scheme: {scheme}, Host: {host}, Port: {port}, Path: {path}");

                if (string.IsNullOrEmpty(scheme))
                {
                    throw new UriFormatException("Invalid URI: Scheme is null or empty.");
                }
                if (string.IsNullOrEmpty(host))
                {
                    throw new UriFormatException("Invalid URI: Host is null or empty.");
                }
                if (string.IsNullOrEmpty(path))
                {
                    throw new UriFormatException("Invalid URI: Path is null or empty.");
                }

                UriBuilder uriBuilder = new()
                {
                    Scheme = scheme,
                    Host = host,
                    Path = path
                };

                if (port.HasValue)
                {
                    uriBuilder.Port = port.Value;
                }

                return uriBuilder.Uri;
            }
        }
        public ActionResult Facebook()
        {
            FacebookClient fb = new();
            Uri loginUrl = fb.GetLoginUrl(new
            {
                client_id = "844094604333648",
                client_secret = "0c9c20c2ef9339d79acd4cb14824ef63",
                redirect_uri = RediredtUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public async Task<ActionResult> FacebookCallback(string code)
        {
            FacebookClient fb = new();
            dynamic result = await fb.GetTaskAsync("oauth/access_token", new
            {
                client_id = "844094604333648",
                client_secret = "0c9c20c2ef9339d79acd4cb14824ef63",
                redirect_uri = RediredtUri.AbsoluteUri,
                code = code
            });
            string accessToken = (string)result.access_token;
            HttpContext.Session.SetString("AccessToken", accessToken);
            try
            {
                dynamic me = await fb.GetTaskAsync("me?fields=email,last_name,first_name,picture,gender", new { access_token = accessToken });
                string email = me.email;
                string firstname = me.first_name;
                string lastname = me.last_name;
                string pictureUrl = me.picture?.data?.url;

                User existingUser = _db.Users.FirstOrDefault(u => u.Email == email);

                if (existingUser == null)
                {
                    User newUser = new()
                    {
                        Username = email,
                        Fullname = $"{firstname} {lastname}",
                        Email = email,
                        Roleid = 3,
                        Status = 1,
                        Loginprovider = "FaceBook",
                        Password = GenerateRandomPassword(),
                        Address = "",
                        Sdt = "",
                        Userimage = pictureUrl,
                        Gender = 1
                    };

                    _db.Users.Add(newUser);
                    _db.SaveChanges();

                    ExternalLogin externalLogin = new()
                    {
                        Provider = FacebookDefaults.AuthenticationScheme,
                        ProviderKey = "FaceBook",
                        UserId = newUser.Userid
                    };

                    _db.ExternalLogins.Add(externalLogin);
                    _db.SaveChanges();

                    SetSession(newUser);
                    HttpContext.Session.SetString("AuthProvider", "FaceBook");
                    return RedirectToAction("Information", "User", new { id = newUser.Userid });
                }
                else
                {
                    SetSession(existingUser);
                    HttpContext.Session.SetString("AuthProvider", "FaceBook");
                    return RedirectToAction("Information", "User", new { id = existingUser.Userid });
                }

                return RedirectToAction("Index", "Home");
            }
            catch (FacebookOAuthException ex)
            {
                Console.WriteLine($"Facebook API error: {ex.Message}");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return RedirectToAction("Index", "Error");
            }
        }
    }
}


