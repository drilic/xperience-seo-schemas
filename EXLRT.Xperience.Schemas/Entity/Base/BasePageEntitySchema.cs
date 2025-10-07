using CMS.Websites;
using EXLRT.Xperience.Schemas.Contracts;
using EXLRT.Xperience.Schemas.Helpers;

namespace EXLRT.Xperience.Schemas.Entity.Base
{
    public abstract class BasePageEntitySchema : BaseEntitySchema
    {
        public override string OverrideSchemaValue => this.GetSchemaOverrideValue();

        public IWebPageFieldsSource CurrentPage { get; set; }

        protected BasePageEntitySchema(IWebPageFieldsSource currentPage)
        {
            this.CurrentPage = currentPage;
        }

        internal string GetSchemaOverrideValue()
        {
            if (this.CurrentPage != null && this.CurrentPage is ISchemaFields)
            {
                var properties = ReflectionPropertiesHelper.GetProperties(this.CurrentPage.GetType());

                return properties.FirstOrDefault(p => p.Name.Equals(nameof(ISchemaFields.SEOPageSchemaOverride)))?.Getter(this.CurrentPage)?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
