using System.Web;
using System.Web.Optimization;

namespace YouConf
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.EnableOptimizations = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js", "~/Scripts/jquery-ui-timepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                        "~/Scripts/main.js",
                        "~/Scripts/main.easteregg.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/ajaxsolr").Include(
                "~/Scripts/ajax-solr/core/Core.js",
                "~/Scripts/ajax-solr/core/AbstractManager.js",
                "~/Scripts/ajax-solr/managers/Manager.jquery.js",
                "~/Scripts/ajax-solr/core/Parameter.js",
                "~/Scripts/ajax-solr/core/ParameterStore.js",
                "~/Scripts/ajax-solr/core/AbstractWidget.js",
                "~/Scripts/ajax-solr/widgets/ResultWidget.js",
                "~/Scripts/ajax-solr/widgets/jQuery/PagerWidget.js",
                "~/Scripts/ajax-solr/core/AbstractTextWidget.js",
                "~/Scripts/ajax-solr/widgets/TextWidget.js")
                );

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/normalize.css", "~/Content/site.*"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}