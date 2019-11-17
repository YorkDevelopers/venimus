using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace VenimusAPIs.Registration
{
    public static class Register_Localisation
    {
        public static void AddLocalisation(this IApplicationBuilder app)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-GB"),
                new CultureInfo("zu-ZA"),
            };

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-GB"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            };

            app.UseRequestLocalization(localizationOptions);
        }
    }
}
