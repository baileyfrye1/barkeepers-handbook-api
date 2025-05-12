using api.DTOs.RatingDTOs;
using api.Mappers;
using api.Models;
using Clerk.BackendAPI;
using Supabase;

namespace api.Services;

public class RatingService
{
    private readonly Client _supabase;

    public RatingService(Client supabase)
    {
        _supabase = supabase;
    }

    public async void CreateRatingAsync(Rating rating)
    {
        var ratingResult = await _supabase.From<Rating>().Insert(rating);
        var model = ratingResult.Model;
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