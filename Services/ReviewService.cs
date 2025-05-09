using api.Models;
using Clerk.BackendAPI;
using Supabase;

namespace api.Services;

public class ReviewService
{
    private readonly Client _supabase;

    public ReviewService(Client supabase)
    {
        _supabase = supabase;
    }

    public async void CreateReviewAsync(Review review)
    {
        var reviewResult = await _supabase.From<Review>().Insert(review);
        var model = reviewResult.Model;
    }

    public async Task<List<Review>> GetReviewsAsync(string userId)
    {
        var query = _supabase.From<Review>();

        var result = await query.Where(r => r.UserId == userId).Get();

        var list = result.Models.ToList();

        foreach (var review in list)
        {
            Console.WriteLine(review);
        }
        return list;
    }
}