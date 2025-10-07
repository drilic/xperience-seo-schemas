using CMS.Core;
using Microsoft.Extensions.DependencyInjection;

namespace EXLRT.Xperience.Schemas.Helpers
{
    internal static class ServiceResolver
    {
        internal static T GetService<T>()
        {
            using (var scope = Service.Resolve<IServiceProvider>().CreateScope())
            {
                return scope.ServiceProvider.GetService<T>();
            }
        }
    }
}
