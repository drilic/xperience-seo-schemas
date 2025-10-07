using CMS.Websites;
using EXLRT.Xperience.Schemas.Contracts;
using EXLRT.Xperience.Schemas.Entity.Base;
using EXLRT.Xperience.Schemas.Helpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.RegularExpressions;

namespace EXLRT.Xperience.Schemas.Extensions
{
    public static class SchemasExtensions
    {
        private const string CONTEXT_SCHEMAS_KEY = "EXLRT.Xperience.SEO.Schema";

        #region "Add schemas"

        public static void AddSchema(this IHttpContextAccessor contextAccessor, BaseEntitySchema schema) =>
            AddSchema(contextAccessor?.HttpContext, schema);

        public static Task AddSchemaAsync(this IHttpContextAccessor contextAccessor, BaseEntitySchema schema) =>
            AddSchemaAsync(contextAccessor?.HttpContext, schema);

        public static void AddSchema(this HttpContext httpContext, BaseEntitySchema schema) =>
            AddSchemaInternalAsync(httpContext, schema).GetAwaiter().GetResult();

        public static Task AddSchemaAsync(this HttpContext httpContext, BaseEntitySchema schema) =>
            AddSchemaInternalAsync(httpContext, schema);

        #endregion

        private static async Task AddSchemaInternalAsync(this HttpContext context, BaseEntitySchema schema)
        {
            if (context is null || schema is null || string.IsNullOrWhiteSpace(schema.SchemaName))
            {
                return;
            }

            var service = ServiceResolver.GetService<ISchemasService>();
            var schemaConfig = await service.GetConfigAsync().ConfigureAwait(false);
            if (!schemaConfig?.Enabled ?? true)
            {
                return;
            }

            var schemas = context.Items[CONTEXT_SCHEMAS_KEY] as Dictionary<string, IBaseEntitySchema>;
            if (schemas == null)
            {
                context.Items[CONTEXT_SCHEMAS_KEY] = schemas = new Dictionary<string, IBaseEntitySchema>();
            }

            schemas[schema.SchemaName] = schema;
        }

        public static IHtmlContent RenderSchemas(this IHtmlHelper _, HttpContext context)
        {
            var schemas = context.Items[CONTEXT_SCHEMAS_KEY] as Dictionary<string, IBaseEntitySchema>;
            if (schemas == null)
            {
                return HtmlString.Empty;
            }

            var generatedSchemas = new List<string>();
            var service = ServiceResolver.GetService<ISchemasService>();
            var urlRetriever = ServiceResolver.GetService<IWebPageUrlRetriever>();

            foreach (var schemaItem in schemas.Values.OfType<BaseEntitySchema>())
            {
                if (!string.IsNullOrEmpty(schemaItem.OverrideSchemaValue))
                {
                    generatedSchemas.Add(schemaItem.OverrideSchemaValue);
                    continue;
                }

                var schemaBuild = schemaItem.Build(service, urlRetriever).GetAwaiter().GetResult()?.ToString();
                if (!string.IsNullOrWhiteSpace(schemaBuild))
                {
                    generatedSchemas.Add(schemaBuild);
                }
            }

            if (!generatedSchemas.Any())
            {
                return HtmlString.Empty;
            }

            var schemaResult = new StringBuilder();
            schemaResult.Append("<script type=\"application/ld+json\">");

            if (generatedSchemas.Count() > 1)
            {
                schemaResult.Append('[')
                            .Append(string.Join(", ", generatedSchemas))
                            .Append(']');
            }
            else
            {
                schemaResult.Append(generatedSchemas[0]);
            }

            schemaResult.Append("</script>");

            var result = Regex.Replace(schemaResult.ToString(), @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1").Replace("scripttype", "script type");

            return new HtmlString(result);
        }
    }
}
