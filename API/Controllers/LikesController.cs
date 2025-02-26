using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(ILikesRepository likesRepository) : BaseAPIController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();

        if(sourceUserId == targetUserId) 
            return BadRequest("You cannot like yourself");

        var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

        if(existingLike == null)
        {
            var like = new UserLike{
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            likesRepository.AddLike(like);
        }
        else
        {
            likesRepository.DeleteLike(existingLike);
        }

        if(await likesRepository.SaveChanges())
            return Ok();

        return BadRequest("Failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        var userId = User.GetUserId();
        var users = await likesRepository.GetCurentUserLikeIds(userId);
        return Ok(users);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await likesRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users.CurrentPage, users);
        return Ok(users);
    }
}
