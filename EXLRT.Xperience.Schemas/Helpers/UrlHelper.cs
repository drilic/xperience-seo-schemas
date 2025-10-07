using CMS.Helpers;
using Schema.NET;
using System.Text.RegularExpressions;

namespace EXLRT.Xperience.Schemas.Helpers
{
    public static class UrlHelper
    {
        public static Uri AsUri(this string candidate)
        {
            Uri.TryCreate(candidate, UriKind.RelativeOrAbsolute, out Uri uri);
            return uri;
        }

        public static OneOrMany<Uri> ClearAndBuildArrayOfURIs(this string input, char separator = ';', bool cleanNewLineChars = true)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<Uri>();
            }

            if (cleanNewLineChars)
            {
                input = Regex.Replace(input, @"\r\n?|\n", string.Empty);
            }

            return input.Split(separator)
                        .Select(link => link.AsUri())
                        .ToList();
        }

        internal static string BuildAbsoluteUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "#";
            }

            url = url.TrimStart('~');

            if (URLHelper.TryGetAbsoluteUrl(url, out var uri) && uri?.IsAbsoluteUri == true)
            {
                return url;
            }

            if (!url.EndsWith('/') && !url.Contains('?'))
            {
                url += "/";
            }

            return $"{RequestContext.URL.GetLeftPart(UriPartial.Authority)}/{url?.TrimStart('/')}".ToLower();
        }
    }
}
