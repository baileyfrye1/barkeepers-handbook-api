using BarkeepersHandbook.Application.Errors;
using BarkeepersHandbook.Application.Exceptions;
using BarkeepersHandbook.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;

namespace BarkeepersHandbook.Application.Services.CocktailServices;

public class CocktailService : ICocktailService
{
    private readonly ISupabaseClientService<Cocktail> _supabase;
    private readonly IRatingService _ratingService;
    private readonly ILogger<CocktailService> _logger;
    private readonly ICocktailImageService _imageService;
    private readonly ICocktailManagementService _cocktailManagementService;

    public CocktailService(ISupabaseClientService<Cocktail> supabase, IRatingService ratingService, ILogger<CocktailService> logger, ICocktailImageService imageService, ICocktailManagementService cocktailManagementService)
    {
        _supabase = supabase;
        _ratingService = ratingService;
        _logger = logger;
        _imageService = imageService;
        _cocktailManagementService = cocktailManagementService;
    }

    public async Task<OneOf<Cocktail, UnexpectedError>> CreateCocktailAsync(Cocktail cocktail, IFormFile? imageFile)
    { 
        string? imageUrl = null;
        if (imageFile != null)
        {
            imageUrl = await _imageService.UploadImage(imageFile);
        }
        cocktail.ImageUrl = imageUrl;

        cocktail.Tags = cocktail.Tags.Select(t => t.ToLower()).ToHashSet();
        foreach (var cocktailIngredient in cocktail.CocktailIngredients)
        {
            cocktail.Tags.Add(cocktailIngredient.Ingredient.Name.ToLower());
        }
 
        var result = await _supabase.InsertAsync(cocktail);
        var createdCocktail = result.Model;
 
        if (createdCocktail is null)
        {
            return new UnexpectedError("Failed to insert cocktail into database.");
        }
 
        await _cocktailManagementService.AddCocktailIngredients(createdCocktail, cocktail.CocktailIngredients);
 
        return createdCocktail; 
    }
    
    public async Task<(List<Cocktail>? Cocktails, int? TotalCount)> GetAllAsync(string? search, int page, bool countOnly)
    {
        var count = await _supabase.Count();
        if (countOnly)
        {
            return (null, count);
        }
        
        // Pagination Variables
        const int itemsPerPage = 10;
        var offset = page == 1 ? 0 : (page - 1) * itemsPerPage;
        var itemLimit = (page * itemsPerPage) - 1;

        var query = _supabase.GetAll();

        if (!string.IsNullOrEmpty(search))
        {
            var capitalizedSearch = string.Join(" ", search.Split(" ").Select(s => char.ToUpper(s[0]) + s.Substring(1)));

            query = query
                .Where(c => c.Name.Contains(capitalizedSearch) || c.Tags.Contains(search));
        }

        var result = await query.Range(offset, itemLimit).Get();

        var cocktails = result.Models;
        
        // var cocktailsWithRatings = cocktails.Select(async c =>
        // {
        //     var fetchedRatings = await _ratingService.GetAllRatingsByCocktailIdAsync(c.Id);
        //     // c.RatingsData.Ratings = fetchedRatings.Select(r => r.ToCocktailRatingDto()).ToList();
        //     return c;
        // }).ToList();
        //
        // var awaitedCocktails = (await Task.WhenAll(cocktailsWithRatings)).ToList();

        return (Cocktails: cocktails, TotalCount: count);
    }

    // TODO: Optimize query to only run one ratings db call instead of fetching ratings for each featured cocktail
    public async Task<List<Cocktail>> GetFeaturedAsync()
    {
        var result = await _supabase.GetFeaturedAsync(n => n.Featured == true);

        var cocktails = result.Models;
        
        var cocktailsWithRatings = cocktails.Select(async c =>
        {
            var fetchedRatings = await _ratingService.GetAllRatingsByCocktailIdAsync(c.Id);
            // c.Ratings = fetchedRatings.Select(r => r.ToCocktailRatingDto()).ToList();
            return c;
        }).ToList();

        var awaitedCocktails = (await Task.WhenAll(cocktailsWithRatings)).ToList();

        return awaitedCocktails;
    }

    public async Task<OneOf<Cocktail, NotFound>> GetOneByIdAsync(int id)
    {
        var result = await _supabase.GetByIdAsync(n => n.Id == id);

        if (result.Model is null)
        {
            return new NotFound();
        }
        
        var cocktail = result.Model;
        
        var fetchedRating = await _ratingService.GetAllRatingsByCocktailIdAsync(id);

        // cocktail.Ratings = fetchedRating;

        return cocktail;
    }

    public async Task<OneOf<Success, NotFound>> UpdateOneAsync(int id, Cocktail cocktailModel)
    {
        var cocktailToBeUpdated = await _supabase.UpdateByIdAsync(n => n.Id == id);
        
        if (cocktailToBeUpdated is null)
        {
            return new NotFound();
        }

        cocktailToBeUpdated.Name = cocktailModel.Name;
        cocktailToBeUpdated.Featured = cocktailModel.Featured;
        cocktailToBeUpdated.Tags = cocktailModel.Tags;
        cocktailToBeUpdated.CocktailIngredients = cocktailModel.CocktailIngredients;
        cocktailToBeUpdated.UpdatedAt = cocktailModel.UpdatedAt;

        await cocktailToBeUpdated.Update<Cocktail>();

        return new Success();
    }

    public async Task DeleteOneAsync(int id)
    {
        try
        {
            await _supabase.DeleteByIdAsync(n => n.Id == id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting cocktail with ID {id}", id);
            throw new ServiceLayerException($"Failed to delete cocktail with Id {id}", e);
        }
    }
}

public interface ICocktailService
{
    Task<OneOf<Cocktail, UnexpectedError>> CreateCocktailAsync(Cocktail cocktail, IFormFile imageFile);
    Task<(List<Cocktail>? Cocktails, int? TotalCount)> GetAllAsync(string? search, int page, bool countOnly);
    Task<List<Cocktail>> GetFeaturedAsync();
    Task<OneOf<Cocktail, NotFound>> GetOneByIdAsync(int id);
    Task<OneOf<Success, NotFound>> UpdateOneAsync(int id, Cocktail cocktailModel);
    Task DeleteOneAsync(int id);
}