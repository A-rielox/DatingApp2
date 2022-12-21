using API.DTOs;
using API.Entities;
using API.Extensions;
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
    private readonly IPhotoService _photoService;

    public UsersController(	IUserRepository userRepository,
							IMapper mapper,
							IPhotoService photoService)
	{
		_userRepository = userRepository;
        _mapper = mapper;
        _photoService = photoService;
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
        // var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // con mi metodo de extension
        var username = User.GetUsername();

        var user = await _userRepository.GetUserByUsernameAsync(username);

		if (user == null) return NotFound();

		//                       ------>
		_mapper.Map(memberUpdateDto, user);

		if(await _userRepository.SaveAllAsync()) return NoContent();

		return BadRequest("Fail to update user.");
	}

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // POST: api/Users/add-photo
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        // si es su primera foto => la pongo como mail
        if (user.Photos.Count == 0)
        {
            photo.IsMain = true;
        }

        user.Photos.Add(photo);

        if (await _userRepository.SaveAllAsync())
        {//                        <-------
            return _mapper.Map<PhotoDto>(photo);
            // el " new {} " es xq la ruta GetUser ocupa ese parametro para ir al user
            // return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));

        }

        return BadRequest("Problem adding photo.");
    }





}
