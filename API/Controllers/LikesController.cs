using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;

    public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
    {
        _userRepository = userRepository;
        _likesRepository = likesRepository;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // para dar like
    // POST: api/likes/{username} --> del route parameter
    [HttpPost("{username}")]
    // el username es de a quien se le da el like
    public async Task<ActionResult> AddLike(string username)
    {
        // el usuario dando el like
        var sourceUserId = User.GetUserId();

        // user al q le doy el like
        var likedUser = await _userRepository.GetUserByUsernameAsync(username);
        // el user logeado con su lista de likes
        var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

        // no se encontro el user al q le quieren dar el like
        if (likedUser == null) return NotFound();

        // para no darse likes a uno mismo
        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        // para ver si ya se le habia dado like
        var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

        if (userLike != null) return BadRequest("You already like this user.");
        // EN LUGAR DE MANDAR EL BADREQUEST SE PODRIA QUITAR EL LIKE 
        if (userLike != null) return BadRequest("You already liked this user.");

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = likedUser.Id,
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _userRepository.SaveAllAsync()) return Ok();        

        return BadRequest("Failed to like user.");
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    // GET: api/likes?predicate=liked o likedBy
    // p' agarrar los likes de un user
    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();

        var users = await _likesRepository.GetUserLikes(likesParams);

        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,
                                      users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }
}
