using System.Data;
using Microsoft.Data.SqlClient;

namespace DbToys.Core.Database;

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

    public override string Escape(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        return $"[{text}]";
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
                Database = database,
                Name = rdr["TABLE_NAME"].ToString(),
                Schema = rdr["TABLE_SCHEMA"].ToString(),
                Description = rdr["Description"].ToString(),
                IsView =
                    string.Compare(rdr["TABLE_TYPE"].ToString(), "View", StringComparison.OrdinalIgnoreCase) ==
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
        ConnectionStringBuilder.InitialCatalog = database;
        using var connection = new SqlConnection(ConnectionStringBuilder.ConnectionString);
        using var cmd = new SqlCommand(COLUMN_SQL, connection);

        var p = cmd.CreateParameter();
        p.ParameterName = "@tableName";
        p.Value = table;
        cmd.Parameters.Add(p);

        var pSchema = cmd.CreateParameter();
        pSchema.ParameterName = "@schemaName";
        pSchema.Value = schema;
        cmd.Parameters.Add(pSchema);

        var result = new List<Column>();
        connection.Open();
        using IDataReader rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var col = new Column
            {
                Name = rdr["ColumnName"].ToString(),
                PropertyType = GetPropertyType(rdr["DataType"].ToString()),
                IsNullable = rdr["IsNullable"].ToString() == "YES",
                DefaultValue = rdr["DefaultSetting"].ToString(),
                DbType = rdr["DataType"].ToString(),
                Description = rdr["Description"].ToString(),
                IsAutoIncrement = rdr["IsIdentity"].ToString() == "1",
                IsPk = rdr["IsPrimaryKey"].ToString() == "1"
            };
            col.PropertyName = CleanUp(col.Name);
            if (int.TryParse(rdr["MaxLength"].ToString(), out var length))
                col.Length = length;
       
            result.Add(col);
        }
        connection.Close();

        return result;
    }

    public override DataTable GetResultSet(Table table, int limit, string sort)
    {
        ArgumentNullException.ThrowIfNull(table);
        ConnectionStringBuilder.InitialCatalog = table.Database;
        var dtResult = new DataTable();
        using var conn = new SqlConnection(ConnectionStringBuilder.ConnectionString);
        conn.Open();
        var sql = $"select top {limit} * from {Escape(table.Name)}";
        if (!string.IsNullOrEmpty(sort)) sql += $" order by {sort}";
        var cmd = new SqlCommand(sql, conn);
        var adp = new SqlDataAdapter(cmd);
        adp.Fill(dtResult);

        return dtResult;
    }

    public List<string> GetPk(string schema, string table)
    {
        var sql = @"SELECT C.COLUMN_NAME FROM  
                      INFORMATION_SCHEMA.TABLE_CONSTRAINTS T  
                      JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C  
                      ON C.CONSTRAINT_NAME=T.CONSTRAINT_NAME  
                      WHERE C.TABLE_SCHEMA=@schema AND C.TABLE_NAME=@tableName AND T.CONSTRAINT_TYPE='PRIMARY KEY'";

        using var connection = new SqlConnection(ConnectionStringBuilder.ConnectionString);
        using var cmd = new SqlCommand(sql, connection);
        var pks = new List<string>();

        var p = cmd.CreateParameter();
        p.ParameterName = "@tableName";
        p.Value = table;
        cmd.Parameters.Add(p);

        var pSchema = cmd.CreateParameter();
        pSchema.ParameterName = "@schema";
        pSchema.Value = schema;
        cmd.Parameters.Add(pSchema);

        connection.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                pks.Add(reader["COLUMN_NAME"].ToString());
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

    const string TABLE_SQL = @"SELECT t.*,s.value as Description
        FROM  INFORMATION_SCHEMA.TABLES t
        LEFT JOIN Sys.Extended_Properties s 
        ON s.major_id= OBJECT_ID(t.TABLE_SCHEMA+'.'+t.TABLE_NAME)  AND s.minor_id=0 AND s.name='MS_Description'
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
            IsNull((Select top 1 1 From INFORMATION_SCHEMA.KEY_COLUMN_USAGE u Where u.Table_Name='person' And u.COLUMN_NAME=cols.COLUMN_NAME),0) IsPrimaryKey,
            s.value as Description
        FROM  INFORMATION_SCHEMA.COLUMNS cols
        LEFT OUTER JOIN sys.extended_properties s 
        ON 
            s.major_id = OBJECT_ID(cols.TABLE_SCHEMA+'.'+cols.TABLE_NAME) 
            AND s.minor_id = cols.ORDINAL_POSITION 
            AND s.name = 'MS_Description' 
        WHERE TABLE_NAME=@tableName AND TABLE_SCHEMA=@schemaName
        ORDER BY OrdinalPosition ASC";

}