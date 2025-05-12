using api.DTOs.RatingDTOs;
using api.Models;

namespace api.Mappers;

public static class RatingMappers
{
    public static RatingDto ToRatingDto(this Rating ratingModel)
    {
        return new RatingDto
        {
            Id = ratingModel.Id,
            Cocktail = ratingModel.Cocktail,
            Rating = ratingModel.RatingValue,
            UserId = ratingModel.UserId,
        };
    }

    public static CocktailRatingDto ToCocktailRatingDto(this Rating ratingModel)
    {
        return new CocktailRatingDto
        {
            Rating = ratingModel.RatingValue
        };
    }
    
    public static CocktailRatingDto ToCocktailRatingDto(this RatingDto ratingDto)
    {
        return new CocktailRatingDto()
        {
            Rating = ratingDto.Rating,
        };
    }

    public static Rating ToRatingFromDto(this CocktailRatingDto ratingDto, int cocktailId, string userId)
    {
        return new Rating
        {
            CocktailId = cocktailId,
            UserId = userId,
            RatingValue = ratingDto.Rating,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
    }
}