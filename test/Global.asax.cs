using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using test.Infrastructure;

namespace test
{

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Заполнение базы тестовыми данными.
            //FillProject fillProject = new FillProject();
            //fillProject.FillDataProject();
            //fillProject.FillDataUser();
            //fillProject.FillUserToProject();
            //fillProject.FillDataTask();

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
        }
    }
}
