using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using RaceDay.Models;

namespace RaceDay.Controllers
{
	public class MFUserController : ApiController
	{
		// POST api/<controller>
		//
		// Add user to the database
		//
		public HttpResponseMessage Post([FromBody]JsonUser value)
		{
			if (value == null)
				Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid user information");

			Models.Repository repository = new Models.Repository();
			MFUser fbUser = new MFUser
			{
				UserId = value.UserId,
				Email = value.Email,
				FirstName = value.FirstName, 
				LastName = value.LastName,
				Name = value.Name
			};
			var mfUser = repository.CreateUser(fbUser);
			repository.SaveChanges();

            List<GroupMember> membership = repository.UserMembership(mfUser);
            if ((membership == null) || (membership.Count == 0))
            {
                Group defaultGroup = repository.FindGroupByCode("JYMF");
                repository.DefaultGroup(mfUser, defaultGroup, GroupRoleEnum.member);
                repository.SaveChanges();
            }


            return Request.CreateResponse(HttpStatusCode.Created, "User added to application");
		}
	}
}