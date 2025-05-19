using api.DTOs.RatingDTOs;
using api.Exceptions;
using api.Mappers;
using api.Models;
using OneOf;
using OneOf.Types;
using Supabase;

namespace api.Services;

public class RatingService : IRatingService
{
    private readonly Client _supabase;
    private readonly ILogger<RatingService> _logger;

    public RatingService(Client supabase, ILogger<RatingService> logger)
    {
        _supabase = supabase;
        _logger = logger;
    }

    public async Task<Rating> CreateRatingAsync(CocktailRatingDto ratingDto, int cocktailId, string userId)
    {
        var ratingModel = ratingDto.ToRatingFromDto(cocktailId, userId);
        
        var ratingResult = await _supabase.From<Rating>().Insert(ratingModel);
        
        return ratingResult.Model;
    }

    public async Task<List<RatingDto>> GetAllRatingsByUserAsync(string userId)
    {
        var query = _supabase.From<Rating>();

        var result = await query.Where(r => r.UserId == userId).Get();

        var ratings = result.Models.Select(r => r.ToRatingDto()).ToList();
        
        return ratings;
    }

    public async Task<List<RatingDto>> GetAllRatingsByIdAsync(int cocktailId)
    {
        var query = await _supabase.From<Rating>().Where(r => r.CocktailId == cocktailId).Get();

        return query.Models.Count == 0 
            ? []
            : query.Models.Select(r => r.ToRatingDto()).ToList();
    }

    public async Task DeleteRatingByIdAsync(string userId, int id)
    {
        try
        {
            await _supabase.From<Rating>().Where(r => r.Id == id && r.UserId == userId).Delete();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting rating with ID {id}", id);
            throw new ServiceLayerException($"Failed to delete rating with ID {id}", e);
        }
    }

    public async Task<OneOf<Success, NotFound>> UpdateRatingAsync(CocktailRatingDto ratingDto, int id, string userId)
    {
       var ratingToBeUpdated = await _supabase.From<Rating>().Where(r => r.Id == id && r.UserId == userId).Single();

       if (ratingToBeUpdated is null)
       {
           return new NotFound();
       }

       ratingToBeUpdated.RatingValue = ratingDto.Rating;

       await ratingToBeUpdated.Update<Rating>();
       
       return new Success();
    }
}

public interface IRatingService
{
    Task<Rating> CreateRatingAsync(CocktailRatingDto ratingDto, int cocktailId, string userId);

    Task<List<RatingDto>> GetAllRatingsByUserAsync(string userId);

    Task<List<RatingDto>> GetAllRatingsByIdAsync(int cocktailId);
    
    Task DeleteRatingByIdAsync(string userId, int id);

    Task<OneOf<Success, NotFound>> UpdateRatingAsync(CocktailRatingDto ratingDto, int id, string userId);
}