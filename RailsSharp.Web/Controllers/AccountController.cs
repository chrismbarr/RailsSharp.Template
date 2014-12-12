using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Microsoft.Owin.Security;
using RailsSharp.Backend;
using RailsSharp.Backend.Users;
using RailsSharp.Web.Models;

namespace RailsSharp.Web.Controllers
{
	[Authorize]
	public partial class AccountController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly IUserCreationService _userCreationService;

		public AccountController(IUserRepository userRepository, IUserCreationService userCreationService)
		{
			_userRepository = userRepository;
			_userCreationService = userCreationService;
		}

		public void CreateAuthenticationTicket(string username)
		{
			var user = _userRepository.Find(username);

			//todo: Auth: actually get roles and put the correct ones in for this user.
			var serializeModel = new CustomPrincipleSerializedModel
			{
				Roles = user.Roles.Select(x => x.ToString()).ToArray(),
				CustomerId = user.CustomerId.ToString()
			};

			var serializer = new JavaScriptSerializer();
			var userData = serializer.Serialize(serializeModel);

			var authTicket = new FormsAuthenticationTicket(
			  1, username, DateTime.Now, DateTime.Now.AddHours(8), false, userData);
			var encTicket = FormsAuthentication.Encrypt(authTicket);
			var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
			Response.Cookies.Add(faCookie);
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public virtual ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid) return View(model);

			if (!_userRepository.Authenticate(model.Email, model.Password))
			{
				ModelState.AddModelError("", "The user name or password provided is incorrect.");
				return View(model);
			}

			//todo: consider adding logic to require email activation before login?
			CreateAuthenticationTicket(model.Email);
			return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public virtual ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Register(RegisterViewModel model)
		{

			if (ModelState.IsValid)
			{
				try
				{
					this._userCreationService.Create(model.Email, model.Password);
					return RedirectToAction("Index", "Home");
				}
				catch (Exception)
				{
					//redisplay the login form
					return View(model);
				}
			}
			//redisplay the login form
			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}


		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}