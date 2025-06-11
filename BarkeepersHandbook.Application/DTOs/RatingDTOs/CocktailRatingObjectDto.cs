using System;
using System.Collections.Generic;
using System.Linq;

namespace BarkeepersHandbook.Application.DTOs.RatingDTOs;

public class CocktailRatingObjectDto
{
    public List<CocktailRatingDto> Ratings { get; set; } = [];
    public double AverageRating => Ratings.Count != 0 ? Math.Round(Ratings.Average(r => r.Rating), 1) : 0;
    public int TotalRatings => Ratings.Count;
}