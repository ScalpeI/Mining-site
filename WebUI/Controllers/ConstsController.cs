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

namespace WebUI.Controllers
{
    [Authorize]
    public class ConstsController : Controller
    {
        IConstRepository repository;
        public ConstsController(IConstRepository repo)
        {
            repository = repo;
        }
        public ViewResult Index()
        {
            return View(repository.Consts.ToList());
        }
        // GET: Consts/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Header = "Admin Panel";
            ViewBag.SubHeader = "Edit Const";
            Const consts = repository.Consts.FirstOrDefault(r => r.id == id);
            return View(consts);
        }

        // POST: Consts/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Const @const)
        {
            if (ModelState.IsValid)
            {
                repository.Update(@const);
                return RedirectToAction("Index", "Admin");
            }
            return View(@const);
        }
    }
}
