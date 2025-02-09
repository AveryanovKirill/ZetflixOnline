
namespace TemplateEngine;

public interface ITemplator
{
    string GetHtmlByTemplate(string template, string name);
    string GetHtmlByTemplate<T>(string template, T obj);
}
