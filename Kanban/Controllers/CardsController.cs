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
    public class CardsController : Controller
    {
        private KanbanContext db = new KanbanContext();

        // POST: Cards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "SectionID,Title,Content,CardColor")] Card card, string CategoryName = "", string TagName = "")
        {
            Section section = db.Sections.Find(card.SectionID);
            if (section == null)
                return HttpNotFound();
            if (ModelState.IsValid)
            {
                if (section.Board.OwnerID != User.Identity.GetUserId())
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                card.Title = card.Title.Trim();
                card.CardColor = card.CardColor;
                db.Cards.Add(card);
                db.SaveChanges();
                return RedirectToAction("Index", "Boards", new { id = section.BoardID });
            }

            return RedirectToAction("Index", "Boards", new { id = section.BoardID, errMessage = "Could not create card, model not vaild" });
        }

        // POST: Cards/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SectionID,Title,Content,CardColor")] Card card)
        {
            Card origCard = db.Cards.Find(card.ID);
            if (origCard == null)
                return HttpNotFound();
            if (ModelState.IsValid)
            {
                if (origCard.Section.Board.OwnerID != User.Identity.GetUserId())
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                origCard.Title = card.Title.Trim();
                origCard.Content = card.Content;
                origCard.CardColor = card.CardColor;
                db.Entry(origCard).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Boards", new { id = origCard.Section.BoardID });
            }

            return RedirectToAction("Index", "Boards", new { id = origCard.Section.BoardID, errMessage = "Could not create card, model not vaild" });
        }

        // POST: Cards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {

            Card card = db.Cards.Find(id);
            if (card == null)
                return HttpNotFound();
            int BoardID = card.Section.BoardID;
            if (card.Section.Board.OwnerID != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            db.Cards.Remove(card);
            db.SaveChanges();
            return RedirectToAction("Index", "Boards", new { id = BoardID });
        }

        [HttpPost]
        [Authorize]
        public ActionResult Reorder(string serializedData, int BoardID, string Parent)
        {
            string errMessage = "none";
            string success = "true";

            Board myBoard = db.Boards.Find(BoardID);
            if (myBoard == null)
                return HttpNotFound();
            if (myBoard.OwnerID != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            List<Card> myCards = new List<Card>();

            foreach (var s in myBoard.Sections)
            {
                myCards.AddRange(s.Cards.ToList());
            }

            if (!String.IsNullOrEmpty(serializedData))
            {
                string[] cards = serializedData.Split('&');
                int[] order = new int[cards.Length];
                int categoryID = int.Parse(Parent.Split('&')[0]);
                int ParentID = int.Parse(Parent.Split('&')[1]);
                int index = 0;
                foreach (string c in cards)
                {
                    int cardID = int.Parse(c.Split('=')[1]);
                    order[index] = cardID;
                    index++;
                }
                index = 1;
                foreach (int id in order)
                {
                    Card c = myCards.Where(mc => mc.ID == id).FirstOrDefault();
                    if (c == null)
                    {
                        errMessage = "Could not find card - " + id.ToString();
                        success = "false";
                        break;
                    }
                    c.SectionID = ParentID;
                    c.Order = index;
                    db.Entry(c).State = EntityState.Modified;
                    db.SaveChanges();
                    index++;
                }
            }

            var result = new { Success = success, Message = errMessage };
            return Json(result, JsonRequestBehavior.AllowGet);
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