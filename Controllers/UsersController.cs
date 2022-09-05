using System.Collections.ObjectModel;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [Authorize]
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