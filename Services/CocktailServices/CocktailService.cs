using api.DTOs.CocktailDTOs;
using api.Errors;
using api.Mappers;
using api.Models;
using api.Validators;
using FluentValidation;
using OneOf;
using OneOf.Types;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace api.Services.CocktailServices;

public class CocktailService : ICocktailService
{
    private readonly Client _supabase;
    private readonly IRatingService _ratingService;
    private readonly IValidator<CreateCocktailRequestDto> _validator;

    public CocktailService(Client supabase, IValidator<CreateCocktailRequestDto> validator, IRatingService ratingService)
    {
        _supabase = supabase;
        _validator = validator;
        _ratingService = ratingService;
    }

    public async Task<OneOf<Cocktail, ValidationFailed, UnexpectedError>> AddOneAsync(CreateCocktailRequestDto cocktailRequestDto, string imageUrl)
    {
        var validationResult = _validator.Validate(cocktailRequestDto);

        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        var cocktailModel = cocktailRequestDto.ToCocktailFromCreateDto(imageUrl);

        var result = await _supabase.From<Cocktail>().Insert(cocktailModel);

        if (result.Model is null)
        {
            return new UnexpectedError("Failed to insert cocktail into database.");
        }

        var newCocktail = result.Model;

        return newCocktail;
    }
    
    public async Task<(List<CocktailDto>? Cocktails, int? TotalCount)> GetAllAsync(string? search, int page, bool countOnly)
    {
        var count = await _supabase.From<Cocktail>().Select("*").Count(Constants.CountType.Exact);
        if (countOnly)
        {
            return (null, count);
        }
        
        // Pagination Variables
        const int itemsPerPage = 10;
        var offset = page == 1 ? 0 : (page - 1) * itemsPerPage;
        var itemLimit = (page * itemsPerPage) - 1;
        
        var query = _supabase.From<Cocktail>().Select("*");

        if (!string.IsNullOrEmpty(search))
        {
            var capitalizedSearch = string.Join(" ", search.Split(" ").Select(s => char.ToUpper(s[0]) + s.Substring(1)));

            query = query
                .Where(c => c.Name.Contains(capitalizedSearch) || c.Tags.Contains(search));
        }

        var result = await query.Range(offset, itemLimit).Get();

        var cocktails = result.Models.Select(c => c.ToCocktailDto()).ToList();
        
        var cocktailsWithRatings = cocktails.Select(async c =>
        {
            var fetchedRatings = await _ratingService.GetAllRatingsByIdAsync(c.Id);
            c.RatingsData.Ratings = fetchedRatings.Select(r => r.ToCocktailRatingDto()).ToList();
            return c;
        }).ToList();
        
        var awaitedCocktails = (await Task.WhenAll(cocktailsWithRatings)).ToList();

        return (Cocktails: awaitedCocktails, TotalCount: count);
    }

    public async Task<List<CocktailDto>> GetFeaturedAsync()
    {
        var result = await _supabase.From<Cocktail>().Select("*, cocktail_id:cocktail_ingredients!inner(*)").Where(n => n.Featured == true).Get();

        var cocktails = result.Models.Select(c => c.ToCocktailDto()).ToList();
        
        var cocktailsWithRatings = cocktails.Select(async c =>
        {
            var fetchedRatings = await _ratingService.GetAllRatingsByIdAsync(c.Id);
            c.RatingsData.Ratings = fetchedRatings.Select(r => r.ToCocktailRatingDto()).ToList();
            return c;
        }).ToList();

        var awaitedCocktails = (await Task.WhenAll(cocktailsWithRatings)).ToList();

        return awaitedCocktails;
    }

    public async Task<OneOf<CocktailDto, NotFound>> GetOneByIdAsync(int id)
    {
        var result = await _supabase
            .From<Cocktail>()
            .Select("*, cocktail_id:cocktail_ingredients!inner(*)")
            .Where(n => n.Id == id)
            .Get();

        if (result.Model is null)
        {
            return new NotFound();
        }

        var cocktail = result.Model.ToCocktailDto();
        
        var fetchedRating = await _ratingService.GetAllRatingsByIdAsync(id);

        cocktail.RatingsData.Ratings = fetchedRating.Select(r => r.ToCocktailRatingDto()).ToList();

        return cocktail;
    }

    public async Task<OneOf<Success, NotFound>> UpdateOneAsync(int id, Cocktail cocktailModel)
    {
        var cocktailToBeUpdated = await _supabase
            .From<Cocktail>()
            .Where(n => n.Id == id)
            .Single();

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

    public async Task<OneOf<Success, NotFound>> DeleteOneAsync(int id)
    {
        var result = await GetOneByIdAsync(id);

        return result.Match<OneOf<Success, NotFound>>(
            cocktail =>
            {
                _supabase.From<Cocktail>().Where(c => c.Id == id).Delete();
                return new Success();
            },
            _ => new NotFound()
        );
    }
}

public interface ICocktailService
{
    Task<OneOf<Cocktail, ValidationFailed, UnexpectedError>> AddOneAsync(CreateCocktailRequestDto cocktailRequestDto, string imageUrl);
    Task<(List<CocktailDto>? Cocktails, int? TotalCount)> GetAllAsync(string? search, int page, bool countOnly);
    Task<List<CocktailDto>> GetFeaturedAsync();
    Task<OneOf<CocktailDto, NotFound>> GetOneByIdAsync(int id);
    Task<OneOf<Success, NotFound>> UpdateOneAsync(int id, Cocktail cocktailModel);
    Task<OneOf<Success, NotFound>> DeleteOneAsync(int id);
}