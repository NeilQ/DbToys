using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Netcool.DbToys.Core.Database;

public class PostgreSqlSchemaReader : SchemaReader
{

    public NpgsqlConnectionStringBuilder ConnectionStringBuilder { get; set; }

    public PostgreSqlSchemaReader() { }

    public PostgreSqlSchemaReader(NpgsqlConnectionStringBuilder builder)
    {
        ConnectionStringBuilder = builder;
    }

    public override string GetServerName()
    {
        return $"{ConnectionStringBuilder.Host}:{ConnectionStringBuilder.Port} - {ConnectionStringBuilder.Username}";
    }

    public override string Escape(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        return $"\"{text}\"";
    }

    public override List<string> ReadDatabases()
    {
        using var connection = new NpgsqlConnection(ConnectionStringBuilder.ConnectionString);
        using var cmd = new NpgsqlCommand
        {
            Connection = connection,
            CommandText = "SELECT datname FROM pg_database WHERE NOT datistemplate AND datname <> 'postgres'"
        };
        connection.Open();
        var dbs = new List<string>();
        {
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dbs.Add(rdr["datname"].ToString());
            }
        }
        connection.Close();
        return dbs;
    }

    public override List<Table> ReadTables(string database)
    {
        var result = new List<Table>();

        ConnectionStringBuilder.Database = database;
        using var connection = new NpgsqlConnection(ConnectionStringBuilder.ConnectionString);
        using var cmd = new NpgsqlCommand(TABLE_SQL, connection);
        connection.Open();
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var tbl = new Table
            {
                Database = database,
                Name = rdr["table_name"].ToString(),
                Schema = rdr["table_schema"].ToString(),
                IsView =
                    string.Compare(rdr["table_type"].ToString(), "View", StringComparison.OrdinalIgnoreCase) ==
                    0
            };
            tbl.CleanName = CleanUp(tbl.Name);
            tbl.ClassName = Inflector.MakeSingular(tbl.CleanName);
            tbl.DisplayName = $"{tbl.Schema}.{tbl.Name}";
            result.Add(tbl);
        }
        connection.Close();

        return result;
    }

    public override List<Column> ReadColumns(string database, string schema, string table)
    {
        ConnectionStringBuilder.Database = database;
        using var connection = new NpgsqlConnection(ConnectionStringBuilder.ConnectionString);
        using var cmd = new NpgsqlCommand { Connection = connection, CommandText = COLUMN_SQL };

        cmd.Parameters.AddWithValue("tableName", NpgsqlDbType.Text, table);
        var pks = GetPk(schema, table)?.ToHashSet() ?? new HashSet<string>();

        var result = new List<Column>();
        connection.Open();
        using IDataReader rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var col = new Column
            {
                Name = rdr["column_name"].ToString(),
                IsNullable = rdr["is_nullable"].ToString() == "YES",
                IsAutoIncrement = rdr["column_default"].ToString()?.StartsWith("nextval(") ?? false,
                DefaultValue = rdr["column_default"].ToString(),
                DbType = rdr["udt_name"].ToString(),
                Description = rdr["column_comment"].ToString()
            };
            col.PropertyName = ToPascalCase(CleanUp(col.Name));
            col.PropertyType = GetPropertyType(rdr["udt_name"].ToString());
            if (int.TryParse(rdr["character_maximum_length"].ToString(), out var length))
                col.Length = length;
            if (pks.Contains(col.Name))
                col.IsPk = true;

            result.Add(col);
        }
        connection.Close();

        return result;
    }

    public override DataTable GetResultSet(Table table, int limit)
    {
        ArgumentNullException.ThrowIfNull(table);
        ConnectionStringBuilder.Database = table.Database;
        var dtResult = new DataTable();
        using var conn = new NpgsqlConnection(ConnectionStringBuilder.ConnectionString);
        conn.Open();
        var sql = $"select * from {table.Schema}.{Escape(table.Name)}";
        if (table.Pk != null) sql += $" order by {Escape(table.Pk.Name)} desc";
        sql += $" limit {limit}";

        var cmd = new NpgsqlCommand(sql, conn);
        var adp = new NpgsqlDataAdapter(cmd);
        adp.Fill(dtResult);
        return dtResult;
    }

    public List<string> GetPk(string schema, string table)
    {
        var pks = new List<string>();
        var sql = @"SELECT a.attname
                        FROM pg_index i
                        JOIN pg_attribute a ON a.attrelid = i.indrelid
                        AND a.attnum = ANY(i.indkey)
                        WHERE i.indrelid = @tableName::regclass
                        AND i.indisprimary;";

        using var connection = new NpgsqlConnection(ConnectionStringBuilder.ConnectionString);
        using var cmd = new NpgsqlCommand { Connection = connection, CommandText = sql };

        cmd.Parameters.AddWithValue("tableName", NpgsqlDbType.Text, $"{schema}.{Escape(table)}");

        connection.Open();
        using var reader = cmd.ExecuteReader();
        if (!reader.HasRows) return pks;
        while (reader.Read())
        {
            pks.Add(reader["attname"].ToString());
        }

        connection.Close();
        return pks;
    }

    private string GetPropertyType(string sqlType)
    {
        switch (sqlType)
        {
            case "int2":
            case "int4":
            case "serial4":
            case "int8":
            case "serial8":
                return "int";
            case "bool":
                return "bool";
            case "bytea":
                return "byte[]";
            case "float8":
                return "double";
            case "money	":
                return "decimal";
            case "numeric":
                return "decimal";
            case "float4":
                return "float";
            case "time":
            case "timetz":
            case "timestamp":
            case "timestamptz":
            case "date":
                return "DateTime";

            default:
                return "string";
        }
    }

    const string TABLE_SQL = @"
            SELECT table_name, table_schema, table_type
            FROM information_schema.tables 
            WHERE (table_type='BASE TABLE' OR table_type='VIEW')
                AND table_schema NOT IN ('pg_catalog', 'information_schema');";

    const string COLUMN_SQL = @"
            SELECT cols.column_name, cols.is_nullable, cols.udt_name, cols.column_default, cols.character_maximum_length,
                (SELECT pg_catalog.col_description(c.oid, cols.ordinal_position::int)
                 FROM pg_catalog.pg_class c
                 WHERE c.relname = cols.table_name
                ) AS column_comment 
            FROM information_schema.columns cols
            WHERE cols.table_name=@tableName;";
}