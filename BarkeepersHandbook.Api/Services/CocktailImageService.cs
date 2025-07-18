using Client = Supabase.Client;
using FileOptions = Supabase.Storage.FileOptions;

namespace BarkeepersHandbook.Api.Services;

public class CocktailImageService : ICocktailImageService
{
	private readonly Client _supabase;
	private const string BucketName = "cocktail-images";
	public CocktailImageService(Client supabase)
	{
		_supabase = supabase;
	}

	public async Task<string> UploadImage(IFormFile file)
	{
		var extension = Path.GetExtension(file.FileName);
		var newName = $"{Guid.NewGuid()}{extension}";
		var tempFilePath = Path.Combine(Path.GetTempPath(), newName);
		
		await using (var stream = new FileStream(tempFilePath, FileMode.Create))
		{
			await file.CopyToAsync(stream);
		}
		
		await _supabase.Storage.From(BucketName).Upload(tempFilePath, newName, new FileOptions { CacheControl = "3600", Upsert = false, ContentType = file.ContentType});
		
		File.Delete(tempFilePath);
		
		return _supabase.Storage.From(BucketName).GetPublicUrl(newName);
	}

	public async Task DeleteImage(string imageName)
	{
		await _supabase.Storage.From(BucketName).Remove(imageName);
	}
}

public interface ICocktailImageService
{
	Task<string> UploadImage(IFormFile file);
	Task DeleteImage(string imageName);
}