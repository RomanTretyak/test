using System;
using System.Linq;
using System.Web.Mvc;
using DataModels;
using test.Models;

namespace test.Controllers
{
    public class ProjectController : Controller
    {
        private BaseLinq baseLinq;
        public ProjectController(BaseLinq linqHelper)
        {
            this.baseLinq = linqHelper;
        }

        // GET: Project
        public ActionResult Index()
        {
            try
            {
                var projects = from p in baseLinq.ContextDb.Projects
                               from s in baseLinq.ContextDb.Status.Where(q => q.Id == p.StatusId).DefaultIfEmpty()
                               orderby p.Name
                               select new Project
                               {
                                   Name = p.Name,
                                   Code = p.Code,
                                   Status = s
                               };

                ViewBag.Project = projects.ToList();
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }

            return View();
        }
    }
}