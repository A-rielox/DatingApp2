using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
	private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository,IMapper mapper)
	{
		_userRepository = userRepository;
        _mapper = mapper;
    }

	////////////////////////////////////////////////
	///////////////////////////////////////////////////
	// GET: api/Users
	[HttpGet]
	public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
	{
		var members = await _userRepository.GetMembersAsync();

		return Ok(members);
	}

	////////////////////////////////////////////////
	///////////////////////////////////////////////////
	// GET: api/Users/userName
	// [Authorize]
	[HttpGet("{username}")]
	public async Task<ActionResult<MemberDto>> GetUser(string username)
	{
		var member = await _userRepository.GetMemberAsync(username);

		return Ok(member);
	}

	////////////////////////////////////////////////
	///////////////////////////////////////////////////
	// PUT: api/Users
	[HttpPut]
	public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
	{
        // el username lo agarro del token
        // este hace coincidencia con "new Claim(JwtRegisteredClaimNames.NameId, user.UserName)"
		// si debugeo => esta claim tiene el nombre
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		var user = await _userRepository.GetUserByUsernameAsync(username);

		if (user == null) return NotFound();

		//                       ------>
		_mapper.Map(memberUpdateDto, user);

		if(await _userRepository.SaveAllAsync()) return NoContent();

		return BadRequest("Fail to update user.");
	}

	////////////////////////////////////////////////
	///////////////////////////////////////////////////
	// XXXX: api/Users





}
