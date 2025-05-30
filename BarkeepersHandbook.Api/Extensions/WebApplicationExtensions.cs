namespace BarkeepersHandbook.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseGlobalErrorHandling(this WebApplication app)
        {
            app.UseExceptionHandler();
            return app;
        }
    }
}