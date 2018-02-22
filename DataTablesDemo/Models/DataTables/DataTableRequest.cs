namespace DataTablesDemo.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTableColumn[] Columns { get; set; }
        public DataTableOrder[] Order { get; set; }
        public DataTableSearch Search { get; set; }

        public static DataTableRequest Parse(NameValueCollection queryString)
        {
            int draw;
            int start;
            int length;
            string searchValue;
            bool searchRegex;
            List<DataTableColumn> columns;
            List<DataTableOrder> order;

            int.TryParse(queryString["draw"], out draw);
            int.TryParse(queryString["start"], out start);
            int.TryParse(queryString["length"], out length);
            searchValue = queryString["search[value]"];
            bool.TryParse(queryString["search[regex]"], out searchRegex);
            columns = GetColumns(queryString);
            order = GetOrder(queryString);

            return new DataTableRequest
            {
                Draw = draw,
                Start = start,
                Length = length,
                Columns = columns.ToArray(),
                Order = order.ToArray(),
                Search = new DataTableSearch
                {
                    Value = searchValue,
                    Regex = searchRegex
                }
            };
        }

        public Dictionary<int, string> GetFilters()
        {
            var result = new Dictionary<int, string>();
            result = Columns
                .Where(c => c.Searchable && !string.IsNullOrEmpty(c.Search.Value))
                .ToDictionary(c => Array.IndexOf(Columns, c), c => c.Search.Value);
            return result;
        }

        private static List<DataTableColumn> GetColumns(NameValueCollection queryString)
        {
            List<DataTableColumn> columns = new List<DataTableColumn>();
            var columnsData =
                queryString.AllKeys.Where(k => k.StartsWith("columns["))
                    .Select(k =>
                    {
                        int indexStart = k.IndexOf("[") + 1;
                        int indexLength = k.IndexOf("]") - indexStart;
                        return new
                        {
                            index = int.Parse(k.Substring(indexStart, indexLength)),
                            property = k.Substring(indexStart + indexLength + 1),
                            value = queryString[k]
                        };
                    })
                    .GroupBy(i => i.index)
                    .OrderBy(i => i.Key);
            foreach (var i in columnsData)
            {
                DataTableColumn column;
                if (columns.Count < i.Key + 1)
                {
                    column = new DataTableColumn();
                    column.Search = new DataTableSearch();
                    columns.Add(column);
                }
                else
                {
                    column = columns[i.Key];
                }
                column.Data = int.Parse(i.Where(k => k.property.Contains("[data]")).Select(k => k.value).FirstOrDefault() ?? "0");
                column.Name = i.Where(k => k.property.Contains("[name]")).Select(k => k.value).FirstOrDefault();
                column.Searchable = bool.Parse(i.Where(k => k.property.Contains("[searchable]")).Select(k => k.value).FirstOrDefault() ?? "false");
                column.Orderable = bool.Parse(i.Where(k => k.property.Contains("[orderable]")).Select(k => k.value).FirstOrDefault() ?? "false");
                column.Search.Value = i.Where(k => k.property.Contains("[search][value]")).Select(k => k.value).FirstOrDefault();
                column.Search.Regex = bool.Parse(i.Where(k => k.property.Contains("[search][refex]")).Select(k => k.value).FirstOrDefault() ?? "false");
            }

            return columns;
        }

        private static List<DataTableOrder> GetOrder(NameValueCollection querystring)
        {
            DataTableOrderDirection orderDirection;
            List<DataTableOrder> order = new List<DataTableOrder>();
            var orderData =
                querystring.AllKeys.Where(k => k.StartsWith("order["))
                    .Select(k =>
                    {
                        int indexStart = k.IndexOf("[") + 1;
                        int indexLength = k.IndexOf("]") - indexStart;
                        return new
                        {
                            index = int.Parse(k.Substring(indexStart, indexLength)),
                            property = k.Substring(indexStart + indexLength + 1),
                            value = querystring[k]
                        };
                    })
                    .GroupBy(i => i.index)
                    .OrderBy(i => i.Key);
            foreach (var i in orderData)
            {
                DataTableOrder orderItem;
                if (order.Count < i.Key + 1)
                {
                    orderItem = new DataTableOrder();
                    order.Add(orderItem);
                }
                else
                {
                    orderItem = order[i.Key];
                }
                orderItem.Column = int.Parse(i.Where(k => k.property.Contains("[column]")).Select(k => k.value).FirstOrDefault() ?? "0");
                if (
                    Enum.TryParse(i.Where(k => k.property.Contains("[dir]")).Select(k => k.value).FirstOrDefault(),
                    true, out orderDirection))
                {
                    orderItem.Dir = orderDirection;
                }
                else
                {
                    orderItem.Dir = DataTableOrderDirection.Asc;
                }
            }

            return order;
        }
    }
}