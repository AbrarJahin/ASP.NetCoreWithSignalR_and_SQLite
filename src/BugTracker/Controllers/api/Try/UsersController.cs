using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly McpDbContext _context;

        public UsersController(McpDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Get()
        {
            var users = await _context.Users
                .Include(u => u.Posts)
                .ToArrayAsync();

            var response = users.Select(u => new
            {
                firstName = u.FirstName,
                lastName = u.LastName,
                posts = u.Posts.Select(p => p.Content)
            });

            return Ok(response);
        }
    }
}
