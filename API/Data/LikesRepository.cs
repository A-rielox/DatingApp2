using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        // para ver si ya existe un like
        return await _context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
    {
        // la lista de a quienes a dado like - userid = sourceuserid
        // la lista de quienes le han dado like - userid = likeduserid
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        // para los q este user ha dado like
        if (predicate == "liked")
        {
            likes = likes.Where(l => l.SourceUserId == userId);
            // selecciona en "users" solo a los q estan en la lista "likes"
            users = likes.Select(l => l.TargetUser);
        }

        // los q le han dado like al user actual
        if (predicate == "likedBy")
        {
            likes = likes.Where(l => l.TargetUserId == userId );
            // selecciona en "users" solo a los q estan en la lista "likes"
            users = likes.Select(l => l.SourceUser );
        }

        var result = await users.Select(u => new LikeDto
        {
            UserName = u.UserName,
            KnownAs = u.KnownAs,
            Age = u.DateOfBirth.CalculateAge(),
            PhotoUrl = u.Photos.FirstOrDefault(p => p.IsMain).Url,
            City = u.City,
            Id = u.Id,
        }).ToListAsync();

        return result;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        // devuelve un user con sus likes
        return await _context.Users
            .Include(u => u.LikedUsers)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
