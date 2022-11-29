using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Netcool.Coding.Core.Database
{
    public class MySqlSchemaReader : SchemaReader
    {
        public MySqlConnectionStringBuilder ConnectionStringBuilder { get; set; }

        public MySqlSchemaReader(MySqlConnectionStringBuilder builder)
        {
            ConnectionStringBuilder = builder;
        }

        public override string GetServerName()
        {
            return $"{ConnectionStringBuilder.Server}({ConnectionStringBuilder.UserID})";
        }

        public override List<string> ReadDatabases()
        {
            using var connection = new MySqlConnection(ConnectionStringBuilder.ConnectionString);
            using var cmd = new MySqlCommand()
            {
                Connection = connection,
                CommandText = "SHOW DATABASES;"
            };

            var dbs = new List<string>();
            connection.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dbs.Add(rdr["DataBase"].ToString());
            }
            connection.Close();

            return dbs;
        }

        public override List<Table> ReadTables(string database)
        {
            ConnectionStringBuilder.Database = database;
            var result = new List<Table>();

            using var connection = new MySqlConnection(ConnectionStringBuilder.ConnectionString);
            using var cmd = new MySqlCommand()
            {
                Connection = connection,
                CommandText = TABLE_SQL
            };

            connection.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Table tbl = new Table
                {
                    Name = rdr["TABLE_NAME"].ToString(),
                    Schema = rdr["TABLE_SCHEMA"].ToString(),
                    IsView = string.Compare(rdr["TABLE_TYPE"].ToString(), "View",
                        StringComparison.OrdinalIgnoreCase) == 0
                };
                tbl.CleanName = CleanUp(tbl.Name);
                tbl.ClassName = Inflector.MakeSingular(tbl.CleanName);
                result.Add(tbl);
            }
            connection.Close();

            return result;
        }

        public override List<Column> ReadColumns(string database, string tableName)
        {
            ConnectionStringBuilder.Database = database;
            using var connection = new MySqlConnection(ConnectionStringBuilder.ConnectionString);
            using var cmd = new MySqlCommand
            {
                Connection = connection,
                CommandText = TABLE_SQL
            };

            var result = new List<Column>();

            var schema = connection.GetSchema("COLUMNS");

            //loop again - but this time pull by table name
            foreach (var item in result)
            {
                //pull the columns from the schema
                var columns = schema.Select("TABLE_NAME='" + item.Name + "'");
                foreach (var row in columns)
                {
                    var lengthStr = row["CHARACTER_MAXIMUM_LENGTH"].ToString();
                    var col = new Column
                    {
                        Name = row["COLUMN_NAME"].ToString(),
                        PropertyType = GetPropertyType(row),
                        IsNullable = row["IS_NULLABLE"].ToString() == "YES",
                        IsPk = row["COLUMN_KEY"].ToString() == "PRI",
                        IsAutoIncrement = row["extra"].ToString().ToLower()
                            .IndexOf("auto_increment", StringComparison.Ordinal) >= 0,
                        DefaultValue = row["COLUMN_DEFAULT"].ToString(),
                        DbType = row["DATA_TYPE"].ToString(),
                        Description = row["COLUMN_COMMENT"].ToString()
                    };
                    col.PropertyName = CleanUp(col.Name);
                    if (!string.IsNullOrEmpty(lengthStr))
                    {
                        if (int.TryParse(lengthStr, out _))
                        {
                            col.Length = int.Parse(lengthStr);
                        }
                    }
                    result.Add(col);
                }
            }

            return result;
        }

        static string GetPropertyType(DataRow row)
        {
            bool bUnsigned = row["COLUMN_TYPE"].ToString().IndexOf("unsigned", StringComparison.Ordinal) >= 0;
            string propType = "string";
            switch (row["DATA_TYPE"].ToString())
            {
                case "bigint":
                    propType = bUnsigned ? "ulong" : "long";
                    break;
                case "int":
                    propType = bUnsigned ? "uint" : "int";
                    break;
                case "smallint":
                    propType = bUnsigned ? "ushort" : "short";
                    break;
                case "guid":
                    propType = "Guid";
                    break;
                case "smalldatetime":
                case "date":
                case "datetime":
                case "timestamp":
                    propType = "DateTime";
                    break;
                case "float":
                    propType = "float";
                    break;
                case "double":
                    propType = "double";
                    break;
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    propType = "decimal";
                    break;
                case "bit":
                case "bool":
                case "boolean":
                    propType = "bool";
                    break;
                case "tinyint":
                    propType = bUnsigned ? "byte" : "sbyte";
                    break;
                case "image":
                case "binary":
                case "blob":
                case "mediumblob":
                case "longblob":
                case "varbinary":
                    propType = "byte[]";
                    break;

            }
            return propType;
        }

        const string TABLE_SQL = @"
            SELECT * 
            FROM information_schema.tables 
            WHERE (table_type='BASE TABLE' OR table_type='VIEW') AND TABLE_SCHEMA=DATABASE()
            ";

    }
}