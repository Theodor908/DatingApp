using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper): ILikesRepository
{

    // implement interface
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurentUserLikeIds(int currentUserId)
    {
        return await context.Likes
            .Where(x => x.SourceUserId == currentUserId)
            .Select(x => x.TargetUserId)
            .ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes
            .FindAsync(sourceUserId, targetUserId);
    }

    public  async Task<PagedList<MemberDTO>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDTO> query; 
        string predicate = likesParams.Predicate;
        int userId = likesParams.UserId;
        int pageNumber = likesParams.PageNumber;
        int pageSize = likesParams.PageSize;

        switch(predicate)
        {
            case "liked":
                query =  likes
                .Where(x => x.SourceUserId == userId)
                .Select(x => x.TargetUser)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes
                    .Where(x => x.TargetUserId == userId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
           default:
                var likeIds = await GetCurentUserLikeIds(userId);
                query = context.Likes
                    .Where(x => x.TargetUserId == userId && likeIds.Contains(x.SourceUserId))
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDTO>(mapper.ConfigurationProvider); // mutual likes
                break;
            }

        return await PagedList<MemberDTO>.CreateAsync(query, pageNumber, pageSize);
    }
}
