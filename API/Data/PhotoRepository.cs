using System;

namespace API.Data;

using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

public class PhotoRepository(DataContext context, IMapper mapper) : IPhotoRepository
{
    public async Task<Photo?> GetPhotoById(int id)
    {
        return await context.Photos.IgnoreQueryFilters().SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PagedList<PhotoForApprovalDTO>> GetUnapprovedPhotos(UserParams userParams)
    {
        var query = context.Photos.AsQueryable();
        query = query
        .IgnoreQueryFilters()
        .Where(p => p.IsApproved == false);
        
        return await PagedList<PhotoForApprovalDTO>.CreateAsync(query.ProjectTo<PhotoForApprovalDTO>(mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
    }

    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
}
