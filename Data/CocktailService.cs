//TODO Create a factory service class that handles base operations but have separate service classes to extend functionality for specific tables
using api.DTOs.Cocktails;
using api.Helpers;

// using api.Helpers;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Interfaces;
using static Supabase.Postgrest.Constants;

namespace api.Data
{
    public class CocktailService
    {
        private readonly Supabase.Client _supabase;

        public CocktailService(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<Cocktail> AddOneAsync(CreateCocktailRequestDto cocktailRequestDto)
        {
            var cocktailModel = cocktailRequestDto.ToCocktailFromCreateDto();

            var result = await _supabase.From<Cocktail>().Insert(cocktailModel);

            // using var memoryStream = new MemoryStream();

            // await cocktailRequestDto.Image.CopyToAsync(memoryStream);

            // Console.WriteLine(memoryStream);

            // var lastIndexOfDot = cocktailRequestDto.Image.FileName.LastIndexOf('.');

            // string extension = cocktailRequestDto.Image.FileName.Substring(lastIndexOfDot + 1);

            // await _supabase
            //     .Storage.From("cocktail-images")
            //     .Upload(memoryStream.ToArray(), $"cocktail-{cocktailModel.Id}.{extension}");

            if (result is null)
            {
                return null;
            }

            var newCocktail = result.Model;

            return newCocktail;
        }

        public async Task<List<CocktailDto>> AddManyAsync(List<Cocktail> cocktails)
        {
            var result = await _supabase.From<Cocktail>().Insert(cocktails);

            var newCocktails = result.Models.Select(c => c.ToCocktailDto()).ToList();

            return newCocktails;
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

            var cocktails = result.Models.Select(c => c.ToCocktailDto()).ToList();

            return cocktails;
        }

        public async Task<CocktailDto?> GetOneByIdAsync(int id)
        {
            var result = await _supabase
                .From<Cocktail>()
                .Select("*, cocktail_id:cocktail_ingredients!inner(*)")
                .Where(n => n.Id == id)
                .Get();

            if (result.Model == null)
            {
                return null;
            }

            var cocktail = result.Model.ToCocktailDto();

            return cocktail;
        }

        public async Task<CocktailDto?> GetOneByNameAsync(string name)
        {
            var result = await _supabase
                .From<Cocktail>()
                .Select("*, cocktail_id:cocktail_ingredients!inner(*)")
                .Where(n => n.Name == name)
                .Get();

            if (result.Model == null)
            {
                return null;
            }

            var cocktail = result.Models.FirstOrDefault().ToCocktailDto();

            return cocktail;
        }

        public async Task<Cocktail?> UpdateOneAsync(int id, Cocktail cocktailModel)
        {
            Console.WriteLine(cocktailModel.Name);
            var cocktailToBeUpdated = await _supabase
                .From<Cocktail>()
                .Where(n => n.Id == id)
                .Single();

            if (cocktailToBeUpdated == null)
            {
                return null;
            }

            cocktailToBeUpdated.Name = cocktailModel.Name;
            cocktailToBeUpdated.Featured = cocktailModel.Featured;
            cocktailToBeUpdated.Tags = cocktailModel.Tags;
            cocktailToBeUpdated.CocktailIngredients = cocktailModel.CocktailIngredients;
            cocktailToBeUpdated.UpdatedAt = cocktailModel.UpdatedAt;

            await cocktailToBeUpdated.Update<Cocktail>();

            return cocktailToBeUpdated;
        }

        public void DeleteOneAsync(int id)
        {
            _supabase.From<Cocktail>().Where(n => n.Id == id).Delete();
        }
    }
}
