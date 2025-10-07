using CMS.Websites;
using EXLRT.Xperience.Schemas.Contracts;
using EXLRT.Xperience.Schemas.Entity.Base;
using EXLRT.Xperience.Schemas.Helpers;
using Schema.NET;

namespace EXLRT.Xperience.Schemas.Entity
{
    public class ArticleSchema : BasePageEntitySchema
    {
        public override string SchemaName => nameof(Article);
        public string Title { get; }
        public string Summary { get; }
        public string RelativeImage { get; }
        public string AuthorFullName { get; }
        public DateTime? PublishedDate { get; }

        public ArticleSchema(IWebPageFieldsSource currentPage, string title = null, string summary = null, string relativeImagePath = null, string authorFullName = null, DateTime? publishedDate = null) : base(currentPage)
        {
            this.Title = title;
            this.Summary = summary;
            this.RelativeImage = relativeImagePath;
            this.AuthorFullName = authorFullName;
            this.PublishedDate = publishedDate;
        }

        public override async Task<Thing> Build(ISchemasService services, IWebPageUrlRetriever urlRetriever)
        {
            var schemaConfiguration = await services.GetConfigAsync();

            var articleUrl = UrlHelper.BuildAbsoluteUrl((await urlRetriever.Retrieve(this.CurrentPage)).RelativePath).AsUri();
            var schema = new Article()
            {
                Headline = this.Title,
                Description = this.Summary,
                Url = articleUrl,
                InLanguage = schemaConfiguration.ChannelLanguage,
                MainEntityOfPage = new WebPage
                {
                    Id = articleUrl
                }
            };

            if (!string.IsNullOrEmpty(this.RelativeImage))
            {
                schema.Image = UrlHelper.BuildAbsoluteUrl(this.RelativeImage).AsUri();
            }

            if (this.PublishedDate.HasValue)
            {
                schema.DatePublished = new DateTimeOffset(this.PublishedDate.Value);
            }

            if (!string.IsNullOrEmpty(this.AuthorFullName))
            {
                schema.Author = new Person
                {
                    Name = this.AuthorFullName
                };
            }

            try
            {
                OrganizationSchema organization = new OrganizationSchema(this.CurrentPage);
                schema.Publisher = await organization.Build(services, urlRetriever) as Organization;
            }
            catch
            {
                // do nothing
            }

            return schema;
        }
    }
}