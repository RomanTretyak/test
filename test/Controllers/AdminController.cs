using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using DataModels;
using LinqToDB;
using test.Models;

namespace test.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private BaseLinq baseLinq;

        public AdminController(BaseLinq linqHelper)
        {
            this.baseLinq = linqHelper;
        }
        public ActionResult JoinUserToProject()
        {
            ViewBag.User = baseLinq.UserList;
            ViewBag.Project = baseLinq.ProjectList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JoinUserToProject(JoinUsersToProject model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userInProject = baseLinq.ContextDb.UsersToProjects.FirstOrDefault(u =>
                        u.UserId == model.User && u.ProjectId == model.Project);

                    // Если такого нету - создаем запись.
                    if (userInProject == null)
                    {
                        InsertNewUserToProject(model);

                        userInProject = baseLinq.ContextDb.UsersToProjects.FirstOrDefault(u =>
                            u.UserId == model.User && u.ProjectId == model.Project);

                        if (userInProject == null)
                        {
                            ModelState.AddModelError("", "Ошибка записи в базу");
                        }
                        else
                        {
                            return RedirectToAction("JoinUserToProject", "Admin");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Format("Пользователь {0} " +
                                                                   "уже работает на проекте {1}.",
                            baseLinq.GetUserName(model.User),
                            baseLinq.GetProjectName(model.Project)));
                    }
                }

                ViewBag.User = baseLinq.UserList;
                ViewBag.Project = baseLinq.ProjectList;

                return View();
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private void InsertNewUserToProject(JoinUsersToProject model)
        {
            try
            {
                baseLinq.ContextDb.Insert(new UsersToProject
                {
                    ProjectId = model.Project,
                    UserId = model.User
                });
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddProject()
        {
            ViewBag.Status = baseLinq.StatusList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProject(RegisterProjectModel model)
        {
            try
            {
                ViewBag.Status = baseLinq.StatusList;

                if (!ModelState.IsValid) return View();

                var project = baseLinq.ContextDb.Projects.FirstOrDefault(p => p.Code == model.Code &&
                    p.Name == model.Name);

                // Проверка на дубли.
                if (project != null) return View();

                // Create new.
                InsertNewProject(model);

                project = baseLinq.ContextDb.Projects.FirstOrDefault(p => p.Name == model.Name &&
                    p.Code == model.Code);


                ActionResult actionResult;
                return ActionResult(project, "Project", out actionResult) ? actionResult : View();
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private void InsertNewProject(RegisterProjectModel model)
        {
            try
            {
                baseLinq.ContextDb.Insert(new Project
                {
                    Name = model.Name,
                    Code = model.Code,
                    StatusId = model.Status
                });
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }
        }

        public ActionResult AddTask()
        {
            ViewBag.Role = baseLinq.RoleList;
            ViewBag.Status = baseLinq.StatusList;
            ViewBag.Project = baseLinq.ProjectList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTask(RegisterTaskModel model)
        {
            ViewBag.Role = baseLinq.RoleList;
            ViewBag.Status = baseLinq.StatusList;
            ViewBag.Project = baseLinq.ProjectList;

            if (!ModelState.IsValid) return View(model);

            var task = baseLinq.ContextDb.Tasks.FirstOrDefault(t => t.TaskName == model.Name);

            if (task == null)
            {
                InsertNewTask(model);

                task = baseLinq.ContextDb.Tasks.FirstOrDefault(t => t.TaskName == model.Name);

                ActionResult actionResult;
                if (ActionResult(task, "Task", out actionResult)) return actionResult;
            }
            else
            {
                ModelState.AddModelError("", "Задача с таким названием уже существуeт!");
            }

            return View(model);
        }

        private void InsertNewTask(RegisterTaskModel model)
        {
            try
            {
                baseLinq.ContextDb.Insert(new Task
                {
                    TaskName = model.Name,
                    Description = model.Description,
                    StatusId = model.Status,
                    ProjectId = model.Project,
                    CreateDate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }
        }

        private bool ActionResult(object newObject, string controllerName, out ActionResult actionResult)
        {
            try
            {
                actionResult = default(ActionResult);
                if (newObject == null)
                {
                    ModelState.AddModelError("", "Ошибка записи в базу");
                }
                else
                {
                    {
                        actionResult = RedirectToAction("Index", controllerName);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        public ActionResult AddUser()
        {
            ViewBag.Role = new SelectList(baseLinq.ContextDb.Roles, "Id", "Name");
            ViewBag.Status = new SelectList(baseLinq.ContextDb.Status, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(RegisterUserModel model)
        {
            ViewBag.Role = new SelectList(baseLinq.ContextDb.Roles, "Id", "Name");
            ViewBag.Status = new SelectList(baseLinq.ContextDb.Status, "Id", "Name");

            try
            {
                if (ModelState.IsValid)
                {
                    var user = GetUserInBase(model);
                    
                    
                    if (user == null)
                    {
                        InsertNewUser(model);

                        user = GetUserInBase(model);
                        // Пользователь добавлен в бд.

                        if (user == null) return View(model);

                        //FormsAuthentication.SetAuthCookie(model.Login, true);
                        //return RedirectToAction("Index", "Home");
                        return Content("Ok");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private void InsertNewUser(RegisterUserModel model)
        {
            try
            {
                IHashCode myHashCode = new HashCode();
                
                baseLinq.ContextDb.Insert(new User
                {
                    Login = model.Login,
                    Password = myHashCode.GetHashString(model.Password),
                    FIO = model.FIO,
                    RoleId = model.Role,
                    StatusId = model.Status
                });
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }
        }

        

        private User GetUserInBase(RegisterUserModel model)
        {
            try
            {
                IHashCode myHashCode = new HashCode();

                var user =
                     baseLinq.ContextDb.Users.FirstOrDefault(u => u.Login == model.Login&&
                                                         u.Password == myHashCode.GetHashString(model.Password));
                return user;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                return null;
            }
        }
    }
}