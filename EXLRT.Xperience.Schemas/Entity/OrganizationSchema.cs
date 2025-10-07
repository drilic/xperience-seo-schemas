using CMS.Websites;
using EXLRT.Xperience.Schemas.Contracts;
using EXLRT.Xperience.Schemas.Entity.Base;
using EXLRT.Xperience.Schemas.Helpers;
using Schema.NET;

namespace EXLRT.Xperience.Schemas.Entity
{
    public class OrganizationSchema : BasePageEntitySchema
    {
        public override string SchemaName => nameof(OrganizationSchema);

        public OrganizationSchema(IWebPageFieldsSource currentPage) : base(currentPage) { }

        public override async Task<Thing> Build(ISchemasService services, IWebPageUrlRetriever urlRetriever)
        {
            var schemaConfiguration = await services.GetConfigAsync();

            var schema = new Organization()
            {
                Url = UrlHelper.BuildAbsoluteUrl((await urlRetriever.Retrieve(this.CurrentPage)).RelativePath).AsUri(),
                Name = schemaConfiguration.OrganizationName,
                AreaServed = schemaConfiguration.OrganizationAreaServed,
                Brand = new Brand
                {
                    Name = schemaConfiguration.OrganizationBrand
                },
                LegalName = schemaConfiguration.OrganizationLegalName,
                SameAs = (schemaConfiguration.OrganizationIdentifiers).ClearAndBuildArrayOfURIs()
            };

            var contacts = (await services.GetContactsAsync())?.Where(item => item != null).Select(contact => new ContactPoint
            {
                Telephone = contact.Phone,
                Email = contact.Email,
                ContactType = contact.ContactType,
                AreaServed = new Values<IAdministrativeArea, IGeoShape, IPlace, string>(contact.AreaServed),
                AvailableLanguage = new Values<ILanguage, string>(contact.AvailableLanguages)
            });

            if (contacts?.Any() ?? false)
            {
                schema.ContactPoint = new OneOrMany<IContactPoint>(contacts);
            }

            try
            {
                var logoData = await services.GetLogoAsync();
                if (logoData != null)
                {
                    var absoluteImageUri = UrlHelper.BuildAbsoluteUrl(logoData.LogoUrl).AsUri();

                    schema.Logo = new ImageObject
                    {
                        Url = absoluteImageUri,
                        ContentUrl = absoluteImageUri,
                        Caption = logoData.LogoAltText,
                        InLanguage = schemaConfiguration.ChannelLanguage,
                    };
                }
            }
            catch
            {
                // do nothing
            }

            return schema;
        }
    }
}