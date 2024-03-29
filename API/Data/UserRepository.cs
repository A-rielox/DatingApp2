﻿using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context,IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
                    .Include(u => u.Photos)
                    .FirstOrDefaultAsync(u => u.UserName == username);
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _context.Users
                        .Include(u => u.Photos)
                        .ToListAsync();
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<MemberDto> GetMemberAsync(string username)
    {
        // con la .ProjectTo NO necesito hacer el .include de las photos,
        // lo hace solito
        var member = await _context.Users
            .Where(u => u.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return member;


        // sin AutoMapper
        // return await _context.Users
        //                 .Where(u => u.UserName == username)
        //                 .Select(u => new MemberDto
        //                 {
        //                     Id = u.Id,
        //                     UserName = u.UserName,
        //                     KnownAs = u.KnownAs,
        //                 }).FirstOrDefaultAsync();
    }

    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        //var members = await _context.Users
        //    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        //    .ToListAsync();

        //return members;

        var query = _context.Users.AsQueryable();

        query = query.Where(u => u.UserName != userParams.CurrentUsername);
        query = query.Where(u => u.Gender == userParams.Gender);

        // p' filtrar x la edad pasada
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActive)
        };
         
        return await PagedList<MemberDto>.CreateAsync(
                    query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
                    userParams.PageNumber, userParams.PageSize);
    }
}
