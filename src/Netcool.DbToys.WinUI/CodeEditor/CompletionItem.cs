namespace Netcool.DbToys.WinUI.CodeEditor;

public class CompletionItem
{
    public string Label { get; set; }
    public CompletionItemKind Kind { get; set; }
    public string Documentation { get; set; }
    public string InsertText { get; set; }

    public CompletionItem(string label)
    {
        Label = label;
        Kind = CompletionItemKind.Function;
        Documentation = label;
        InsertText = label;
    }
    public CompletionItem(string label, string documentation)
    {
        Label = label;
        Kind = CompletionItemKind.Function;
        Documentation = documentation;
        InsertText = label;
    }

    public CompletionItem(string label, CompletionItemKind kind)
    {
        Label = label;
        Kind = kind;
        Documentation = label;
        InsertText = label;
    }

    public CompletionItem(string label, CompletionItemKind kind, string documentation, string insertText)
    {
        Label = label;
        Kind = kind;
        Documentation = documentation;
        InsertText = insertText;
    }

    public static List<CompletionItem> CustomCompletionItems = new()
    {
        new("string.to_camal_case"),
        new("string.to_pascal_case"),
        new("string.to_snake_case"),
        new("string.to_singlar"),
        new("string.to_plural"),
    };

    public static List<CompletionItem> VariableCompletionItems = new()
    {
        new CompletionItem("table", CompletionItemKind.Variable),
        new CompletionItem("table.columns", CompletionItemKind.Property),
        new CompletionItem("table.name", CompletionItemKind.Property),
        new CompletionItem("table.clean_name", CompletionItemKind.Property),
        new CompletionItem("table.database", CompletionItemKind.Property),
        new CompletionItem("table.display_name", CompletionItemKind.Property),
        new CompletionItem("column.name", CompletionItemKind.Property),
        new CompletionItem("column.property_name", CompletionItemKind.Property),
        new CompletionItem("column.property_type", CompletionItemKind.Property),
        new CompletionItem("column.is_pk", CompletionItemKind.Property),
        new CompletionItem("column.is_nullable", CompletionItemKind.Property),
        new CompletionItem("column.is_auto_increment", CompletionItemKind.Property),
        new CompletionItem("column.db_type", CompletionItemKind.Property),
        new CompletionItem("column.length", CompletionItemKind.Property),
        new CompletionItem("column.description", CompletionItemKind.Property),
        new CompletionItem("column.default_value", CompletionItemKind.Property),
        new CompletionItem("col.name", CompletionItemKind.Property),
        new CompletionItem("col.property_name", CompletionItemKind.Property),
        new CompletionItem("col.property_type", CompletionItemKind.Property),
        new CompletionItem("col.is_pk", CompletionItemKind.Property),
        new CompletionItem("col.is_nullable", CompletionItemKind.Property),
        new CompletionItem("col.is_auto_increment", CompletionItemKind.Property),
        new CompletionItem("col.db_type", CompletionItemKind.Property),
        new CompletionItem("col.length", CompletionItemKind.Property),
        new CompletionItem("col.description", CompletionItemKind.Property),
        new CompletionItem("col.default_value", CompletionItemKind.Property)
     };

    public static List<CompletionItem> ScribanCompletionItems = new()
    {
        new("string.escape"),
        new("string.append"),
        new("string.capitalize"),
        new("string.capitalizewords"),
        new("string.contains"),
        new("string.empty"),
        new("string.whitespace"),
        new("string.downcase"),
        new("string.ends_with"),
        new("string.handleize"),
        new("string.literal"),
        new("string.lstrip"),
        new("string.pluralize"),
        new("string.prepend"),
        new("string.remove"),
        new("string.remove_first"),
        new("string.remove_last"),
        new("string.replace"),
        new("string.replace_first"),
        new("string.rstrip"),
        new("string.size"),
        new("string.slice"),
        new("string.slice1"),
        new("string.split"),
        new("string.starts_with"),
        new("string.strip"),
        new("string.strip_newlines"),
        new("string.to_int"),
        new("string.to_long"),
        new("string.to_float"),
        new("string.to_double"),
        new("string.truncate"),
        new("string.truncatewords"),
        new("string.upcase"),
        new("string.md5"),
        new("string.sha1"),
        new("string.sha256"),
        new("string.hmac_sha1"),
        new("string.hmac_sha256"),
        new("string.pad_left"),
        new("string.pad_right"),
        new("string.base64_encode"),
        new("string.base64_decode"),
        new("string.index_of"),

        new("array.add"),
        new("array.add_range"),
        new("array.compact"),
        new("array.concat"),
        new("array.cycle"),
        new("array.each"),
        new("array.filter"),
        new("array.first"),
        new("array.insert_at"),
        new("array.join"),
        new("array.last"),
        new("array.limit"),
        new("array.map"),
        new("array.offset"),
        new("array.remove_at"),
        new("array.reverse"),
        new("array.size"),
        new("array.sort"),
        new("array.uniq"),
        new("array.contains"),

        new("object.default"),
        new("object.eval"),
        new("object.eval_template"),
        new("object.format"),
        new("object.has_key"),
        new("object.has_value"),
        new("object.keys"),
        new("object.size"),
        new("object.typeof"),
        new("object.kind"),
        new("object.values"),

        new("math.abs"),
        new("math.ceil"),
        new("math.divided_by"),
        new("math.floor"),
        new("math.format"),
        new("math.is_number"),
        new("math.minus"),
        new("math.modulo"),
        new("math.plus"),
        new("math.round"),
        new("math.times"),
        new("math.uuid"),
        new("math.random"),

        new("regex.escape"),
        new("regex.match"),
        new("regex.matches"),
        new("regex.replace"),
        new("regex.split"),
        new("regex.unescape"),

        new("html.strip"),
        new("html.escape"),
        new("html.url_encode"),
        new("html.url_escape"),

        new("date.now"),
        new("date.add_days"),
        new("date.add_months"),
        new("date.add_years"),
        new("date.add_hours"),
        new("date.add_minutes"),
        new("date.add_seconds"),
        new("date.add_milliseconds"),
        new("date.parse"),
        new("date.to_string"),

        new("timespan.from_days"),
        new("timespan.from_hours"),
        new("timespan.from_minutes"),
        new("timespan.from_seconds"),
        new("timespan.from_milliseconds"),
        new("timespan.parse")
    };
}