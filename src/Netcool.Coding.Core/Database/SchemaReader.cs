using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Netcool.Coding.Core.Database
{
    public interface ISchemaReader
    {
        string GetServerName();

        List<string> ReadDatabases();

        List<Table> ReadTables(string database);

        List<Column> ReadColumns(string database, string tableName);
    }

    public abstract class SchemaReader : ISchemaReader
    {
        static readonly Regex RxCleanUp = new Regex(@"[^\w\d_]", RegexOptions.Compiled);

        static readonly string[] CsKeywords =
        {
            "abstract", "event", "new", "struct", "as", "explicit", "null",
            "switch", "base", "extern", "object", "this", "bool", "false", "operator", "throw",
            "break", "finally", "out", "true", "byte", "fixed", "override", "try", "case", "float",
            "params", "typeof", "catch", "for", "private", "uint", "char", "foreach", "protected",
            "ulong", "checked", "goto", "public", "unchecked", "class", "if", "readonly", "unsafe",
            "const", "implicit", "ref", "ushort", "continue", "in", "return", "using", "decimal",
            "int", "sbyte", "virtual", "default", "interface", "sealed", "volatile", "delegate",
            "internal", "short", "void", "do", "is", "sizeof", "while", "double", "lock",
            "stackalloc", "else", "long", "static", "enum", "namespace", "string"
        };

        public abstract string GetServerName();

        public abstract List<string> ReadDatabases();
        public abstract List<Column> ReadColumns(string database, string tableName);

        public abstract List<Table> ReadTables(string database);

        /// <summary>
        /// Convert value to Pascal case.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected static string ToPascalCase(string value)
        {
            // If there are 0 or 1 characters, just return the string.
            if (value == null) return null;
            if (value.Length < 2) return value.ToUpper();

            // Split the string into words.
            var words = value.Split(
                new[] { '_' },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            var result = "";
            foreach (var word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
            }

            return result;
        }

        protected static Func<string, string> CleanUp = (str) =>
        {
            str = RxCleanUp.Replace(str, "_");

            if (char.IsDigit(str[0]) || CsKeywords.Contains(str))
                str = "@" + str;

            return str;
        };

    }
}
