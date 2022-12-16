using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly DataContext _context;

	public UsersController(DataContext context)
	{
		_context = context;
	}

	////////////////////////////////////////////////
	///////////////////////////////////////////////////
	// GET: api/Users
	[HttpGet]
	public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
	{
		var users = await _context.Users.ToListAsync();

		return Ok(users);
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET: api/Users/userId
    [HttpGet("{id}", Name = "GetUser")]
	public async Task<ActionResult<AppUser>> GetUser(int id)
	{
		var user = await _context.Users.FindAsync(id);

		return Ok(user);
	}

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET: api/Users












    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET: api/Users





}
