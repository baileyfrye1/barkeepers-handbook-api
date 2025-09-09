using System.Linq.Expressions;
using Supabase.Postgrest;
using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Responses;
using Client = Supabase.Client;

namespace BarkeepersHandbook.Application.Services;

public class SupabaseClientService<TModel> : ISupabaseClientService<TModel> where TModel : BaseModel, new()
{
    private readonly Client _supabaseClient;

    private SupabaseClientService(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }
   
    public Task<ModeledResponse<TModel>> InsertAsync(TModel model)
    {
        return _supabaseClient.From<TModel>().Insert(model);
    }

    public IPostgrestTable<TModel> GetAll()
    {
        return _supabaseClient.From<TModel>().Select("*");
    }

    public Task<int> Count()
    {
        return _supabaseClient.From<TModel>().Select("*").Count(Constants.CountType.Exact);
    }

    public Task<ModeledResponse<TModel>> GetFeaturedAsync(Expression<Func<TModel,bool>> predicate)
    {
        return _supabaseClient.From<TModel>().Select("*, cocktail_id:cocktail_ingredients!inner(*)").Where(predicate).Get();
    }

    public Task<ModeledResponse<TModel>> GetByIdAsync(Expression<Func<TModel,bool>> predicate)
    {
        return _supabaseClient
            .From<TModel>()
            .Select("*, cocktail_id:cocktail_ingredients!inner(*)")
            .Where(predicate)
            .Get();
    }

    public Task<TModel?> UpdateByIdAsync(Expression<Func<TModel,bool>> predicate)
    {
        return _supabaseClient
            .From<TModel>()
            .Where(predicate)
            .Single();
    }

    public Task DeleteByIdAsync(Expression<Func<TModel,bool>> predicate)
    {
        return _supabaseClient.From<TModel>().Where(predicate).Delete();
    }

    public Task<string> UploadFile()
    {
        throw new NotImplementedException();
    }

    public string GetPublicUrl()
    {
        throw new NotImplementedException();
    }

    public Task RemoveFile()
    {
        throw new NotImplementedException();
    }
}

public interface ISupabaseClientService<TModel> where TModel : BaseModel, new()
{
    Task<ModeledResponse<TModel>> InsertAsync(TModel model);
    IPostgrestTable<TModel> GetAll();
    Task<int> Count();
    Task<ModeledResponse<TModel>> GetFeaturedAsync(Expression<Func<TModel,bool>> predicate); 
    Task<ModeledResponse<TModel>> GetByIdAsync(Expression<Func<TModel,bool>> predicate); 
    Task<TModel?> UpdateByIdAsync(Expression<Func<TModel,bool>> predicate);
    Task DeleteByIdAsync(Expression<Func<TModel,bool>> predicate);
    Task<string> UploadFile();
    string GetPublicUrl();
    Task RemoveFile();
}