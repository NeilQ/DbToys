using System.Text;
using System.Text.Json;
using Netcool.DbToys.Core.Database;
using Scriban.Functions;

namespace Netcool.DbToys.Core.Scriban;

public class CustomScribanStringFunctions : StringFunctions
{
    public static string ToSingular(string text)
    {
        return Inflector.MakeSingular(text);
    }

    public static string ToPlural(string text)
    {
        return Inflector.MakePlural(text);
    }

    public static string ToCamelCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        if (text.Length < 2) return text.ToLowerInvariant();
        var sb = new StringBuilder();
        var chars = text.AsSpan();

        sb.Append(char.ToLowerInvariant(chars[0]));
        var newWords = false;
        for (var i = 1; i < chars.Length; ++i)
        {
            var c = chars[i];
            if (c is '_' or ' ')
            {
                newWords = true;
                continue;
            }

            if (newWords && char.IsAsciiLetter(c))
            {
                sb.Append(char.ToUpperInvariant(c));
                newWords = false;
                continue;
            }

            sb.Append(c);
        }
        return sb.ToString();
    }

    public static string ToPascalCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        if (text.Length < 2) return text.ToUpperInvariant();
        var sb = new StringBuilder();
        var chars = text.AsSpan();

        sb.Append(char.ToUpperInvariant(chars[0]));
        var newWords = false;
        for (var i = 1; i < chars.Length; ++i)
        {
            var c = chars[i];
            if (c is '_' or ' ')
            {
                newWords = true;
                continue;
            }

            if (newWords && char.IsAsciiLetter(c))
            {
                sb.Append(char.ToUpperInvariant(c));
                newWords = false;
                continue;
            }

            sb.Append(c);
        }
        return sb.ToString();
    }

    public static string ToSnakeCase(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        if (text.Length < 2) return text;
        var sb = new StringBuilder();
        var chars = text.AsSpan();

        sb.Append(char.ToLowerInvariant(chars[0]));
        for (var i = 1; i < chars.Length; ++i)
        {
            var c = chars[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}