﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

using RaceDay.Models;

namespace RaceDay
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication
	{
		public static InMemoryCache cache = null;

		protected void Application_Start()
		{
			cache = new InMemoryCache();

			AreaRegistration.RegisterAllAreas();

			GlobalConfiguration.Configure(WebApiConfig.Register);

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.AuthenticationType == "Forms")
                {
                    //--get the ticket
                    System.Web.Security.FormsIdentity id = (System.Web.Security.FormsIdentity)HttpContext.Current.User.Identity;
                    FormsAuthenticationTicket ticket = id.Ticket;

                    //--get the stored user data spliced  (User Id|User Role)
                    string[] userData = ticket.UserData.Split('|');

                    string userId = userData[0];
                    string userName = userData[1];
                    string userFirst = userData[2];
                    string userLast = userData[3];
                    string userEmail = userData[4];

                    if (userData.Length >= 6)
                    {
                        GroupRoleEnum userRole = GroupRoleEnum.member;

                        try
                        {
                            userRole = EnumExtensions.EnumParse<GroupRoleEnum>(userData[5]);
                        }
                        catch (Exception) { }

                        RaceDayUser user = new RaceDayUser(id, userId, userRole, userName, userFirst, userLast, userEmail, ticket.IsPersistent);
                        HttpContext.Current.User = user;
                    }
                }
            }
        }

        /// <summary>
        /// returns an absolute url for the current application
        /// </summary>
        /// <returns></returns>
        public static string GetAbsoluteHref()
		{
			return GetAbsoluteHref(HttpContext.Current);
		}

		/// <summary>
		/// returns an absolute url for the current application
		/// </summary>
		/// <returns></returns>
		public static string GetAbsoluteHref(HttpContext context)
		{
			HttpRequest req = context.Request;

			string host = req.Url.Host;

			if (host == "localhost")
				host += ":" + req.Url.Port.ToString();

			string href = "http://" + (host).Replace("//", "/");

			if (href.Substring(href.Length - 1, 1) != "/")
				href += "/";

			if (req.IsSecureConnection)
				href = href.Replace("http://", "https://");

			return href;
		}

		/// <summary>
		/// creates an absolute href for a file.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static string ConvertToAbsoluteHref(string filePath)
		{
			//chop a leading /
			if (!String.IsNullOrEmpty(filePath))
			{
				if (filePath.Substring(0, 1) == "/")
					filePath = filePath.Substring(1);
			}

			return GetAbsoluteHref() + filePath;
		}
	}
}