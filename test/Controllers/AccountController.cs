using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using test.Models;

namespace test.Controllers
{
    public class AccountController : Controller
    {
        private BaseLinq baseLinq;

        public AccountController(BaseLinq linqHelper)
        {
            this.baseLinq = linqHelper;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            try
            {
                IHashCode myHashCode = new HashCode();

                if (!ModelState.IsValid) return View(model);

                // Find user.
                var user =
                    baseLinq.ContextDb.Users.FirstOrDefault(u => u.Login == model.Name &&
                                                                 u.Password == myHashCode.GetHashString(model.Password));

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");

                return View(model);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logoff()
        {
            try
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }
       
    }
}