using Supabase.Gotrue;

namespace api.Data
{
    public class AuthService
    {
        private readonly Supabase.Client _supabase;

        public AuthService(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<Session?> RegisterUser(string email, string password)
        {
            var session = await _supabase.Auth.SignUp(email, password);

            if (session is null)
            {
                return null;
            }

            return session;
        }

        public async Task<Session?> LoginUser(string email, string password)
        {
            var session = await _supabase.Auth.SignIn(email, password);

            if (session is null)
            {
                return null;
            }

            return session;
        }

        public User? RetrieveUser()
        {
            var user = _supabase.Auth.CurrentUser;

            if (user is null)
            {
                return null;
            }

            return user;
        }

        public async Task<Session?> RefreshToken()
        {
            await _supabase.Auth.RefreshToken();
            var session = _supabase.Auth.CurrentSession;

            if (session is null)
            {
                return null;
            }

            return session;
        }
    }
}
