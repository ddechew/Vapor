namespace VaporWebAPI.Utils;

using RazorLight;

/// <summary>
/// Utility class for rendering Razor email templates to HTML strings.
/// </summary>
public static class EmailTemplateRenderer
{
    /// <summary>
    /// Renders a Razor email template with the specified model into an HTML string.
    /// </summary>
    /// <typeparam name="TModel">The type of the model passed to the template.</typeparam>
    /// <param name="templateName">A unique name used internally for RazorLight caching.</param>
    /// <param name="fileName">The file name of the Razor template (e.g., 'VerifySuccessEmail.cshtml').</param>
    /// <param name="model">The model to bind to the template.</param>
    /// <returns>An HTML string rendered from the template with the provided model.</returns>
    public static async Task<string> RenderAsync<TModel>(string templateName, string fileName, TModel model)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "DTOs", "EmailTemplates", fileName);
        var templateContent = await File.ReadAllTextAsync(templatePath);

        var engine = new RazorLightEngineBuilder()
            .UseMemoryCachingProvider()
            .Build();

        return await engine.CompileRenderStringAsync(templateName, templateContent, model);
    }
}