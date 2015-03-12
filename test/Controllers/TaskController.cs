using System.Linq;
using System.Web.Mvc;
using DataModels;

namespace test.Controllers
{
    public class TaskController : Controller
    {
        private TestDBDB contextDb = new TestDBDB();
        // GET: Task
        public ActionResult Index()
        {
            var task = from t in contextDb.Tasks
                from s in contextDb.Status.Where(q => q.Id == t.StatusId).DefaultIfEmpty()
                from u in contextDb.Users.Where(q => q.Id == t.UserId).DefaultIfEmpty()
                from p in contextDb.Projects.Where(q => q.Id == t.ProjectId).DefaultIfEmpty()
                select new Task
                {
                    TaskName = t.TaskName,
                    Description = t.Description,
                    Project = p,
                    Status = s,
                    UserId = u.Id
                };

            ViewBag.Task = task.ToList();

            return View();
        }
    }
}