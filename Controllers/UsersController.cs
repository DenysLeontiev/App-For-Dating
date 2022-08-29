using System.Collections.ObjectModel;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route(template: "api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet(template: "{id}")] //api/users/3
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            AppUser user = await _context.Users.FindAsync(id);

            if(user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }
    }
}