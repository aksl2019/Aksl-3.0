using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Aksl.Data
{
    public static partial class SqlServerExtensions
    {
        public async static Task<bool> IsFullTextSupportedAsync(this DbContext dbContext)
        {
            //stringBuilder.Append("EXEC('");
            //stringBuilder.Append(" SELECT CASE SERVERPROPERTY(''IsFullTextInstalled'')");
            //stringBuilder.Append(" WHEN 1 THEN");
            //stringBuilder.Append("   CASE DatabaseProperty (DB_NAME(DB_ID()), ''IsFulltextEnabled'')");
            //stringBuilder.Append("   WHEN 1 THEN 1");
            //stringBuilder.Append("   ELSE 0");
            //stringBuilder.Append("   END");
            //stringBuilder.Append(" ELSE 0 ");
            //stringBuilder.Append(" END')");

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT CASE SERVERPROPERTY('IsFullTextInstalled')");
            stringBuilder.Append("   WHEN 1 THEN");
            stringBuilder.Append("   CASE DatabaseProperty (DB_NAME(DB_ID()), 'IsFulltextEnabled')");
            stringBuilder.Append("   WHEN 1 THEN 1");
            stringBuilder.Append("   ELSE 0");
            stringBuilder.Append("   END");
            stringBuilder.Append("   ELSE 0 ");
            stringBuilder.Append("   END");

            DbConnection dbConnection = dbContext.Database.GetDbConnection();
            await dbConnection.OpenAsync();
            DbCommand command = dbConnection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = stringBuilder.ToString();
            var result = await command.ExecuteScalarAsync();

            dbConnection.Close();
            dbConnection.Dispose();

            //var result =await dbContext.Database.ExecuteSqlCommandAsync(stringBuilder.ToString());

            // var result = dbContext.IntQueryTypes.FromSql("EXEC [FullText_IsSupported]")
            //var result = dbContext.Query<IntQueryType>().FromSql(stringBuilder.ToString())
            //              .Select(intValue => intValue.Value).FirstOrDefault();
            return (int)result > 0;
        }
    }
}
