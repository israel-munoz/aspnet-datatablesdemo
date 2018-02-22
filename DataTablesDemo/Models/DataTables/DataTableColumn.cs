namespace DataTablesDemo.Models
{
    public class DataTableColumn
    {
        public int Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DataTableSearch Search { get; set; }
    }
}