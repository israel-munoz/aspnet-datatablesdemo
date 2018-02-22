using DataTablesDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace DataTablesDemo.Services
{
    public class PeopleService
    {
        private Person[] GetPeople()
        {
            string json = File.ReadAllText(HostingEnvironment.MapPath("~/App_Data/people.json"));
            var data = JsonConvert.DeserializeObject<Person[]>(json);
            return data;
        }

        public int GetTotalPeople()
        {
            return GetPeople().Length;
        }

        private void Addfilter(ref IEnumerable<Person> query, int column, string filter)
        {
            Func<Person, bool> filterFunc;
            switch (column)
            {
                default:
                    filterFunc = (p) => p.FirstName.Contains(filter, true);
                    break;
                case 1:
                    filterFunc = (p) => p.LastName.Contains(filter, true);
                    break;
                case 2:
                    filterFunc = (p) => p.Position.Contains(filter, true);
                    break;
                case 3:
                    filterFunc = (p) => p.Office.Contains(filter, true);
                    break;
                case 4:
                    filterFunc = (p) => p.StartDate.Contains(filter, true);
                    break;
                case 5:
                    filterFunc = (p) => p.Salary.Contains(filter, true);
                    break;
            }
            query = query.Where(filterFunc);
        }

        public Person[] GetPeople(
            int start,
            int length,
            string tableFilter,
            Dictionary<int, string> columnsFilter,
            Dictionary<int, int> order,
            out int total)
        {
            var query = GetPeople().AsEnumerable();
            if (columnsFilter != null)
            {
                foreach (var f in columnsFilter)
                {
                    Addfilter(ref query, f.Key, f.Value);
                }
            }

            if (!string.IsNullOrEmpty(tableFilter))
            {
                query = query.Where(item =>
                    item.FirstName.Contains(tableFilter, true) ||
                    item.LastName.Contains(tableFilter, true) ||
                    item.Position.Contains(tableFilter, true) ||
                    item.Office.Contains(tableFilter, true) ||
                    item.StartDate.Contains(tableFilter, true) ||
                    item.Salary.Contains(tableFilter, true));
            }

            int index = 0;
            foreach (var col in order)
            {
                if (index == 0)
                {
                    SetFirstSort(ref query, col.Key, col.Value);
                }
                else
                {
                    SetNextSort(ref query, col.Key, col.Value);
                }

                index += 1;
            }

            total = query.Count();

            query = query.Skip(start).Take(length);

            return query.ToArray();
        }

        private void SetFirstSort(ref IEnumerable<Person> query, int column, int direction)
        {
            Func<Person, string> filter = SetSortFunc(column);

            query = direction > 0
                ? query.OrderBy(filter)
                : query.OrderByDescending(filter);
        }

        private void SetNextSort(ref IEnumerable<Person> query, int column, int direction)
        {
            Func<Person, string> filter = SetSortFunc(column);

            query = direction > 0
                ? ((IOrderedEnumerable<Person>)query).ThenBy(filter)
                : ((IOrderedEnumerable<Person>)query).ThenByDescending(filter);
        }

        private Func<Person, string> SetSortFunc(int column)
        {
            switch (column)
            {
                case 1:
                    return (p) => p.LastName;
                case 2:
                    return (p) => p.Position;
                case 3:
                    return (p) => p.Office;
                case 4:
                    return (p) => p.StartDate;
                case 5:
                    return (p) => p.Salary;
                default:
                    return (p) => p.FirstName;
            }
        }
    }
}