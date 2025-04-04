using api.Data;
using api.DTOs.Cocktails;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CocktailRepository : ICocktailRepository
    {
        //         private readonly AppDbContext _context;

        //         public CocktailRepository(AppDbContext context)
        //         {
        //             _context = context;
        //         }

        //         public async Task<Cocktail> CreateAsync(Cocktail cocktailModel)
        //         {
        //             await _context.Cocktails.AddAsync(cocktailModel);
        //             await _context.SaveChangesAsync();
        //             return cocktailModel;
        //         }

        //         public async Task<Cocktail?> DeleteAsync(int id)
        //         {
        //             var cocktail = await _context.Cocktails.FirstOrDefaultAsync(c => c.Id == id);

        //             if (cocktail == null)
        //             {
        //                 return null;
        //             }

        //             _context.Cocktails.Remove(cocktail);
        //             await _context.SaveChangesAsync();
        //             return cocktail;
        //         }

        //         // public async Task<List<CocktailDto>> GetAllAsync()
        //         // {
        //         //     return await _context.Cocktails.Include(i => i.CocktailIngredients).ThenInclude(ci => ci.Ingredient).Select(c => c.ToCocktailDto()).ToListAsync();
        //         // }

        //         // public async Task<Cocktail?> GetByIdAsync(int id)
        //         // {
        //         //     return await _context.Cocktails.Include(i => i.CocktailIngredients).ThenInclude(ci => ci.Ingredient).FirstOrDefaultAsync(c => c.Id == id);
        //         // }

        //         //         public async Task<Cocktail?> UpdateAsync(int id, UpdateCocktailRequestDto cocktailModel)
        //         //         {
        //         //             var existingCocktail = await _context.Cocktails.FirstOrDefaultAsync(c => c.Id == id);

        //         //             if (existingCocktail == null)
        //         //             {
        //         //                 return null;
        //         //             }

        //         //             existingCocktail.Name = cocktailModel.Name;
        //         //             existingCocktail.Featured = cocktailModel.Featured;
        //         //             existingCocktail.Tags = cocktailModel.Tags;
        //         //             // existingCocktail.CreatedAt = cocktailModel.CreatedAt;
        //         //             // existingCocktail.UpdatedAt = cocktailModel.UpdatedAt;

        //         //             await _context.SaveChangesAsync();
        //         //             return existingCocktail;
        //         //         }
    }
}
