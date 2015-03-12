using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DataModels;
using LinqToDB;
using PagedList;
using test.Models;

namespace test.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private BaseLinq baseLinq;

        public HomeController (BaseLinq linqHelper)
	    {
            this.baseLinq = linqHelper;
	    }

        public ActionResult Edit(int? id)
        {
            try
            {
                ViewBag.Status = baseLinq.StatusList;
                ViewBag.Task = GetFreedowTaskFromCurrentUser();

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                // Nullable int
                var currentTask = baseLinq.ContextDb.Tasks.Find(id.GetValueOrDefault());

                var editTask = FillUserTask(currentTask);

                return View(editTask);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private static CurrentUserTaskModel FillUserTask(Task currentTask)
        {
            try
            {
                var editTask = new CurrentUserTaskModel
                {
                    Task = currentTask.TaskName,
                    Description = currentTask.Description,
                    CreateDate = currentTask.CreateDate,
                    Id = currentTask.Id,
                    Time = currentTask.Time,
                    Status = currentTask.StatusId.GetValueOrDefault()
                };
                return editTask;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(CurrentUserTaskModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var isFreeTime = baseLinq.OverLimitTime(User.Identity.Name, 
                            model.Time.GetValueOrDefault(), model.CreateDate, model.Id);
                    if (isFreeTime)
                    {
                        var result = UpdateTask(model);
                        if (result)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    ModelState.AddModelError("", "Time is max.");
                }

                ViewBag.Status = baseLinq.StatusList;

            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }

            return View();
        }

        public ActionResult Create()
        {
            try
            {
                ViewBag.Status = baseLinq.StatusList;
                ViewBag.Task = GetFreedowTaskFromCurrentUser();

                // Для отображения текущей даты ).
                var emptyUserTask = new CurrentUserTaskModel { CreateDate = DateTime.Now };

                return View(emptyUserTask);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                return null;
            }
        }

        private SelectList GetFreedowTaskFromCurrentUser()
        {
            try
            {
                var userId = baseLinq.GetCurrentUserId(User.Identity.Name);

                // Список id проектов на которых работает пользователь.
                var projectCurrentUser = baseLinq.ContextDb.UsersToProjects.Where(q => q.UserId == userId).
                    Select(q => q.ProjectId).ToList();

                // Формируем список свободных задач для конкретного юзера.
                // Добавить проверку на акнивный не активный.

                var freedomTaskFromCurrentUser = baseLinq.ContextDb.Tasks.Where(t => projectCurrentUser.Contains(
                    t.ProjectId) && t.UserId == null);

                return new SelectList(freedomTaskFromCurrentUser, "Id", "TaskName");

            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CurrentUserTaskModel model)
        {
            try
            {
                ViewBag.Task = baseLinq.TaskList;
                ViewBag.Status = baseLinq.StatusList;

                // Не правильно, потом убрать.
                model.Id = Convert.ToInt32(model.Task);
                model.Task = baseLinq.GetTaskName(model.Id);

                if (ModelState.IsValid)
                {
                    if (model.Task != null)
                    {
                        // Проверять суму времени за день.

                        var isFreeTime = baseLinq.OverLimitTime(User.Identity.Name, 
                            model.Time.GetValueOrDefault(), model.CreateDate);
                        if (isFreeTime)
                        {
                            var result = UpdateTask(model);
                            if (result)
                            {
                                return RedirectToAction("Index");
                            }
                        }
                        ModelState.AddModelError("", "Time is max.");
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                ModelState.AddModelError("", "Error update task.");
            }

            return View();
        }

        private bool UpdateTask(CurrentUserTaskModel model)
        {
            try
            {
                baseLinq.ContextDb.Tasks
                    .Where(t => t.Id == model.Id)
                    .Set(t => t.CreateDate, model.CreateDate)
                    .Set(t => t.Description, model.Description)
                    .Set(t => t.StatusId, model.Status)
                    .Set(t => t.Time, model.Time)
                    .Set(t => t.UserId, baseLinq.GetCurrentUserId(User.Identity.Name))
                    .Set(t => t.TaskName, model.Task)
                    .Update();

                return true;

            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                return false;
            }
        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.TimeSortParm = sortOrder == "time" ? "time_desc" : "time";
            ViewBag.StatusSortParm = sortOrder == "status" ? "status_desc" : "status";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var tasks = from t in baseLinq.ContextDb.Tasks.Where(q => q.UserId == baseLinq.GetCurrentUserId(User.Identity.Name))
                        from s in baseLinq.ContextDb.Status.Where(q => q.Id == t.StatusId).DefaultIfEmpty()
                        from p in baseLinq.ContextDb.Projects.Where(q => q.Id == t.ProjectId).DefaultIfEmpty()
                        orderby t.CreateDate
                        select new CurrentUserTaskModel
                        {
                            Task = t.TaskName,
                            Description = t.Description,
                            CreateDate = t.CreateDate,
                            Id = t.Id,
                            Time = t.Time,
                            Status = s.Id,
                            StatusName = s.Name
                        };

            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => s.Task.Contains(searchString)
                                       || s.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    tasks = tasks.OrderBy(t => t.CreateDate);
                    break;
                case "name_desc":
                    tasks = tasks.OrderBy(t => t.Task);
                    break;
                case "name":
                    tasks = tasks.OrderByDescending(t => t.Task);
                    break;
                case "time_desc":
                    tasks = tasks.OrderBy(t => t.Time);
                    break;
                case "time":
                    tasks = tasks.OrderByDescending(t => t.Time);
                    break;
                case "status_desc":
                    tasks = tasks.OrderBy(t => t.Status);
                    break;
                case "status":
                    tasks = tasks.OrderByDescending(t => t.Status);
                    break;
                default:  // Date descending 
                    tasks = tasks.OrderBy(s => s.Task);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(tasks.ToPagedList(pageNumber, pageSize));
        }
    }
}