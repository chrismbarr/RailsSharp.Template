using System.Web.Optimization;

namespace RailsSharp.Web
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
				"~/Scripts/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
				"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/dependency").Include(
				"~/Scripts/jquery-{version}.js",
				"~/Scripts/underscore.js",
				"~/Scripts/LogR.js",
				"~/Scripts/jMess.js",
				"~/Scripts/NeedsToMoveSomeday.js",
				"~/Scripts/bootstrap.js",
				"~/Scripts/respond.js"));

			bundles.Add(new ScriptBundle("~/bundles/app").Include(
				"~/Scripts/jquery.signalR-{version}.js",
				"~/Scripts/angular.js",
				"~/Scripts/angular-cookies.js",
				"~/App/TypeScriptEvents.js",
				"~/App/RootCtrl.js",
				"~/App/Users/*.js",
				"~/App/Application.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/bootstrap.css",
				"~/Content/site.css"));

			// Set EnableOptimizations to false for debugging. For more information,
			// visit http://go.microsoft.com/fwlink/?LinkId=301862
			BundleTable.EnableOptimizations = false;
		}
	}
}
