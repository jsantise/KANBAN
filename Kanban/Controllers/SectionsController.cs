using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kanban.DAL;
using Kanban.Models;
using Microsoft.AspNet.Identity;
namespace Kanban.Controllers
{
    public class SectionsController : Controller
    {
        private KanbanContext db = new KanbanContext();

        // POST: Sections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ID,BoardID,Title")] Section section)
        {
            if (ModelState.IsValid)
            {
                Board board = db.Boards.Find(section.BoardID);
                if (board == null)
                    return HttpNotFound();
                if (board.OwnerID != User.Identity.GetUserId())
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                section.Order = board.Sections.Count() + 1;
                section.ParentID = 0; // new sections start at the top level
                section.Title = section.Title.Trim();
                db.Sections.Add(section);
                db.SaveChanges();
                return RedirectToAction("Index", "Boards", new { id = section.BoardID, sectionOpen = true });
            }

            return RedirectToAction("Index", "Boards", new { id = section.BoardID, errMessage = "Could not create section, model not vaild", sectionOpen = true });
        }

        // POST: Sections/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title")] Section section)
        {
            if (ModelState.IsValid)
            {
                Section origSection = db.Sections.Find(section.ID);
                if (origSection == null)
                    return HttpNotFound();
                if (origSection.Board.OwnerID != User.Identity.GetUserId())
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                origSection.Title = section.Title.Trim();
                db.Entry(origSection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Boards", new { id = origSection.BoardID, sectionEditOpen = true });
            }

            return RedirectToAction("Index", "Boards", new { id = section.BoardID, errMessage = "Could not edit section, model not vaild", sectionEditOpen = true });

        }

        [HttpPost]
        [Authorize]
        public ActionResult Reorder(string serializedData, int BoardID, int ParentID)
        {
            // this is called when the sections are reordered
            string errMessage = "";
            Board board = db.Boards.Find(BoardID);
            if (board == null)
            {
                errMessage = "Could not find board - " + BoardID.ToString();
            }
            else if (board.OwnerID != User.Identity.GetUserId())
            {
                errMessage = "Not authorized as owner";
            }
            else if (!String.IsNullOrEmpty(serializedData))
            {
                string[] sections = serializedData.Split('&');
                int[] order = new int[sections.Length];
                int index = 0;
                foreach (string s in sections)
                {
                    // get the new indexes from the serialized data
                    order[index] = int.Parse(s.Split('=')[1]);
                    index++;
                }
                index = 1;
                foreach (int id in order)
                {
                    Section s = board.Sections.Where(b => b.ID == id).FirstOrDefault();
                    if (s == null)
                    {
                        errMessage = "Could not find section - " + id.ToString();
                        break;
                    }
                    s.Order = index; // set the new index
                    s.ParentID = ParentID;
                    db.Entry(s).State = EntityState.Modified;
                    db.SaveChanges();
                    index++;
                }
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "Boards", new { id = BoardID, errMessage = errMessage, sectionOpen = true });
            return Json(new { Url = redirectUrl, sectionOpen = true });
        }

        // POST: Sections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Section section = db.Sections.Find(id);
            if (section == null)
                return HttpNotFound();
            int BoardID = section.BoardID;
            if (section.Board.OwnerID != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (section.Cards.Count() > 0)
                return RedirectToAction("Index", "Boards", new { id = BoardID, sectionEditOpen = true, errMessage = "Cannot delete section with cards in it.  Please move or delete all cards in the section before deleting." });
            db.Sections.Remove(section);
            db.SaveChanges();
            return RedirectToAction("Index", "Boards", new { id = BoardID, sectionEditOpen = true });
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