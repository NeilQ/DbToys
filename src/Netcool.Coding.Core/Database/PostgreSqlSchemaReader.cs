using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;

namespace Netcool.Coding.Core.Database
{
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

        public override List<Table> ReadTables(string dbName)
        {
            var result = new List<Table>();

            ConnectionStringBuilder.Database = dbName;
            using var connection = new NpgsqlConnection(ConnectionStringBuilder.ConnectionString);
            using var cmd = new NpgsqlCommand { Connection = connection, CommandText = TABLE_SQL };
            connection.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var tbl = new Table
                {
                    Name = rdr["table_name"].ToString(),
                    Schema = rdr["table_schema"].ToString(),
                    IsView =
                        string.Compare(rdr["table_type"].ToString(), "View", StringComparison.OrdinalIgnoreCase) ==
                        0
                };
                tbl.CleanName = CleanUp(tbl.Name);
                tbl.ClassName = Inflector.MakeSingular(tbl.CleanName);
                result.Add(tbl);
            }
            connection.Close();

            return result;
        }

        public override List<Column> ReadColumns(string database,string tableName)
        {
            ConnectionStringBuilder.Database = database;
            using var connection = new NpgsqlConnection(ConnectionStringBuilder.ConnectionString);
            using var cmd = new NpgsqlCommand { Connection = connection, CommandText = COLUMN_SQL };

            var p = cmd.CreateParameter();
            p.ParameterName = "@tableName";
            p.Value = tableName;
            cmd.Parameters.Add(p);

            var result = new List<Column>();
            connection.Open();
            using IDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Column col = new Column
                {
                    Name = rdr["column_name"].ToString(),
                    IsNullable = rdr["is_nullable"].ToString() == "YES",
                    IsAutoIncrement = rdr["column_default"].ToString().StartsWith("nextval("),
                    DefaultValue = rdr["column_default"].ToString(),
                    DbType = rdr["udt_name"].ToString(),
                    Description = rdr["column_comment"].ToString()
                };
                col.PropertyName = ToPascalCase(CleanUp(col.Name));
                col.PropertyType = GetPropertyType(rdr["udt_name"].ToString());
                result.Add(col);
            }
            connection.Close();
            // todo get pk

            return result;
        }

        public List<string> GetPk(string table)
        {
            var pks = new List<string>();
            var sql = @"SELECT kcu.column_name 
            FROM information_schema.key_column_usage kcu
            JOIN information_schema.table_constraints tc
            ON kcu.constraint_name=tc.constraint_name
            WHERE lower(tc.constraint_type)='primary key'
            AND kcu.table_name=@tablename";

            using var connection = new NpgsqlConnection(ConnectionStringBuilder.ConnectionString);
            using var cmd = new NpgsqlCommand { Connection = connection, CommandText = sql };

            var p = cmd.CreateParameter();
            p.ParameterName = "@tableName";
            p.Value = table;
            cmd.Parameters.Add(p);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.HasRows) return pks;
            while (reader.Read())
            {
                pks.Add(reader["column_name"].ToString());
            }

            connection.Close();
            return pks;
        }

        private string GetPropertyType(string sqlType)
        {
            switch (sqlType)
            {
                case "int8":
                    return "int";
                case "serial8":
                    return "int";

                case "bool":
                    return "bool";

                case "bytea	":
                    return "byte[]";

                case "float8":
                    return "double";

                case "int4":
                case "serial4":
                    return "int";

                case "money	":
                    return "decimal";

                case "numeric":
                    return "decimal";

                case "float4":
                    return "float";

                case "int2":
                    return "int";

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
            SELECT cols.column_name, cols.is_nullable, cols.udt_name, cols.column_default,
                (SELECT pg_catalog.col_description(c.oid, cols.ordinal_position::int)
                 FROM pg_catalog.pg_class c
                 WHERE c.relname = cols.table_name
                ) AS column_comment 
            FROM information_schema.columns cols
            WHERE cols.table_name=@tableName;";
    }
}