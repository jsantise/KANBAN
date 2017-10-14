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
using Kanban.ViewModels;
using Microsoft.AspNet.Identity;

namespace Kanban.Controllers
{
    public class BoardsController : Controller
    {
        private KanbanContext db = new KanbanContext();

        // GET: Boards
        [Authorize]
        public ActionResult Index(int? id, string errMessage = "",
            bool sectionOpen = false, bool categoryOpen = false, bool tagOpen = false,
            bool sectionEditOpen = false, bool categoryEditOpen = false, bool tagEditOpen = false, bool boardEditOpen = false, bool cardEditOpen = false)
        {
            BoardViewModel view = new BoardViewModel();
            string user = User.Identity.GetUserId();
            if (db.Boards.Where(b => b.OwnerID == user).Count() > 0)
            {
                List<Board> boards = db.Boards.Where(b => b.OwnerID == user).ToList();
                List<MiniBoard> miniBoards = new List<MiniBoard>();
                foreach (var b in boards)
                {
                    MiniBoard m = new MiniBoard(b.ID, b.Title);
                    miniBoards.Add(m);
                }
                view.Boards = miniBoards;

                if (id == null)
                {
                    view.SelectedBoard = boards.FirstOrDefault();
                }
                else
                {
                    Board origBoard = db.Boards.Find(id);
                    if (origBoard == null)
                        return HttpNotFound();
                    Board selectedBoard = boards.Where(b => b.ID == id).FirstOrDefault();
                    if (selectedBoard == null)
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    view.SelectedBoard = selectedBoard;
                }

                SectionRow row = new SectionRow();
                view.SectionTable = row.getTable(view.SelectedBoard.Sections.ToList());
                foreach (var r in view.SectionTable)
                {
                    r.Row = r.Row.OrderBy(rr => rr.Order).ToList();
                }
                view.ActiveSections = row.getActiveSections(view.SelectedBoard.Sections.ToList(), view.SectionTable);
                List<Section> sections = view.SelectedBoard.Sections.OrderBy(s => s.ParentID).ThenBy(s => s.Order).ToList();
                view.SelectedBoard.Sections = sections;
                List<Card> cards = new List<Card>();
                foreach (var item in view.ActiveSections)
                {
                    cards.AddRange(item.Cards);
                    List<Card> orderCards = item.Cards.OrderBy(c => c.Order).ToList();
                    item.Cards = orderCards;
                }
                ViewBag.uncategorizedCards = cards.Count > 0 ? true : false;
            }

            ViewBag.sectionOpen = sectionOpen;
            ViewBag.categoryOpen = categoryOpen;
            ViewBag.tagOpen = tagOpen;
            ViewBag.sectionEditOpen = sectionEditOpen;
            ViewBag.categoryEditOpen = categoryEditOpen;
            ViewBag.tagEditOpen = tagEditOpen;
            ViewBag.boardEditOpen = boardEditOpen;
            ViewBag.cardEditOpen = cardEditOpen;
            ViewBag.errorMessage = errMessage;
            return View(view);
        }

        // POST: Boards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ID,Title")] Board board)
        {
            if (ModelState.IsValid)
            {
                board.OwnerID = User.Identity.GetUserId();
                db.Boards.Add(board);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(board);
        }


        // POST: Boards/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ID,Title")] Board board)
        {
            if (ModelState.IsValid)
            {
                Board origBoard = db.Boards.Find(board.ID);

                if (origBoard == null)
                    return HttpNotFound();
                if (origBoard.OwnerID != User.Identity.GetUserId())
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

                origBoard.Title = board.Title;
                db.Entry(origBoard).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { boardEditOpen = true, id = board.ID });
            }
            return View(board);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Reorder(string serializedData, int BoardID)
        {
            string errMessage = "none";
            string success = "true";
            string user = User.Identity.GetUserId();
            List<Board> myBoards = db.Boards.Where(b => b.OwnerID == user).ToList();

            if (!String.IsNullOrEmpty(serializedData))
            {
                string[] boards = serializedData.Split('&');
                int[] order = new int[boards.Length];
                int index = 0;
                foreach (string b in boards)
                {
                    order[index] = int.Parse(b.Split('=')[1]);
                    index++;
                }
                index = 1;
                foreach (int id in order)
                {
                    Board b = myBoards.Where(mb => mb.ID == id).FirstOrDefault();
                    if (b == null)
                    {
                        errMessage = "Could not find board - " + id.ToString();
                        success = "false";
                        break;
                    }
                    b.Order = index;
                    db.Entry(b).State = EntityState.Modified;
                    db.SaveChanges();
                    index++;
                }
            }

            var result = new { Success = success, Message = errMessage };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: Boards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Board board = db.Boards.Find(id);
            if (board == null)
                return HttpNotFound();
            if (board.OwnerID != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            db.Boards.Remove(board);
            db.SaveChanges();
            return RedirectToAction("Index", new { boardEditOpen = true });
        }

        [HttpGet]
        [Authorize]
        public ActionResult Copy(int BoardID)
        {
            //This does a copy - no cards
            Board board = db.Boards.Find(BoardID);
            if (board == null)
                return HttpNotFound();
            if (board.OwnerID != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            Board clone = db.Boards.AsNoTracking()
                                    .Include(b => b.Sections)
                                    .Where(b => b.ID == BoardID).FirstOrDefault();
            db.Boards.Add(clone);
            db.SaveChanges(); //Need to save changes so I can get the new ID's

            clone.Title = clone.Title + " - Copy";
            foreach (var section in clone.Sections.Where(s => s.ParentID != 0))
            {
                Section match = board.Sections.Where(s => s.Title == section.Title).FirstOrDefault();
                Section parent = board.Sections.Where(s => s.ID == match.ParentID).FirstOrDefault();
                Section newParent = clone.Sections.Where(s => s.Title == parent.Title).FirstOrDefault();
                section.ParentID = newParent.ID;
            }

            db.SaveChanges();

            return RedirectToAction("Index", new { boardEditOpen = true, id = BoardID });
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