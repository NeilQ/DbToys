using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Netcool.Coding.Core.Database
{
    public class SqlServerSchemaReader : SchemaReader
    {
        public SqlConnectionStringBuilder ConnectionStringBuilder { get; set; }

        public SqlServerSchemaReader(SqlConnectionStringBuilder builder)
        {
            ConnectionStringBuilder = builder;
        }

        public override string GetServerName()
        {
            return $"{ConnectionStringBuilder.DataSource}({ConnectionStringBuilder.UserID})";
        }

        public override List<string> ReadDatabases()
        {
            var result = new List<string>();
            using var connection = new SqlConnection(ConnectionStringBuilder.ConnectionString);
            connection.Open();

            result.AddRange(from DataRow row in connection.GetSchema(SqlClientMetaDataCollectionNames.Databases).Rows select row[0].ToString());
            connection.Close();

            return result;
        }


        public override List<Table> ReadTables(string database)
        {
            var result = new List<Table>();

            ConnectionStringBuilder.InitialCatalog = database;
            using var connection = new SqlConnection(ConnectionStringBuilder.ConnectionString);
            using var cmd = new SqlCommand { Connection = connection, CommandText = TABLE_SQL };

            connection.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var tbl = new Table
                {
                    Name = rdr["TABLE_NAME"].ToString(),
                    Schema = rdr["TABLE_SCHEMA"].ToString(),
                    IsView =
                        string.Compare(rdr["TABLE_TYPE"].ToString(), "View", StringComparison.OrdinalIgnoreCase) ==
                        0
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
            ConnectionStringBuilder.InitialCatalog = database;
            using var connection = new SqlConnection(ConnectionStringBuilder.ConnectionString);
            using var cmd = new SqlCommand() { Connection = connection, CommandText = COLUMN_SQL };

            var p = cmd.CreateParameter();
            p.ParameterName = "@tableName";
            p.Value = tableName;
            cmd.Parameters.Add(p);

            p = cmd.CreateParameter();
            p.ParameterName = "@schemaName";
            p.Value = "dbo";
            cmd.Parameters.Add(p);

            var result = new List<Column>();
            connection.Open();
            using IDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Column col = new Column
                {
                    Name = rdr["ColumnName"].ToString(),
                    PropertyType = GetPropertyType(rdr["DataType"].ToString()),
                    IsNullable = rdr["IsNullable"].ToString() == "YES",
                    IsAutoIncrement = ((int)rdr["IsIdentity"]) == 1,
                    DefaultValue = rdr["DefaultSetting"].ToString(),
                    DbType = rdr["DataType"].ToString(),
                    Description = rdr["Description"].ToString()
                };
                col.PropertyName = CleanUp(col.Name);
                var lengthStr = rdr["MaxLength"].ToString();
                if (!string.IsNullOrEmpty(lengthStr))
                {
                    col.Length = int.Parse(lengthStr);
                }

                result.Add(col);
            }
            connection.Close();

            // todo: get pks

            return result;
        }


        public List<string> GetPk(string table)
        {
            var sql = @"SELECT c.name AS ColumnName
                FROM sys.indexes AS i 
                INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id 
                INNER JOIN sys.objects AS o ON i.object_id = o.object_id 
                LEFT OUTER JOIN sys.columns AS c ON ic.object_id = c.object_id AND c.column_id = ic.column_id
                WHERE (i.is_primary_key = 1) AND (o.name = @tableName)";

            using var connection = new SqlConnection(ConnectionStringBuilder.ConnectionString);
            using var cmd = new SqlCommand() { Connection = connection, CommandText = sql };
            var pks = new List<string>();

            var p = cmd.CreateParameter();
            p.ParameterName = "@tableName";
            p.Value = table;
            cmd.Parameters.Add(p);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    pks.Add(reader["ColumnName"].ToString());
                }
            }
            connection.Close();

            return pks;
        }

        public string GetPropertyType(string sqlType)
        {
            string sysType = "string";
            switch (sqlType)
            {
                case "bigint":
                    sysType = "long";
                    break;
                case "smallint":
                    sysType = "short";
                    break;
                case "int":
                    sysType = "int";
                    break;
                case "uniqueidentifier":
                    sysType = "Guid";
                    break;
                case "smalldatetime":
                case "datetime":
                case "datetime2":
                case "date":
                case "time":
                    sysType = "DateTime";
                    break;
                case "datetimeoffset":
                    sysType = "DateTimeOffset";
                    break;
                case "float":
                    sysType = "double";
                    break;
                case "real":
                    sysType = "float";
                    break;
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    sysType = "decimal";
                    break;
                case "tinyint":
                    sysType = "byte";
                    break;
                case "bit":
                    sysType = "bool";
                    break;
                case "image":
                case "binary":
                case "varbinary":
                case "timestamp":
                    sysType = "byte[]";
                    break;
                case "geography":
                    sysType = "Microsoft.SqlServer.Types.SqlGeography";
                    break;
                case "geometry":
                    sysType = "Microsoft.SqlServer.Types.SqlGeometry";
                    break;
            }
            return sysType;
        }

        const string TABLE_SQL = @"SELECT *
        FROM  INFORMATION_SCHEMA.TABLES
        WHERE TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW'
        ORDER BY TABLE_SCHEMA,TABLE_TYPE,TABLE_NAME";

        const string COLUMN_SQL = @"SELECT 
            TABLE_CATALOG AS [Database],
            TABLE_SCHEMA AS Owner, 
            TABLE_NAME AS TableName, 
            COLUMN_NAME AS ColumnName, 
            ORDINAL_POSITION AS OrdinalPosition, 
            COLUMN_DEFAULT AS DefaultSetting, 
            IS_NULLABLE AS IsNullable, DATA_TYPE AS DataType, 
            CHARACTER_MAXIMUM_LENGTH AS MaxLength, 
            DATETIME_PRECISION AS DatePrecision,
            COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsIdentity') AS IsIdentity,
            COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsComputed') as IsComputed,
            s.value as Description
        FROM  INFORMATION_SCHEMA.COLUMNS
        LEFT OUTER JOIN sys.extended_properties s 
        ON 
            s.major_id = OBJECT_ID(INFORMATION_SCHEMA.COLUMNS.TABLE_SCHEMA+'.'+INFORMATION_SCHEMA.COLUMNS.TABLE_NAME) 
            AND s.minor_id = INFORMATION_SCHEMA.COLUMNS.ORDINAL_POSITION 
            AND s.name = 'MS_Description' 
        WHERE TABLE_NAME=@tableName AND TABLE_SCHEMA=@schemaName
        ORDER BY OrdinalPosition ASC";

    }
}