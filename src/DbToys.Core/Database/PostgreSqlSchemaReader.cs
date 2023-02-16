using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace DbToys.Core.Database;

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
                Description = rdr["table_description"].ToString(),
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
        cmd.Parameters.AddWithValue("schema", NpgsqlDbType.Text, schema);

        var result = new List<Column>();
        connection.Open();
        using IDataReader rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var col = new Column
            {
                Name = rdr["column_name"].ToString(),
                IsNullable = rdr["is_nullable"].ToString() == "1",
                IsAutoIncrement =rdr["is_identity"].ToString()=="YES" || rdr["auto_increment"].ToString() == "1",
                DefaultValue = rdr["column_default"].ToString(),
                DbType = rdr["udt_name"].ToString(),
                Description = rdr["description"].ToString(),
                IsPk = rdr["is_primary_key"].ToString() == "1",
            };
            col.PropertyName = ToPascalCase(CleanUp(col.Name));
            col.PropertyType = GetPropertyType(col.DbType);
            if (int.TryParse(rdr["data_length"].ToString(), out var length) && length > 0)
                col.Length = length;

            result.Add(col);
        }
        connection.Close();

        return result;
    }

    public override DataTable GetResultSet(Table table, int limit, string sort)
    {
        ArgumentNullException.ThrowIfNull(table);
        ConnectionStringBuilder.Database = table.Database;
        var dtResult = new DataTable();
        using var conn = new NpgsqlConnection(ConnectionStringBuilder.ConnectionString);
        conn.Open();
        var sql = $"select * from {table.Schema}.{Escape(table.Name)}";
        if (!string.IsNullOrEmpty(sort)) sql += $" order by {sort}";
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
            SELECT t.table_name, t.table_schema,t.table_type,obj_description(c.oid) As table_description
            FROM information_schema.tables t
            LEFT JOIN pg_class c ON c.relname = t.table_name AND c.relkind = 'r'
            WHERE (table_type='BASE TABLE' OR table_type='VIEW')
                AND table_schema NOT IN ('pg_catalog', 'information_schema');";

    const string COLUMN_SQL = @"
              SELECT
      a.attname AS column_name,
      col.udt_name AS udt_name,
      COALESCE ( col.character_maximum_length, col.numeric_precision,- 1 ) AS data_length,
      col.numeric_scale AS scale,
      ( CASE a.attnotnull WHEN 't' THEN 0 ELSE 1 END ) AS is_nullable,
      ( CASE a.attnum WHEN cs.conkey [ 1 ] THEN 1 ELSE 0 END ) AS is_primary_key,
      ( CASE WHEN position( 'nextval' IN col.column_default ) > 0 THEN 1 ELSE 0 END ) AS auto_increment,
      col_description ( a.attrelid, a.attnum ) AS description,
      col.column_default,
      col.is_identity
      FROM
      pg_attribute a
      LEFT JOIN pg_class c ON a.attrelid = c.oid
      LEFT JOIN pg_constraint cs ON cs.conrelid = c.oid
      AND cs.contype = 'p'
      LEFT JOIN pg_namespace n ON n.oid = c.relnamespace
      LEFT JOIN information_schema.COLUMNS col ON col.table_schema = n.nspname
      AND col.table_name = c.relname
      AND col.column_name = a.attname
      WHERE
      a.attnum > 0
      AND col.udt_name IS NOT NULL
      AND c.relname = @tableName
      AND n.nspname = @schema
      order by a.attnum asc";
}