//TODO Create a factory service class that handles base operations but have separate service classes to extend functionality for specific tables
using api.DTOs.Cocktails;
using api.Errors;
using api.Mappers;
using api.Models;
using api.Validators;
using FluentValidation;
using OneOf;
using OneOf.Types;

namespace api.Data
{
    public class CocktailService
    {
        private readonly Supabase.Client _supabase;
        private readonly IValidator<CreateCocktailRequestDto> _validator;

        public CocktailService(Supabase.Client supabase, IValidator<CreateCocktailRequestDto> validator)
        {
            _supabase = supabase;
            _validator = validator;
        }

        public async Task<OneOf<Cocktail, ValidationFailed, UnexpectedError>> AddOneAsync(CreateCocktailRequestDto cocktailRequestDto)
        {
            var validationResult = _validator.Validate(cocktailRequestDto);

            if (!validationResult.IsValid)
            {
                return new ValidationFailed(validationResult.Errors);
            }

            var cocktailModel = cocktailRequestDto.ToCocktailFromCreateDto();

            var result = await _supabase.From<Cocktail>().Insert(cocktailModel);

            if (result.Model is null)
            {
                return new UnexpectedError("Failed to insert cocktail into database.");
            }

            var newCocktail = result.Model;

            return newCocktail;

        }

        public async Task<List<CocktailDto>> GetAllAsync(string? search)
        {
            var query = _supabase.From<Cocktail>().Select("*, cocktail_id:cocktail_ingredients!inner(*)");


            if (!string.IsNullOrEmpty(search))
            {
                var capitalizedSearch = string.Join(" ", search.Split(" ").Select(s => char.ToUpper(s[0]) + s.Substring(1)));

                query = query
                .Where(c => c.Name.Contains(capitalizedSearch) || c.Tags.Contains(search));
            }

            var result = await query.Get();

            var cocktails = result.Models.Select(c => c.ToCocktailDto()).ToList() ?? new List<CocktailDto>();

            return cocktails;
        }

        public async Task<List<CocktailDto>> GetFeaturedAsync()
        {
            var result = await _supabase.From<Cocktail>().Select("*, cocktail_id:cocktail_ingredients!inner(*)").Where(n => n.Featured == true).Get();

            var cocktails = result.Models.Select(c => c.ToCocktailDto()).ToList() ?? new List<CocktailDto>();

            return cocktails;
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
}
