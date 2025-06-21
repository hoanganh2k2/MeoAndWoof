using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PostgresContext _db;
        public UserController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<UserDTO>> GetUsers()
        {
            List<UserDTO> users = _db.Users.Select(user => new UserDTO
            {
                Userid = user.Userid,
                Username = user.Username,
                Fullname = user.Fullname,
                Gender = user.Gender,
                Email = user.Email,
                Roleid = user.Roleid,
                Userimage = user.Userimage,
                Address = user.Address,
                Sdt = user.Sdt,
                Status = user.Status
            }).ToList();

            return Ok(users);
        }

        [HttpGet("{id:int}", Name = "GetUser")]
        public ActionResult<UserDTO> GetUsers(int id)
        {
            if (id <= 0) return BadRequest();

            UserDTO? user = _db.Users.Where(u => u.Userid == id)
                            .Select(u => new UserDTO
                            {
                                Userid = u.Userid,
                                Username = u.Username,
                                Fullname = u.Fullname,
                                Gender = u.Gender,
                                Email = u.Email,
                                Roleid = u.Roleid,
                                Userimage = u.Userimage,
                                Address = u.Address,
                                Sdt = u.Sdt,
                                Status = u.Status
                            }).FirstOrDefault();

            if (user == null) return NotFound();

            return Ok(user);
        }
        [HttpPost("PostCustomer")]
        public ActionResult<UserDTO> PostCustomer([FromBody] UserDTO userDTO)
        {
            return CreateUser(userDTO, 3);
        }

        [HttpPost("PostCServiceProvider")]
        public ActionResult<UserDTO> PostServiceProvider([FromBody] UserDTO userDTO)
        {
            return CreateUser(userDTO, 2);
        }

        [HttpPost("PostAdmin")]
        public ActionResult<UserDTO> PostAdmin([FromBody] UserDTO userDTO)
        {
            return CreateUser(userDTO, 1);
        }

        private ActionResult<UserDTO> CreateUser(UserDTO userDTO, int role)
        {
            if (userDTO == null) return BadRequest();

            if (_db.Users.FirstOrDefault(u => u.Username == userDTO.Username) != null)
            {
                ModelState.AddModelError(string.Empty, "Username already Exists!");
                return BadRequest(ModelState);
            }
            if (_db.Users.FirstOrDefault(u => u.Email == userDTO.Email) != null)
            {
                ModelState.AddModelError(string.Empty, "Email already Exists!");
                return BadRequest(ModelState);
            }
            if (_db.Users.FirstOrDefault(u => u.Sdt == userDTO.Sdt) != null)
            {
                ModelState.AddModelError(string.Empty, "Phone number already Exists!");
                return BadRequest(ModelState);
            }

            User user = new()
            {
                Userid = userDTO.Userid,
                Username = userDTO.Username,
                Password = userDTO.Password,
                Fullname = userDTO.Fullname,
                Gender = userDTO.Gender,
                Email = userDTO.Email,
                Roleid = role,
                Address = userDTO.Address,
                Sdt = userDTO.Sdt,
                Status = 1
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return CreatedAtRoute("GetUser", new { id = userDTO.Userid }, userDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            User? user = _db.Users.Find(id);
            if (user == null) return NotFound();

            user.Status = 0;

            _db.Users.Update(user);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("recover/{id:int}", Name = "Recover")]
        public IActionResult Recover(int id)
        {
            if (id <= 0) return BadRequest();

            User? user = _db.Users.Find(id);
            if (user == null) return NotFound();

            user.Status = 1;

            _db.Users.Update(user);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] UserDTO userDTO)
        {
            if (userDTO == null || id != userDTO.Userid) return BadRequest();

            User? user = _db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Address = userDTO.Address;
            user.Sdt = userDTO.Sdt;
            user.Gender = userDTO.Gender;
            if (userDTO.Userimage != null && userDTO.Userimage.Length > 0) user.Userimage = userDTO.Userimage;

            _db.Users.Update(user);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}/changepass")]
        public IActionResult ChangePassWord(int id, [FromBody] UserDTO userDTO)
        {
            if (userDTO == null || id != userDTO.Userid) return BadRequest();

            User? user = _db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Password = userDTO.Password;

            _db.Users.Update(user);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpGet("{id:int}/getpass", Name = "GetPass")]
        public ActionResult<UserDTO> GetPass(int id)
        {
            if (id <= 0) return BadRequest();

            UserDTO? user = _db.Users.Where(u => u.Userid == id)
                            .Select(u => new UserDTO
                            {
                                Password = u.Password,
                                Userimage = u.Userimage
                            }).FirstOrDefault();

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("Login")]
        public ActionResult<UserDTO> Login(string username, string pass)
        {
            UserDTO? user = _db.Users.Where(u => u.Username == username && u.Password == pass)
                            .Select(u => new UserDTO
                            {
                                Userid = u.Userid,
                                Username = u.Username,
                                Fullname = u.Fullname,
                                Gender = u.Gender,
                                Email = u.Email,
                                Roleid = u.Roleid,
                                Address = u.Address,
                                Sdt = u.Sdt,
                                Status = u.Status
                            }).FirstOrDefault();

            if (user == null) return NotFound();

            return Ok(user);
        }
        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            User? user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(UserDTO userDTO)
        {
            User? existingUser = await _db.Users.SingleOrDefaultAsync(u => u.Userid == userDTO.Userid);
            if (existingUser == null)
            {
                return NotFound();
            }
            existingUser.Password = userDTO.Password;
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
