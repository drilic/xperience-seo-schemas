using EXLRT.Xperience.Schemas.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace EXLRT.Xperience.Schemas.Configurations
{
    public static class SchemasConfigurations
    {
        public static void AddSchemas<TImplementation>(this IServiceCollection services) where TImplementation : ISchemasService
        {
            services.AddScoped(typeof(ISchemasService), typeof(TImplementation));
        }
    }
}