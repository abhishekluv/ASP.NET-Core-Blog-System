using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASPNETCoreBlog.Infrastructure
{
    public class HtmlWhitespaceMiddleware
    {
        private readonly RequestDelegate _next;

        public HtmlWhitespaceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request is for the admin area
            if (context.Request.Path.StartsWithSegments("/admin"))
            {
                // Skip minification for the admin area
                await _next(context);
                return;
            }

            var originalBodyStream = context.Response.Body;

            using (var newBodyStream = new MemoryStream())
            {
                context.Response.Body = newBodyStream;

                await _next(context);

                context.Response.Body = originalBodyStream;

                if (context.Response.ContentType != null && context.Response.StatusCode == 200 && context.Response.ContentType.Contains("text/html"))
                {
                    newBodyStream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(newBodyStream))
                    {
                        var responseBody = await reader.ReadToEndAsync();
                        var minifiedResponseBody = MinifyHtml(responseBody);

                        await context.Response.WriteAsync(minifiedResponseBody);
                    }
                }
                else
                {
                    newBodyStream.Seek(0, SeekOrigin.Begin);
                    await newBodyStream.CopyToAsync(originalBodyStream);
                }
            }
        }

        private string MinifyHtml(string html)
        {
            // Step 1: Extract all <pre> and <script> tag content and replace with placeholders
            var placeholders = new List<string>();
            html = ReplaceWithPlaceholder(html, @"<pre[^>]*>.*?</pre>", placeholders);
            //html = ReplaceWithPlaceholder(html, @"<script[^>]*>.*?</script>", placeholders);

            // Step 2: Minify the remaining HTML (outside of <pre> and <script> tags)
            html = Regex.Replace(html, @"\s{2,}", " "); // Reduce multiple spaces to one
            html = Regex.Replace(html, @">\s+<", "><"); // Remove spaces between tags
            html = Regex.Replace(html, @"\n|\r", "");   // Remove new lines
            html = html.Trim();

            // Step 3: Re-insert the <pre> and <script> tag content back into the HTML
            for (int i = 0; i < placeholders.Count; i++)
            {
                html = html.Replace($"<!--placeholder-{i}-->", placeholders[i]);
            }

            return html;
        }

        private string ReplaceWithPlaceholder(string html, string pattern, List<string> placeholders)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline);
            var matches = regex.Matches(html);

            foreach (Match match in matches)
            {
                placeholders.Add(match.Value);
                html = html.Replace(match.Value, $"<!--placeholder-{placeholders.Count - 1}-->");
            }

            return html;
        }
    }

}
