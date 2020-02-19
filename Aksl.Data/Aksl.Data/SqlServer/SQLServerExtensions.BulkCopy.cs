using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Aksl.Data
{
    public static partial class SqlServerExtensions
    {
        public async static Task ExecuteSqlBulkCopyAync<TEntity>(this DbContext dbContext, List<TEntity> entitylList, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default, int? batchSize = null, int? bulkCopyTimeout = null) where TEntity : class
        {
            var destinationTableName = typeof(TEntity).Name;

            DbConnection dbConnection = dbContext.Database.GetDbConnection();
            //await dbConnection.OpenAsync();

            var sqlConnection = dbConnection as SqlConnection;

            DataTable dt = await GetDataTableAync(entitylList, sqlConnection, destinationTableName);
            await WriteToServerAsync();

            dbConnection.Close();
            dbConnection.Dispose();

            async Task WriteToServerAsync()
            {
                var bulkCopy = CreateSqlBulkCopy(); 

                await bulkCopy.WriteToServerAsync(dt);
            }

            SqlBulkCopy CreateSqlBulkCopy()
            {

                var bulkCopy = sqlBulkCopyOptions == SqlBulkCopyOptions.Default ? new SqlBulkCopy(sqlConnection) : new SqlBulkCopy(sqlConnection, sqlBulkCopyOptions, null);

                if (batchSize.HasValue)
                {
                    bulkCopy.BatchSize = batchSize.Value;
                }

                if (bulkCopyTimeout.HasValue)
                {
                    bulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                }

                bulkCopy.DestinationTableName = destinationTableName;

                return bulkCopy;
            }
        }

        private async static Task<DataTable> GetDataTableAync<TEntity>(List<TEntity> entityList, SqlConnection sqlConnection, string tableName)
        {
            DataTable dt = new DataTable();

            Type modelType = typeof(TEntity);

            List<(string Name, int ColOrder)> columns = await GetTableColumnsAync(sqlConnection, tableName);
            List<PropertyInfo> mappingProps = new List<PropertyInfo>();

            var props = modelType.GetProperties();
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                PropertyInfo mappingProp = props.FirstOrDefault(a => a.Name == column.Name);
                if (mappingProp == null)
                {
                    throw new Exception(string.Format("model 类型 '{0}'未定义与表 '{1}' 列名为 '{2}' 映射的属性", modelType.FullName, tableName, column.Name));
                }

                mappingProps.Add(mappingProp);
                Type dataType = GetUnderlyingType(mappingProp.PropertyType);
                if (dataType.IsEnum)
                {
                    dataType = typeof(int);
                }

                dt.Columns.Add(new DataColumn(column.Name, dataType));
            }

            foreach (var entity in entityList)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < mappingProps.Count; i++)
                {
                    PropertyInfo prop = mappingProps[i];
                    object value = prop.GetValue(entity);

                    if (GetUnderlyingType(prop.PropertyType).IsEnum)
                    {
                        if (value != null)
                        {
                            value = (int)value;
                        }
                    }

                    dr[i] = value ?? DBNull.Value;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        private static async Task<List<(string Name, int ColOrder)>> GetTableColumnsAync(SqlConnection sourceConnection, string tableName)
        {
            string sql = string.Format("select * from syscolumns inner join sysobjects on syscolumns.id=sysobjects.id where sysobjects.xtype='U' and sysobjects.name='{0}' order by syscolumns.colid asc", tableName);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT *");
            stringBuilder.Append("   FROM syscolumns INNER JOIN sysobjects ON syscolumns.id=sysobjects.id");
            stringBuilder.Append($"  WHERE sysobjects.xtype='U' AND sysobjects.name='{tableName}' ");
            stringBuilder.Append("   ORDER BY syscolumns.colid ASC");

            List<(string Name, int ColOrder)> columns = new List<(string Name, int ColOrder)>();

            var sqlConnection = (SqlConnection)((ICloneable)sourceConnection).Clone();
            await sqlConnection.OpenAsync();
            DbCommand command = sqlConnection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = stringBuilder.ToString();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                   // SysColumn column = new SysColumn();
                    (string Name, int ColOrder) column = default;
                    column.Name = reader.GetString("name");
                    column.ColOrder = reader.GetInt16("colorder");

                    columns.Add(column);
                }
            }

            sqlConnection.Close();
            sqlConnection.Dispose();

            return columns;
        }

        private static Type GetUnderlyingType(Type type)
        {
            Type unType = Nullable.GetUnderlyingType(type); ;
            if (unType == null)
            {
                unType = type;
            }

            return unType;
        }

        class SysColumn
        {
            public string Name { get; set; }
            public int ColOrder { get; set; }
        }
    }
}
