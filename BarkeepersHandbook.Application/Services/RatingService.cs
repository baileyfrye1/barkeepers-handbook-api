using BarkeepersHandbook.Application.Errors;
using BarkeepersHandbook.Application.Exceptions;
using BarkeepersHandbook.Application.Models;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using Supabase;

namespace BarkeepersHandbook.Application.Services;

public class RatingService : IRatingService
{
    private readonly Client _supabase;
    private readonly ILogger<RatingService> _logger;

    public RatingService(Client supabase, ILogger<RatingService> logger)
    {
        _supabase = supabase;
        _logger = logger;
    }

    public async Task<OneOf<Rating, UnexpectedError, AlreadyRated>> CreateRatingAsync(
        Rating rating,
        int cocktailId,
        string userId
    )
    {
        var existingRating = await _supabase
            .From<Rating>()
            .Where(r => r.CocktailId == cocktailId && r.UserId == userId)
            .Get();

        if (existingRating.Model is not null)
        {
            return new AlreadyRated();
        }

        var createdRating = await _supabase.From<Rating>().Insert(rating);

        if (createdRating.Model is null)
        {
            return new UnexpectedError("Failed to create rating");
        }

        return createdRating.Model;
    }

    public async Task<List<Rating>> GetRatingsByUserAsync(string userId)
    {
        var query = _supabase.From<Rating>();

        var result = await query.Where(r => r.UserId == userId).Get();

        var ratings = result.Models;

        return ratings;
    }

    public async Task<List<Rating>> GetAllRatingsByCocktailIdAsync(int cocktailId)
    {
        var query = await _supabase.From<Rating>().Where(r => r.CocktailId == cocktailId).Get();

        return query.Models.Count == 0 ? [] : query.Models;
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

    public async Task<OneOf<Success, NotFound>> UpdateRatingAsync(
        Rating rating,
        int id,
        string userId
    )
    {
        var ratingToBeUpdated = await _supabase
            .From<Rating>()
            .Where(r => r.Id == id && r.UserId == userId)
            .Single();

        if (ratingToBeUpdated is null)
        {
            return new NotFound();
        }

        ratingToBeUpdated.RatingValue = rating.RatingValue;

        await ratingToBeUpdated.Update<Rating>();

        return new Success();
    }
}

public interface IRatingService
{
    Task<List<Rating>> GetRatingsByUserAsync(string userId);

    Task<List<Rating>> GetAllRatingsByCocktailIdAsync(int cocktailId);

    Task<OneOf<Rating, UnexpectedError, AlreadyRated>> CreateRatingAsync(
        Rating rating,
        int cocktailId,
        string userId
    );

    Task<OneOf<Success, NotFound>> UpdateRatingAsync(
        Rating rating,
        int id,
        string userId
    );

    Task DeleteRatingByIdAsync(string userId, int id);
}
