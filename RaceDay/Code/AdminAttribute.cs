using RaceDay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RaceDay
{
    public class AdminAttribute : AuthorizeAttribute
    {
        private RaceDayEntities db = new RaceDayEntities();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
            {
                return false;
            }

            var mfUser = httpContext.User as RaceDayUser;
            if (mfUser == null)
            {
                return false;
            }

            if (mfUser.IsInRole(GroupRoleEnum.admin))
            {
                return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Home",
                                action = "Index"
                            })
                        );
        }
    }
}