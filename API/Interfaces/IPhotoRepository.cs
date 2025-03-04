using System;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IPhotoRepository
{

    Task<PagedList<PhotoForApprovalDTO>> GetUnapprovedPhotos(UserParams userParams);
    Task<Photo?> GetPhotoById(int id);
    void RemovePhoto(Photo photo);

}
