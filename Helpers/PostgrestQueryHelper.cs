namespace api.Helpers
{
    public static class PostgrestQueryHelper
    {
        public static string ArrayToString(List<string> array)
        {
            return string.Join(",", array);
        }
    }
}