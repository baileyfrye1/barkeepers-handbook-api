using api.DTOs.RatingDTOs;
using api.Mappers;
using api.Models;
using Clerk.BackendAPI;
using Supabase;

namespace api.Services;

public class RatingService : IRatingService
{
    private readonly Client _supabase;

    public RatingService(Client supabase)
    {
        _supabase = supabase;
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
}

public interface IRatingService
{
    Task<Rating> CreateRatingAsync(CocktailRatingDto ratingDto, int cocktailId, string userId);

    Task<List<RatingDto>> GetAllRatingsByUserAsync(string userId);

    Task<List<RatingDto>> GetAllRatingsByIdAsync(int cocktailId);
}