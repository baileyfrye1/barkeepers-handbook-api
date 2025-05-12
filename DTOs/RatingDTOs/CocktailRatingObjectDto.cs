namespace api.DTOs.RatingDTOs;

public class CocktailRatingObjectDto
{
    public List<CocktailRatingDto> Ratings { get; set; } = [];
    public double AverageRating => Ratings.Count != 0 ? Ratings.Average(r => r.Rating) : 0;
    public int TotalRatings => Ratings.Count;
}