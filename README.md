# xperience-seo-schemas
The SEO and JSON-LD Showcase provides a lightweight implementation that injects structured data schemas into the page. It serves as a practical example for awareness and demonstration purposes, highlighting how schema markup can enhance SEO visibility...

![Image of generated structured data](https://github.com/drilic/xperience-seo-schemas/blob/master/documentation/code-img.png)

It includes several predefined schemas, but you can easily implement your own if needed. You can either modify the source code directly or create a custom schema and inject it on your page. See the _Setup_ section below for more details.

Older versions of Kentico provided more out-of-the-box data mapping. However, with the new modular architecture, it's difficult to reuse all that logic and automatically generate schemas. Because of this, and due to the removal of _SKUInfo_, the Product schema (although very important) was excluded. It requires too many data sets for a generic implementation. Every field can have custom naming, so for simplicity, it was left out. You can easily create your own Product schema and include all relevant information for your setup.

There is an _ISchemasService_ interface that needs to be implemented in your project to handle metadata fetching. You can configure your channel specific settings (see the [channel settings setup](https://github.com/drilic/xperience-ui-customsettings)) or map it in whatever way fits your setup best (appsettings, content type page...). All schemas will be built and rendered sequentially on the page. The rendering order doesn't matter, but each schema type must be unique. If multiple schemas of the same type are added, only the last one will be used. That's why the _SchemaName_ property (from the _IBaseEntitySchema_ interface) must always be unique.

In our earlier versions, there was also a Kentico UI for overriding schemas. However, it was removed due to the platform's major architectural changes. If you want to reintroduce this functionality, your page needs to implement ISchemaFields, and the value should be stored in _SEOPageSchemaOverride_ directly from your page model. 
```csharp
namespace DancingGoat.Models
{
	/// <summary>
	/// Represents a page of type <see cref="Store"/>.
	/// </summary>
	[RegisterContentTypeMapping(CONTENT_TYPE_NAME)]
	public partial class Store : IWebPageFieldsSource, ISEOFields, ISchemaFields
    {

        /// <summary>
        /// Comes from the custom dancing goat ISEOFields reusable field schema
        /// </summary>
        public string NewOverrideSchemaField { get; set; }

        /// <summary>
        /// Comes from the ISchemaFields, from schemas project.
        /// </summary>
        public string SEOPageSchemaOverride => this.NewOverrideSchemaField;

      ...
		}
}
```

You can use this as _Text below the input_ text to help users with validation (make sure to check 'Render HTML in the text...'):

> Enter valid JSON+LD object (without script tag). To validate it, use this <a href="https://validator.schema.org/" target="_blank">link</a>.

This project serves mainly as a **showcase and SEO awareness demo** for JSON-LD integration. Keep that in mind during testing, and consider adding caching if you experience performance issues.

## Requirements

- **Xperience by Kentico 30.9.2** - It has not been tested on older versions of Xperience, but it should work. Use it with caution...
- **net8.0** - as a long-term support (LTS) release
- **Schema.NET 13.0.0** - used for generating structured data schemas

## Download & Installation

1. Download source code
1. Include downloaded library into your project
   - Copy folder _EXLRT.Xperience.Schemas_ into your project
   - Add as exsiting project in Visual Studio
   - Add project reference to main project
1. Rebuild solution

## Setup

Add the following configuration in the head of the main layout (e.g. _Layout.cshtml):
```csharp
@using EXLRT.Xperience.Schemas.Extensions;

<head>
  ...
  @Html.RenderSchemas(Context)
  ...
</head>
```

Implement your own _ISchemasService_ to fetch the general required data:
```csharp
namespace DancingGoat.Services
{
    public class CustomSchemaService : ISchemasService
    {
        public Task<SchemasConfigurationModel> GetConfigAsync()
        {
             ...
        }

        public Task<IEnumerable<SchemasContactModel>> GetContactsAsync()
        {
             ...
        }

        public Task<SchemasLogoModel> GetLogoAsync()
        {
            ...
        }
    }
}

// In Program.cs register newly created Service
builder.Services.AddSchemas<CustomSchemaService>();
```

Use the following extensions to register schemas on pages and components:
```csharp
  public static void AddSchema(this IHttpContextAccessor contextAccessor, BaseEntitySchema schema);

  public static Task AddSchemaAsync(this IHttpContextAccessor contextAccessor, BaseEntitySchema schema);

  public static void AddSchema(this HttpContext httpContext, BaseEntitySchema schema);

  public static Task AddSchemaAsync(this HttpContext httpContext, BaseEntitySchema schema);
```

Tips:
* Add an Organization schema in your BaseController, so it's emitted on every page.
* Generate BreadcrumbList inside a ViewComponent by implementing the appropriate schema type there. For custom **component based** schemas use _BaseComponentEntitySchema_
* Attach page-specific schemas directly at the page level, depending on the content. For custom **page based** schemas use _BasePageEntitySchema_

Sample of usage on DancingGoat HomePage Controller:
```csharp
  public async Task<IActionResult> Index()
  {
      var homePage = await contentRetriever.RetrieveCurrentPage<HomePage>(
          new RetrieveCurrentPageParameters { LinkedItemsMaxLevel = 4 },
          HttpContext.RequestAborted
      );

      var cafes = await GetCafes(homePage);

      // schema builder
      await HttpContext.AddSchemaAsync(new OrganizationSchema(homePage));

      return View(HomePageViewModel.GetViewModel(homePage, cafes));
  }
```

## Contributions and Support

Feel free to fork and submit pull requests or report issues to contribute. Either this way or another one, we will look into them as soon as possible.
