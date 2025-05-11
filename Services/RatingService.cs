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

    public async void CreateReviewAsync(Rating rating)
    {
        var reviewResult = await _supabase.From<Rating>().Insert(rating);
        var model = reviewResult.Model;
    }

    public async Task<List<RatingDto>> GetReviewsAsync(string userId)
    {
        var query = _supabase.From<Rating>();

        var result = await query.Where(r => r.UserId == userId).Get();

        var reviews = result.Models.Select(r => r.ToRatingDto()).ToList();
        
        return reviews;
    }
}