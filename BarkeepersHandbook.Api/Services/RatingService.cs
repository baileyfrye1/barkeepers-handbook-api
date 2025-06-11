using BarkeepersHandbook.Api.Errors;
using BarkeepersHandbook.Api.Exceptions;
using BarkeepersHandbook.Api.Mappers;
using BarkeepersHandbook.Application.DTOs.RatingDTOs;
using BarkeepersHandbook.Application.Models;
using OneOf;
using OneOf.Types;
using Supabase;

namespace BarkeepersHandbook.Api.Services;

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
        CocktailRatingDto ratingDto,
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

        var testModel = ratingDto.ToRatingFromDto(cocktailId, userId);

        var ratingModel = ratingDto.ToRatingFromDto(cocktailId, userId);

        var createdRating = await _supabase.From<Rating>().Insert(ratingModel);

        if (createdRating.Model is null)
        {
            return new UnexpectedError("Failed to create rating");
        }

        return createdRating.Model;
    }

    public async Task<List<RatingDto>> GetRatingsByUserAsync(string userId)
    {
        var query = _supabase.From<Rating>();

        var result = await query.Where(r => r.UserId == userId).Get();

        var ratings = result.Models.Select(r => r.ToRatingDto()).ToList();

        return ratings;
    }

    public async Task<List<RatingDto>> GetAllRatingsByCocktailIdAsync(int cocktailId)
    {
        var query = await _supabase.From<Rating>().Where(r => r.CocktailId == cocktailId).Get();

        return query.Models.Count == 0 ? [] : query.Models.Select(r => r.ToRatingDto()).ToList();
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
        CocktailRatingDto ratingDto,
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

        ratingToBeUpdated.RatingValue = ratingDto.Rating;

        await ratingToBeUpdated.Update<Rating>();

        return new Success();
    }
}

public interface IRatingService
{
    Task<List<RatingDto>> GetRatingsByUserAsync(string userId);

    Task<List<RatingDto>> GetAllRatingsByCocktailIdAsync(int cocktailId);

    Task<OneOf<Rating, UnexpectedError, AlreadyRated>> CreateRatingAsync(
        CocktailRatingDto ratingDto,
        int cocktailId,
        string userId
    );

    Task<OneOf<Success, NotFound>> UpdateRatingAsync(
        CocktailRatingDto ratingDto,
        int id,
        string userId
    );

    Task DeleteRatingByIdAsync(string userId, int id);
}
