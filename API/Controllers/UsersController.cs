using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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
	//[Authorize]
	[HttpGet("{username}")]
	public async Task<ActionResult<MemberDto>> GetUser(string username)
	{
		var member = await _userRepository.GetMemberAsync(username);

		return Ok(member);
	}

	////////////////////////////////////////////////
	///////////////////////////////////////////////////
	// XXXX: api/Users












	////////////////////////////////////////////////
	///////////////////////////////////////////////////
	// XXXX: api/Users





}
