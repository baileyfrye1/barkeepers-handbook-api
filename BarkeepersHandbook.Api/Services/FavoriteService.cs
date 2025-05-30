using BarkeepersHandbook.Api.DTOs.FavoriteDTOs;
using BarkeepersHandbook.Api.Errors;
using BarkeepersHandbook.Api.Models;
using OneOf;

namespace BarkeepersHandbook.Api.Services;

public class FavoriteService : IFavoriteService
{
 public Task<List<FavoriteDto>> GetFavoritesByUserAsync(string userId)
 {
  throw new NotImplementedException();
 }

 public Task<OneOf<Favorite, UnexpectedError>> AddFavoriteAsync(Favorite favorite)
 {
  throw new NotImplementedException();
 }

 public Task DeleteFavoriteByIdAsync(string userId, int id)
 {
  throw new NotImplementedException();
 }
}

public interface IFavoriteService
{
 Task<List<FavoriteDto>> GetFavoritesByUserAsync(string userId);

 Task<OneOf<Favorite, UnexpectedError>> AddFavoriteAsync(Favorite favorite);
 
 Task DeleteFavoriteByIdAsync(string userId, int id);
}