using Supabase.Storage;

namespace api.Data
{
	public class ImageService
	{
		private readonly StorageFileApi _storageBucket;
		public ImageService(StorageFileApi storageBucket)
		{
			_storageBucket = storageBucket;
		}

		public async Task<string> UploadImage()
		{
			return "test";
		}
	}
}