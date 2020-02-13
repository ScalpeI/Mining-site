using Domain.Abstract;
using Domain.Entities;
using System.Linq;
using System.Web.Mvc;
using WebUI.Filters;

namespace WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        IUserRepository repository;

        public AdminController(IUserRepository repo)
        {
            repository = repo;
        }

        public ViewResult Index()
        {
            ViewBag.Header = "Admin Panel";
            ViewBag.SubHeader = "User list";
            return View(repository.Users);
        }

        [IndexException]
        public ViewResult Edit(int userId)
        {
            User user = repository.Users.FirstOrDefault(r => r.UserId == userId);
            ViewBag.Header = "Admin Panel";
            ViewBag.SubHeader = "Edit " + user.Login;
            return View(user);
        }
        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                repository.UpdateUser(user);
                TempData["message"] = string.Format("{0} has been saved", user.Login);
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(user);
                
            }
        }
        public ViewResult Create()
        {
            ViewBag.Header = "Admin Panel";
            ViewBag.SubHeader = "Create User";
            return View();
        }
        [HttpPost]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                repository.CreateUser(user);
                TempData["message"] = string.Format("{0} has been created", user.Login);
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(user);
            }
        }
        [IndexException]
        public ActionResult Delete(int userId)
        {
            ViewBag.Header = "Admin Panel";
            
            User user = repository.Users.FirstOrDefault(r => r.UserId == userId);
            ViewBag.SubHeader = "Delete "+user.Login;
            return View(user);
        }
        [HttpPost]
        public ActionResult Delete(User user)
        {
            if (ModelState.IsValid)
            {
                TempData["message"] = "Account has been delete";
                repository.DeleteUser(user);
                return RedirectToAction("Index");
            }
            else
            {
                return View(user);
            }
        }
    }
}