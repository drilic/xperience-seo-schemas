using CMS.Websites;
using EXLRT.Xperience.Schemas.Contracts;
using EXLRT.Xperience.Schemas.Entity.Base;
using EXLRT.Xperience.Schemas.Helpers;
using EXLRT.Xperience.Schemas.Models;
using Schema.NET;

namespace EXLRT.Xperience.Schemas.Entity
{
    public class BreadcrumbSchema : BaseComponentEntitySchema
    {
        public override string SchemaName => nameof(BreadcrumbSchema);
        public IEnumerable<BreadcrumbSchemaItem> Items { get; }

        public BreadcrumbSchema(IEnumerable<BreadcrumbSchemaItem> items)
        {
            this.Items = items;
        }

        public override Task<Thing> Build(ISchemasService services, IWebPageUrlRetriever urlRetriever)
        {
            var breadcrumbItems = new List<IListItem>();

            var index = 1;
            foreach (var item in this.Items)
            {
                breadcrumbItems.Add(new ListItem()
                {
                    Position = index,
                    Item = new WebPage()
                    {
                        Id = UrlHelper.BuildAbsoluteUrl(item.Url).AsUri(),
                        Name = item.Name
                    }
                });

                index++;
            }

            return Task.FromResult<Thing>(new BreadcrumbList()
            {
                ItemListElement = breadcrumbItems
            });
        }
    }
}