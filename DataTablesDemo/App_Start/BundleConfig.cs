namespace DataTablesDemo
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css/base").Include(
                "~/node_modules/bootstrap/dist/css/bootstrap.css",
                "~/node_modules/datatables.net-bs/css/dataTables.bootstrap.css"));

            bundles.Add(new ScriptBundle("~/js/base").Include(
                "~/node_modules/jquery/dist/jquery.js",
                "~/node_modules/boostrap/dist/js/bootstrap.js",
                "~/node_modules/datatables.net/js/jquery.dataTables.js",
                "~/node_modules/datatables.net-bs/js/dataTables.bootstrap.js"));
        }
    }
}