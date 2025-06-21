using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly PostgresContext _db;

        public RoleController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RoleDTO>> GetRoles()
        {
            List<RoleDTO> roles = _db.Roles.Select(role => new RoleDTO
            {
                RoleId = role.Roleid,
                RoleName = role.Rolename
            }).ToList();

            return Ok(roles);
        }

        [HttpGet("{id:int}", Name = "GetRole")]
        public ActionResult<RoleDTO> GetRoles(int id)
        {
            if (id <= 0) return BadRequest();

            RoleDTO? roleDTO = _db.Roles.Where(r => r.Roleid == id)
                            .Select(r => new RoleDTO
                            {
                                RoleId = r.Roleid,
                                RoleName = r.Rolename
                            }).FirstOrDefault();

            if (roleDTO == null) return NotFound();

            return Ok(roleDTO);
        }

        [HttpPost]
        public ActionResult<RoleDTO> Post([FromBody] RoleDTO roleDTO)
        {
            if (roleDTO == null) return BadRequest();

            if (_db.Roles.FirstOrDefault(r => r.Rolename == roleDTO.RoleName) != null)
            {
                ModelState.AddModelError("CustomError", "Role name already Exists!");
                return BadRequest(ModelState);
            }

            Role role = new()
            {
                Roleid = roleDTO.RoleId,
                Rolename = roleDTO.RoleName
            };

            _db.Roles.Add(role);
            _db.SaveChanges();

            return CreatedAtRoute("GetRole", new { id = roleDTO.RoleId }, roleDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Role? role = _db.Roles.Find(id);

            if (role == null) return NotFound();

            _db.Roles.Remove(role);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] RoleDTO roleDTO)
        {
            if (roleDTO == null || id != roleDTO.RoleId) return BadRequest();

            Role? role = _db.Roles.Find(id);

            if (role == null) return NotFound();

            role.Rolename = roleDTO.RoleName;

            _db.Roles.Update(role);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
