using CMS.Websites;
using EXLRT.Xperience.Schemas.Contracts;
using Schema.NET;

namespace EXLRT.Xperience.Schemas.Entity.Base
{
    public abstract class BaseEntitySchema : IBaseEntitySchema
    {
        public abstract string SchemaName { get; }

        public abstract string OverrideSchemaValue { get; }

        public abstract Task<Thing> Build(ISchemasService services, IWebPageUrlRetriever urlRetriever);
    }
}
