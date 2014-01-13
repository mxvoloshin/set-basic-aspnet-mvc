using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using set_basic_aspnet_mvc.Domain.Entities;

namespace set_basic_aspnet_mvc.Helpers
{
    public static class DapperHelper
    {
        private static string ProperiesNamesAsColumns(IEnumerable<string> properties)
        {
            var result = string.Empty;
            foreach (var property in properties)
            {
                if (result.Length == 0)
                    result += property;
                else
                    result += String.Format(", {0}", property);
            }

            return result;
        }
        private static string ProperiesNamesAsValues(IEnumerable<string> properties)
        {
            var result = string.Empty;
            foreach (var property in properties)
            {
                if (result.Length == 0)
                    result += String.Format("@{0}",property);
                else
                    result += String.Format(", @{0}", property);
            }

            return result;
        }

        //todo: maybe not all properties needed, and we need to mark database properties with attribute to select them
        private static IEnumerable<PropertyInfo> GetEntityProperties<T>(bool exceptIdProperty) where T: BaseEntity
        {
            var result = typeof (T).GetProperties().ToList();
            
            if (exceptIdProperty)
                result = result.Where(x => x.Name.ToLower() != "id").ToList();
            
            return result;
        }

        public static string CreateInsertSql<T>() where T:BaseEntity
        {
            var entityName = typeof (T).Name;

            var properties = GetEntityProperties<T>(true);
            var propertiesNames = properties.Select(x => x.Name).ToList();

            var propertiesSql = ProperiesNamesAsColumns(propertiesNames);
            var valuesSql = ProperiesNamesAsValues(propertiesNames);
            
            var insertSql = String.Format("insert {0} ({1}) values({2})", entityName, propertiesSql, valuesSql);
            
            return insertSql;
        }

        public static string CreateDeleteSql<T>() where T : BaseEntity
        {
            var entityName = typeof(T).Name;
            return String.Format("delete from {0} where id = @id", entityName);
        }

        public static string CreateUpdateSql<T>() where T : BaseEntity
        {
            var entityName = typeof(T).Name;

            var properties = GetEntityProperties<T>(true);
            var propertiesNames = properties.Select(x => x.Name).ToList();

            string propertiesForUpdate = string.Empty;
            foreach (var propertyName in propertiesNames)
            {
                if (propertiesForUpdate.Length == 0)
                    propertiesForUpdate += string.Format("{0} = @{0}", propertyName);
                else
                    propertiesForUpdate += string.Format(", {0} = @{0}", propertyName);
            }
            var updateSql = String.Format("update {0} set {1} where id = @id", entityName, propertiesForUpdate);
            
            return updateSql;
        }

        public static string CreateUpdateSqlForSoftDelete<T>() where T : BaseEntity
        {
            var entityName = typeof(T).Name;
            var updateSql = String.Format("update {0} set DeletedBy = @DeletedBy, DeletedAt = @DeletedAt, IsDeleted = @IsDeleted" +
                                          " where id = @id", entityName);

            return updateSql;
        }
    }
}