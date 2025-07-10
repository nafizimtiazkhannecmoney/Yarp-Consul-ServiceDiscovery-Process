using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Model;
using UserService.Repository;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        private readonly AppDbContext _DbContext;
        private readonly UserRepository _repo;

        public userController(AppDbContext DbContext, UserRepository repo)
        {
            this._DbContext = DbContext;
            _repo = repo;
        }
        // GET /api/tusers
        [HttpGet("TestUserGetAll-----")]
        public async Task<ActionResult<IEnumerable<TUser>>> GetAll()
        {
            var list = await _DbContext.TUsers.OrderBy(u => u.FirstName).ToListAsync();
            return Ok(list);
        }

        [HttpGet("TestUserGetbyid-----")]
        public async Task<ActionResult<IEnumerable<TUser>>> GetAllbyid()
        {
            var list = await _DbContext.TUsers.Where(u => u.IdUserKey == 100004).FirstOrDefaultAsync();
            return Ok(list);
        }

        //-----------------------------------------------//
        [HttpGet]
        public async Task<IActionResult> All()
        => Ok(await _repo.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ById(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(LoginRequestDto dto)
        {
            var user = await _repo.SignInAsync(dto.Username, dto.Password);
            return user is null ? Unauthorized() : Ok(user);
        }

    }
}
