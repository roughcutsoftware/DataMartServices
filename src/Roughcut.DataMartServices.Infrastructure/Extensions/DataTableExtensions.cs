using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Roughcut.DataMartServices.Infrastructure.Extensions
{

    public static class DataTableExtensions
    {
        // remove "this" if not on C# 3.0 / .NET 3.5
        // http://stackoverflow.com/questions/564366/convert-generic-list-enumerable-to-datatable
        public static DataTable ToDataTable_DoesNotHandleNulls<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }

            //
            return table;
        }

        // remove "this" if not on C# 3.0 / .NET 3.5
        // http://stackoverflow.com/questions/564366/convert-generic-list-enumerable-to-datatable
        public static DataTable ToDataTable<T>(this IList<T> data)
        {

            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
