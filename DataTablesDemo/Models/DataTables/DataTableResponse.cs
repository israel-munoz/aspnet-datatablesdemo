namespace DataTablesDemo.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DataTableResponse<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public IEnumerable<T> DataItems { set; private get; }
        public IEnumerable<string[]> Data =>
            DataItems.Select(item =>
                item.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(prop => prop.GetValue(item).ToString())
                        .ToArray());
    }
}