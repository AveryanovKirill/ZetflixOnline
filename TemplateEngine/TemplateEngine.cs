using System.Text;
using System.Text.RegularExpressions;
using TemplateEngine;

namespace TemplateEngine;



public class TemplateEngine
{
    public string Render(string template, object data)
    {
        template = ProcessLoops(template, data);

        var properties = data.GetType().GetProperties();
        var result = template;

        foreach (var property in properties)
        {
            var placeholder = $"{{{{{property.Name}}}}}";
            var value = property.GetValue(data);

            if (value is DateTime dateValue)
            {
                value = dateValue.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("ru-RU"));
            }

            result = result.Replace(placeholder, value?.ToString() ?? string.Empty);
        }

        return result;
    }

    private string ProcessLoops(string template, object data)
    {
        var regex = new Regex(@"\{\{#foreach (.*?)\}\}(.*?)\{\{\/foreach\}\}", RegexOptions.Singleline);
        return regex.Replace(template, match =>
        {
            var propertyName = match.Groups[1].Value;
            var loopContent = match.Groups[2].Value;

            var property = data.GetType().GetProperty(propertyName);
            if (property != null)
            {
                var collection = property.GetValue(data) as IEnumerable<object>;
                if (collection != null && collection.Any())
                {
                    var loopResult = string.Empty;
                    foreach (var item in collection)
                    {
                        loopResult += new TemplateEngine().Render(loopContent, item);
                    }
                    return loopResult;
                }
            }
            return string.Empty;
        });
    }

    private string ProcessConditions(string template, object data)
    {
        var regex = new Regex(@"\{\{#if (.*?)\}\}(.*?)\{\{\/if\}\}", RegexOptions.Singleline);
        return regex.Replace(template, match =>
        {
            var conditionProperty = match.Groups[1].Value;
            var conditionContent = match.Groups[2].Value;

            var property = data.GetType().GetProperty(conditionProperty);
            if (property != null)
            {
                var value = property.GetValue(data);
                if (value != null && (value is bool b && b || !(value is bool)))
                {
                    return new TemplateEngine().Render(conditionContent, data);
                }
            }
            return string.Empty;
        });
    }

    public string ProcessComments(string template, string replacementText, string comment)
    {
        var regex = new Regex($"<!--{comment}-->(.*?)<!-/{comment}-->", RegexOptions.Singleline);

        // Заменяем содержимое между комментариями на переданный текст
        string updatedTemplate = regex.Replace(template, match =>
        {
            return replacementText;
        });

        // Возвращаем новый HTML-код
        return updatedTemplate;
    }
}