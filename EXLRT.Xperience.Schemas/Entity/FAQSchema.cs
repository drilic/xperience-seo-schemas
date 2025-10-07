using CMS.Websites;
using EXLRT.Xperience.Schemas.Contracts;
using EXLRT.Xperience.Schemas.Entity.Base;
using EXLRT.Xperience.Schemas.Helpers;
using EXLRT.Xperience.Schemas.Models;
using Schema.NET;

namespace EXLRT.Xperience.Schemas.Entity
{
    public class FAQSchema : BasePageEntitySchema
    {
        public override string SchemaName => nameof(FAQSchema);
        public string Name { get; }
        public string Description { get; }
        public IEnumerable<FaqSchemaItem> Items { get; }

        public FAQSchema(IWebPageFieldsSource currentPage, string name = null, string description = null, IEnumerable<FaqSchemaItem> items = null) : base(currentPage)
        {
            this.Name = name;
            this.Description = description;
            this.Items = items;
        }

        public override async Task<Thing> Build(ISchemasService services, IWebPageUrlRetriever urlRetriever)
        {
            var schemaConfiguration = await services.GetConfigAsync();

            var faq = this.Items?.Select(item => new Question
            {
                Name = item.QuestionPlainText,
                AcceptedAnswer = new Answer
                {
                    Text = item.AnswerPlainText
                }
            });

            var schema = new FAQPage()
            {
                Name = this.Name,
                Description = this.Description,
                Url = UrlHelper.BuildAbsoluteUrl((await urlRetriever.Retrieve(this.CurrentPage)).RelativePath).AsUri(),
                InLanguage = schemaConfiguration.ChannelLanguage,
                MainEntity = new OneOrMany<IThing>(faq)
            };

            return schema;
        }
    }
}