using RaceDay.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

using RaceDay.Services.Models;

namespace RaceDay.Services
{
    public class RaceDayClient
    {
        private const string COMMAND_LOGIN = "login";
        private const string COMMAND_MFUSER = "mfuser";
        private const string COMMAND_EVENT = "event";
        private const string COMMAND_ATTEND = "attend";

        #region Login
        public static async Task<AuthResult> Login(string emailInput, string passwordInput)
        {
            // Create the input model
            //
            var loginAuth = new LoginAuth
            {
                groupid = RaceDayConfiguration.Instance.APIGroup,
                email = emailInput,
                password = passwordInput,
                apikey = RaceDayConfiguration.Instance.APIKey
            };

            // Configure the REST client
            //
            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            var loginResult = await client.PostApi<AuthResult>(COMMAND_LOGIN, loginAuth, HttpStatusCode.OK);

            return loginResult;
        }

        public static async Task<HttpStatusCode> ForgotPassword(string email)
        {
            // Prepare the API input as URL parameters
            //
            var requestParms = $"groupid={HttpContext.Current.Server.UrlEncode(RaceDayConfiguration.Instance.APIGroup)}&apikey={HttpContext.Current.Server.UrlEncode(RaceDayConfiguration.Instance.APIKey)}&email={HttpContext.Current.Server.UrlEncode(email)}";

            // Configure the REST client
            //
            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            var noResponse = await client.GetApi<APIResult>(COMMAND_LOGIN + "?" + requestParms);

            return client.StatusCode;
        }

        public static async Task<HttpStatusCode> UpdatePassword(string Email, string Password)
        {
            var parms = new LoginAuth
            {
                groupid = RaceDayConfiguration.Instance.APIGroup,
                email = Email,
                password = Password,
                apikey = RaceDayConfiguration.Instance.APIKey,
            };

            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            await client.SimpleApi(COMMAND_LOGIN, "PUT", parms);

            return client.StatusCode;
        }
        #endregion

        #region Events
        public static async Task<List<EventAttending>> GetAllEventsForCurrentUser()
        {
            var token = await AppToken();
            if (token == null)
                return null;

            // Setup GET Rest Client
            //
            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            client.AddHeader("Authorization", $"Bearer {token}");

            // Get the event list
            //
            var events = await client.GetApi<List<EventAttending>>(COMMAND_EVENT);
            if (client.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await Authorize();
                if (token != null)
                {
                    client.ClearHeaders();
                    client.AddHeader("Authorization", $"Bearer {token}");
                    events = await client.GetApi<List<EventAttending>>(COMMAND_EVENT);
                }
            }

            return events;
        }

        public static async Task<EventDetail> GetEventDetail(int eventId)
        {
            var token = await AppToken();
            if (token == null)
                return null;

            // Setup GET Rest Client
            //
            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            client.AddHeader("Authorization", $"Bearer {token}");

            // Get the event details
            //
            var eventDetail = await client.GetApi<EventDetail>($"{COMMAND_EVENT}/{eventId}");
            if (client.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await Authorize();
                if (token != null)
                {
                    client.ClearHeaders();
                    client.AddHeader("Authorization", $"Bearer {token}");
                    eventDetail = await client.GetApi<EventDetail>($"{COMMAND_EVENT}/{eventId}");
                }
            }

            return eventDetail;
        }

        public static async Task AddUserToEvent(int eventId)
        {
            var token = await AppToken();
            if (token == null)
                return;

            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            client.AddHeader("Authorization", $"Bearer {token}");
            await client.PutApi($"{COMMAND_ATTEND}/{eventId}");
            if (client.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await Authorize();
                if (token != null)
                {
                    client.ClearHeaders();
                    client.AddHeader("Authorization", $"Bearer {token}");
                    await client.PutApi($"{COMMAND_ATTEND}/{eventId}");
                }
            }

            return;
        }

        public static async Task RemoveUserFromEvent(int eventId)
        {
            var token = await AppToken();
            if (token == null)
                return;

            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            client.AddHeader("Authorization", $"Bearer {token}");
            await client.DeleteApi($"{COMMAND_ATTEND}/{eventId}");
            if (client.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await Authorize();
                if (token != null)
                {
                    client.ClearHeaders();
                    client.AddHeader("Authorization", $"Bearer {token}");
                    await client.DeleteApi($"{COMMAND_ATTEND}/{eventId}");
                }
            }

            return;
        }

        public static async Task<EventDetail> AddEvent(string name, DateTime date, string url, string location, string description, string creatorid)
        {
            var token = await AppToken();
            if (token == null)
                return null;

            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            client.AddHeader("Authorization", $"Bearer {token}");

            var eventInfo = new JsonEvent
            {
                Name = name,
                Date = date,
                Url = url,
                Location = location,
                Description = description,
                CreatorId = creatorid
            };

            var result = await client.PostApi<EventDetail>(COMMAND_EVENT, eventInfo, HttpStatusCode.Created);
            if (client.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await Authorize();
                if (token != null)
                {
                    client.ClearHeaders();
                    client.AddHeader("Authorization", $"Bearer {token}");
                    result = await client.PostApi<EventDetail>(COMMAND_EVENT, eventInfo, HttpStatusCode.Created);
                }
            }
            return result;
        }
        #endregion

        #region MFUser
        public static async Task<List<JsonUser>> GetAllUsers()
        {
            var token = await AppToken();
            if (token == null)
                return null;

            // Setup GET Rest Client
            //
            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            client.AddHeader("Authorization", $"Bearer {token}");

            // Get the user list
            //
            var users = await client.GetApi<List<JsonUser>>(COMMAND_MFUSER);
            if (client.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await Authorize();
                if (token != null)
                {
                    client.ClearHeaders();
                    client.AddHeader("Authorization", $"Bearer {token}");
                    users = await client.GetApi<List<JsonUser>>(COMMAND_MFUSER);
                }
            }
            return users;
        }

        public static async Task<JsonUser> GetUserDetail(string userId)
        {
            var token = await AppToken();
            if (token == null)
                return null;

            // Setup GET Rest Client
            //
            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            client.AddHeader("Authorization", $"Bearer {token}");

            // Get the user details
            //
            var userDetail = await client.GetApi<JsonUser>($"{COMMAND_MFUSER}/{userId}");
            if (client.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await Authorize();
                if (token != null)
                {
                    client.ClearHeaders();
                    client.AddHeader("Authorization", $"Bearer {token}");
                    userDetail = await client.GetApi<JsonUser>($"{COMMAND_MFUSER}/{userId}");
                }
            }
            return userDetail;
        }

        public static async Task<HttpStatusCode> EditUser(string id, JsonUser user)
        {
            var token = await AppToken();
            if (token == null)
                return HttpStatusCode.Forbidden;

            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            client.AddHeader("Authorization", $"Bearer {token}");

            await client.SimpleApi($"{COMMAND_MFUSER}/{id}", "PUT", user);
            if (client.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await Authorize();
                if (token != null)
                {
                    client.ClearHeaders();
                    client.AddHeader("Authorization", $"Bearer {token}");
                    await client.SimpleApi($"{COMMAND_MFUSER}/{id}", "PUT", user);
                }
            }
            return client.StatusCode;
        }

        public static async Task<HttpStatusCode> UserCreate(string firstName, string lastName, string email, string password)
        {
            return await UserCreate($"{firstName} {lastName}", firstName, lastName, email, password);
        }
        public static async Task<HttpStatusCode> UserCreate(string name, string firstName, string lastName, string email, string password)
        {
            var token = await AppToken();
            if (token == null)
                return HttpStatusCode.Forbidden;

            // Create input
            //
            var newUser = new JsonUser
            {
                FirstName = firstName,
                LastName = lastName,
                Name = name,
                Email = email,
                Password = password,
                UserId = string.Empty
            };

            // Post to REST client and get response
            //
            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            client.AddHeader("Authorization", $"Bearer {token}");
            var userResult = await client.PostApi<string>(COMMAND_MFUSER, newUser, HttpStatusCode.Created);
            if (client.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await Authorize();
                if (token != null)
                {
                    client.ClearHeaders();
                    client.AddHeader("Authorization", $"Bearer {token}");
                    userResult = await client.PostApi<string>(COMMAND_MFUSER, newUser, HttpStatusCode.Created);
                }
            }

            return client.StatusCode;
        }

        public static async Task<HttpStatusCode> UserRegister(string code, string firstName, string lastName, string email, string password)
        {
            return await UserRegister(code, $"{firstName} {lastName}", firstName, lastName, email, password);
        }
        public static async Task<HttpStatusCode> UserRegister(string code, string name, string firstName, string lastName, string email, string password)
        {
            // Create input
            //
            var newUser = new JsonUser
            {
                FirstName = firstName,
                LastName = lastName,
                Name = name,
                Email = email,
                Password = password,
                UserId = string.Empty
            };

            // Post to REST client and get response
            //
            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            var userResult = await client.PostApi<string>($"{COMMAND_MFUSER}?code={code.ToUpper()}", newUser, HttpStatusCode.Created);

            return client.StatusCode;
        }

        public static async Task<HttpStatusCode>DeleteUser(string id)
        {
            var token = await AppToken();
            if (token == null)
                return HttpStatusCode.Forbidden;

            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            client.AddHeader("Authorization", $"Bearer {token}");

            await client.DeleteApi($"{COMMAND_MFUSER}/{id}");
            if (client.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await Authorize();
                if (token != null)
                {
                    client.ClearHeaders();
                    client.AddHeader("Authorization", $"Bearer {token}");
                    await client.DeleteApi($"{COMMAND_MFUSER}/{id}");
                }
            }
            return client.StatusCode;
        }
        #endregion

        #region Helpers

        public static async Task<string> AppToken()
        {
            return (!string.IsNullOrEmpty(RaceDayUser.CurrentUser.Token) ? RaceDayUser.CurrentUser.Token : await Authorize());
        }

        public static async Task<string> Authorize()
        {
            var auth = new LoginAuth
            {
                groupid = RaceDayConfiguration.Instance.APIGroup,
                email = RaceDayUser.CurrentUser.Email,
                apikey = RaceDayConfiguration.Instance.APIKey
            };

            var client = new RestClient(RaceDayConfiguration.Instance.APIUrl);
            var loginResult = await client.PostApi<AuthResult>(COMMAND_LOGIN, auth, HttpStatusCode.OK);

            if (loginResult != null)
            {
                RaceDayUser.LoginUser(loginResult, RaceDayUser.CurrentUser.IsPersistent);
            }

            return RaceDayUser.CurrentUser.Token;
        }

        #endregion
    }
}
