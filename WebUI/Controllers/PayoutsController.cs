using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain.Concrete;
using Domain.Entities;
using Domain.Abstract;
using System.Text;
using System.IO;

namespace WebUI.Controllers
{
    [Authorize]
    public class PayoutsController : Controller
    {
        IPayoutRepository repository;

        public PayoutsController(IPayoutRepository repo)
        {
            repository = repo;
        }

        // GET: Payouts
        public ActionResult Index(string Login)
        {
            ViewBag.Header = "Admin Panel";
            ViewBag.SubHeader = Login + " Payouts List";
            ViewBag.Login = Login;
            return View(repository.Payouts.Where(x=>x.owner== Login));
        }

        // GET: Payouts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payout payout = repository.Payouts.FirstOrDefault(x=>x.id==id);
            if (payout == null)
            {
                return HttpNotFound();
            }
            ViewBag.Header = "Admin Panel";
            ViewBag.SubHeader = "Payouts Details";
            return View(payout);
        }

        // GET: Payouts/Create
        public ActionResult Create(string owner)
        {
            ViewBag.owner = owner;
            ViewBag.Header = "Admin Panel";
            ViewBag.SubHeader = owner + " Payouts Create";
            return View();
        }

        // POST: Payouts/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Payout payout)
        {
            if (ModelState.IsValid)
            {
                repository.Create(payout);
                return RedirectToAction("Index",new { Login = payout.owner});
            }
            else 
            return View(payout);
        }

        // GET: Payouts/Edit/5
        public ActionResult Edit(int? id, string Login)
        {
            ViewBag.Login = Login;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payout payout = repository.Payouts.FirstOrDefault(x=>x.id==id);
            if (payout == null)
            {
                return HttpNotFound();
            }
            ViewBag.Header = "Admin Panel";
            ViewBag.SubHeader = Login+" Payouts Edit";
            return View(payout);
        }

        // POST: Payouts/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Payout payout)
        {
            if (ModelState.IsValid)
            {
                repository.Edit(payout);
                return RedirectToAction("Index", new { Login = payout.owner });
            } else
            return View(payout);
        }

        // GET: Payouts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payout payout = repository.Payouts.FirstOrDefault(x => x.id == id);
            if (payout == null)
            {
                return HttpNotFound();
            }
            ViewBag.Header = "Admin Panel";
            ViewBag.SubHeader = "Payouts Delete";
            return View(payout);
        }

        // POST: Payouts/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Payout payout = repository.Payouts.FirstOrDefault(x => x.id == id);
            repository.Delete(payout);
            return RedirectToAction("Index", new { Login = payout.owner });
        }

        

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
