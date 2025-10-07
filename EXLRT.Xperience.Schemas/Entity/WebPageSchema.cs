using CMS.Websites;
using EXLRT.Xperience.Schemas.Contracts;
using EXLRT.Xperience.Schemas.Entity.Base;
using EXLRT.Xperience.Schemas.Helpers;
using Schema.NET;

namespace EXLRT.Xperience.Schemas.Entity
{
    public class WebPageSchema : BasePageEntitySchema
    {
        public override string SchemaName => nameof(WebPageSchema);
        public string Name { get; }
        public string Description { get; }
        public string RelativeImage { get; }
        public DateTime? PublishedDate { get; }

        public WebPageSchema(IWebPageFieldsSource currentPage, string name = null, string description = null, string relativeImagePath = null, DateTime? publishedDate = null) : base(currentPage)
        {
            this.Name = name;
            this.Description = description;
            this.RelativeImage = relativeImagePath;
            this.PublishedDate = publishedDate;
        }

        public override async Task<Thing> Build(ISchemasService services, IWebPageUrlRetriever urlRetriever)
        {
            var schemaConfiguration = await services.GetConfigAsync();

            var schema = new WebPage()
            {
                Name = this.Name,
                Description = this.Description,
                Url = UrlHelper.BuildAbsoluteUrl((await urlRetriever.Retrieve(this.CurrentPage)).RelativePath).AsUri(),
                InLanguage = schemaConfiguration.ChannelLanguage
            };

            if (!string.IsNullOrEmpty(this.RelativeImage))
            {
                schema.Image = UrlHelper.BuildAbsoluteUrl(this.RelativeImage).AsUri();
            }

            if (this.PublishedDate.HasValue)
            {
                schema.DatePublished = new DateTimeOffset(this.PublishedDate.Value);
            }

            return schema;
        }
    }
}