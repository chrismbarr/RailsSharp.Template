using System.Web.Mvc;

namespace RailsSharp.Web.Controllers
{
	public partial class HomeController : Controller
	{
		public virtual ActionResult Index()
		{
			if (!Request.IsAuthenticated) return RedirectToAction(MVC.Account.ActionNames.Register, MVC.Account.Name);
			return View();
		}

		public virtual ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public virtual ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}