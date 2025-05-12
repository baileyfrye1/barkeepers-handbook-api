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
}