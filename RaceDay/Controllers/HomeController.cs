using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using RaceDay.Services;
using RaceDay.Services.Models;
using RaceDay.Utilities;
using RaceDay.ViewModels;

namespace RaceDay.Controllers
{
	[HandleError(View = "Error")]
    [Authorize]
	public partial class HomeController : BaseController
    {
        // GET: /Home/Index
		//
		// Main page of the application.  Displays the list of events along with participation and also initializes the
		// event form for adding events.
		//
		public virtual async Task<ActionResult> Index()
        {
			IndexViewModel model = new IndexViewModel();

			model.EventForm.GroupCode = RaceDayConfiguration.Instance.APIGroup;

            // Get all events for the current user
            //
            List<EventAttending> events = await RaceDayClient.GetAllEventsForCurrentUser();
            model.Events = EventInfo.CopyFromService(events, RaceDayUser.CurrentUser.UserId);

            // Return the view
            //
            model.PageMessage = GetTempDataPageMessage();
            return View(model);
        }

		// GET: /Home/Pending
		//
		public virtual ActionResult Pending()
		{
			return View();
		}

		// POST: /Home/Event
		//
		// Event form has posted new event information to be added to the event list
		// ValidateInput - Razor view engine provides automatic HTML Encoding so allow all input values
		//   as they will be safely displayed in the view.
		//
		[HttpPost]
		[ValidateInput(false)]
		public virtual async Task<ActionResult> Event(IndexViewModel model)
		{
            if (ModelState.IsValid)
            {
                var newEvent = await RaceDayClient.AddEvent(
                    model.EventForm.EventName.Trim(),
                    model.EventForm.EventDate,
                    (model.EventForm.EventUrl != null ? model.EventForm.EventUrl.Trim() : null),
                    (model.EventForm.EventLocation != null ? model.EventForm.EventLocation.Trim() : null),
                    (model.EventForm.EventDescription != null ? model.EventForm.EventDescription.Trim() : null),
                    RaceDayUser.CurrentUser.UserId
                );
                if (newEvent != null)
                {
                    await RaceDayClient.AddUserToEvent(newEvent.eventinfo.EventId);

                    // redisplay new list positioning on the added event
                    //
                    return Redirect(VirtualPathUtility.ToAppRelative("~/") + "#e" + newEvent.eventinfo.EventId.ToString());
                }
                else
                {
                    model.PageMessage = new PageMessageModel(MessageDismissEnum.close, CssMessageClassEnum.error, "Unable to create event");
                }
            }

            return View(MVC.Home.Views.Index, model);
		}

		// POST: /Home/Attending
		//
		// Change attendance on a single event
		//
		[HttpPost]
		public virtual async Task<ActionResult> Attending(String EventId, String ClassName)
        {
            // Event Id must be an integer and found
            //
            Int32 eventId = 0;
            if (!Int32.TryParse(EventId, out eventId))
                eventId = 0;
            if (eventId == 0)
                return new HttpStatusCodeResult(400, "Invalid event Id");

            // ClassName must be recognized
            //
            if ((ClassName != "glyphicon-check") && (ClassName != "glyphicon-unchecked"))
                return new HttpStatusCodeResult(400, "Unrecognized class name");

            EventDetail eventDetail;

            // Switch the attendance
            //
            AttendanceResult result = new AttendanceResult();

            if (ClassName == "glyphicon-check")
            {
                await RaceDayClient.RemoveUserFromEvent(eventId);

                eventDetail = await RaceDayClient.GetEventDetail(eventId);
                result.Button = RenderPartialViewToString(MVC.Shared.Views.Partials._NotAttendingButton, EventInfo.CopyFromEventService(false, eventDetail));
            }
            else
            {
                await RaceDayClient.AddUserToEvent(eventId);

                eventDetail = await RaceDayClient.GetEventDetail(eventId);
                result.Button = RenderPartialViewToString(MVC.Shared.Views.Partials._AttendingButton, EventInfo.CopyFromEventService(true, eventDetail));
            }

            // Rebind the participant list with the change
            //
            List<EventParticipant> participants = new List<EventParticipant>();
            foreach (var user in eventDetail.attendees)
            {
                participants.Add(EventParticipant.FromJson(user));
            }

            result.Attendees = RenderPartialViewToString(MVC.Shared.Views.Partials._ParticipantList, participants);
            return Json(result);
		}

		// POST:  /Home/Participants
		//
		// Return participants for the given event.  Returns HTML from the partial view that renders participant information
		//
		[HttpPost]
		public virtual async Task<ActionResult> Participants(string EventId)
		{
            // Event Id must be an integer and found
            //
            Int32 eventId = 0;
            if (!Int32.TryParse(EventId, out eventId))
                eventId = 0;
            if (eventId == 0)
                return new HttpStatusCodeResult(400, "Invalid event Id");

            // Retrieve participants and format into appropriate view model
            //
            var attendings = await RaceDayClient.GetEventDetail(eventId);
            List<EventParticipant> participants = new List<EventParticipant>();

            foreach(var user in attendings.attendees)
            {
                participants.Add(EventParticipant.FromJson(user));
            }

            // return the rendered view with participants in it
            //
            AttendanceResult result = new AttendanceResult();
            result.Attendees = RenderPartialViewToString(MVC.Shared.Views.Partials._ParticipantList, participants);

            return Json(result);
		}

        [HttpGet]
        public virtual ActionResult MyProfile()
        {
            if (RaceDayUser.CurrentUser != null)
            {
                var userView = new ProfileViewModel
                {
                    UserId = RaceDayUser.CurrentUser.UserId,
                    Name = RaceDayUser.CurrentUser.Name,
                    FirstName = RaceDayUser.CurrentUser.FirstName,
                    LastName = RaceDayUser.CurrentUser.LastName,
                    Email = RaceDayUser.CurrentUser.Email,
                };
                return View(userView);
            }
            else
                return RedirectToAction("Index", "Home", null);
        }

        [HttpPost]
        public virtual async Task<ActionResult> MyProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new RaceDay.Services.Models.JsonUser
                {
                    UserId = model.UserId,
                    Name = model.Name.Trim(),
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    Email = model.Email.Trim()
                };
                var status = await RaceDayClient.EditUser(model.UserId, user);
                if (status == HttpStatusCode.OK)
                {
                    if (model.Password != null && !string.IsNullOrEmpty(model.Password.Trim()))
                    {
                        status = await RaceDayClient.UpdatePassword(model.Email, model.Password);
                        if (status == HttpStatusCode.OK)
                            model.PageMessage = new PageMessageModel(MessageDismissEnum.close, CssMessageClassEnum.success, "Profile with password updated");
                        else
                            model.PageMessage = new PageMessageModel(MessageDismissEnum.close, CssMessageClassEnum.error, "Unable to update password");
                    }
                    else
                        model.PageMessage = new PageMessageModel(MessageDismissEnum.close, CssMessageClassEnum.success, "Profile updated");

                    RaceDayUser.CurrentUser.Name = model.Name;
                    RaceDayUser.CurrentUser.FirstName = model.FirstName;
                    RaceDayUser.CurrentUser.LastName = model.LastName;
                    RaceDayUser.CurrentUser.Email = model.Email;
                }
                else
                    model.PageMessage = new PageMessageModel(MessageDismissEnum.close, CssMessageClassEnum.error, "Unable to update profile");
            }

            return View(model);
        }
    }
}
