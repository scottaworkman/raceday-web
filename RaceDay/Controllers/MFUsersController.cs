using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using RaceDay.Models;
using RaceDay.Services;
using RaceDay.ViewModels;

namespace RaceDay.Controllers
{
    [RaceDay.AdminAttribute]
    [HandleError(View = "Error")]
    public partial class MFUsersController : BaseController
    {
        private RaceDayEntities db = new RaceDayEntities();

        // GET: MFUsers
        public virtual async Task<ActionResult> Index()
        {
            var users = await RaceDayClient.GetAllUsers();

            return View(users);
        }

        // GET: MFUsers/Details/5
        public virtual async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await RaceDayClient.GetUserDetail(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: MFUsers/Create
        public virtual ActionResult Create()
        {
            return View(new RaceDay.ViewModels.MFUserViewModel());
        }

        // POST: MFUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Create([Bind(Include = "Name,FirstName,LastName,Email,Password")] MFUserViewModel mFUser)
        {
            if (ModelState.IsValid)
            {
                await RaceDayClient.UserCreate(mFUser.Name, mFUser.FirstName, mFUser.LastName, mFUser.Email, mFUser.Password);
                return RedirectToAction("Index");
            }

            return View(mFUser);
        }

        // GET: MFUsers/Edit/5
        public virtual async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await RaceDayClient.GetUserDetail(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(new MFUserViewModel
            {
                UserId = user.UserId,
                Name = user.Name,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
            });
        }

        // POST: MFUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit(string id, [Bind(Include = "UserId,Name,FirstName,LastName,Email,Password")] MFUserViewModel mFUser)
        {
            if (ModelState.IsValid)
            {
                var user = await RaceDayClient.GetUserDetail(id);
                if (user != null)
                {
                    user.Name = mFUser.Name;
                    user.FirstName = mFUser.FirstName;
                    user.LastName = mFUser.LastName;
                    user.Email = mFUser.Email;
                    user.Password = mFUser.Password;

                    await RaceDayClient.EditUser(id, user);

                    return RedirectToAction("Index");
                }
            }
            return View(mFUser);
        }

        // GET: MFUsers/Delete/5
        public virtual async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await RaceDayClient.GetUserDetail(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: MFUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> DeleteConfirmed(string id)
        {
            await RaceDayClient.DeleteUser(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
