using RaceDay.Models;
using RaceDay.Services;
using RaceDay.Services.Models;
using RaceDay.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace RaceDay
{
    public class RaceDayUser : IPrincipal
    {
        protected IIdentity identity = null;
        protected string userId;
        protected GroupRoleEnum userRole;
        protected string userName;
        protected string userFirstname;
        protected string userLastname;
        protected string userEmail;
        protected Boolean ticketPersistent;

        public RaceDayUser(FormsIdentity identity, string userid, GroupRoleEnum role, string name, string firstname, string lastname, string email, bool persistent)
        {
            this.identity = identity;
            UserId = userid;
            Role = role;
            Name = name;
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            ticketPersistent = persistent;
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return identity; }
        }
        public bool IsInRole(string role)
        {
            return Role.ToString().Equals(role, StringComparison.OrdinalIgnoreCase);
        }
        public bool IsInRole(GroupRoleEnum role)
        {
            return IsInRole(role.ToString());
        }
        #endregion

        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        public GroupRoleEnum Role
        {
            get { return userRole; }
            set { userRole = value; }
        }
        public Boolean IsPersistent
        {
            get { return this.ticketPersistent; }
        }
        public string FirstName
        {
            get { return userFirstname; }
            set { userFirstname = value; }
        }
        public string LastName
        {
            get { return userLastname; }
            set { userLastname = value; }
        }
        public string Name
        {
            get { return userName; }
            set { userName = value; }
        }
        public string Email
        {
            get { return userEmail; }
            set { userEmail = value; }
        }
        public string Token
        {
            get { return identity.Name; }
        }

        public static RaceDayUser CurrentUser
        {
            get { return HttpContext.Current.User as RaceDayUser; }
        }

        public static void LoginUser(AuthResult user, Boolean persistent)
        {
            // Expiration date/time as determiend by the Remember Me checkbox
            //
            DateTime expires = DateTime.Now.AddHours(8);

            if (persistent)
                expires = DateTime.Now.AddYears(1);

            // Create the .NET authorization ticket
            //
            var authTicket = new FormsAuthenticationTicket(1, user.token, DateTime.Now, expires, persistent, user.userid + "|" + user.name + "|" + user.firstname + "|" + user.lastname + "|" + user.email + "|" + user.role);
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            if (persistent)
                authCookie.Expires = authTicket.Expiration;
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

            // Create the user before the next request is made
            //
            GroupRoleEnum userRole = GroupRoleEnum.member;
            try
            {
                userRole = EnumExtensions.EnumParse<GroupRoleEnum>(user.role);
            }
            catch (Exception) { }

            RaceDayUser webUser = new RaceDayUser(new FormsIdentity(authTicket), user.userid, (GroupRoleEnum)user.role, user.name, user.firstname, user.lastname, user.email, persistent);
            HttpContext.Current.User = webUser;
        }

        public static void LogoffUser()
        {
            if (HttpContext.Current.User != null)
            {
                HttpContext.Current.User = null;
                FormsAuthentication.SignOut();
            }
        }
    }
}