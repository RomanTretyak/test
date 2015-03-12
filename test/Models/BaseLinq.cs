using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using DataModels;

namespace test.Models
{
    public interface IBaseLinq
    {
        SelectList ProjectList { get; }
        SelectList UserList { get; }
        SelectList StatusList { get; }
        SelectList RoleList { get; }
        SelectList TaskList { get; }
        int GetCurrentUserId(string userName);
        string GetTaskName(int id);
        string GetUserName(int userId);
        string GetProjectName(int projectId);
        bool OverLimitTime(string userName, int newTime, DateTime? date, int taskId = 0);

    }
    public class BaseLinq : IBaseLinq
    {
        public TestDBDB ContextDb = new TestDBDB();

        public SelectList ProjectList
        {
            get { return new SelectList(ContextDb.Projects, "Id", "Name"); }
        }

        public SelectList UserList
        {
            get { return new SelectList(ContextDb.Users, "Id", "Login"); }
        }
        public SelectList StatusList
        {
            get { return new SelectList(ContextDb.Status, "Id", "Name"); }
        }
        public SelectList RoleList
        {
            get { return new SelectList(ContextDb.Roles, "Id", "Name"); }
        }
        public SelectList TaskList
        {
            get { return new SelectList(ContextDb.Tasks, "Id", "TaskName"); }
        }

        public int GetCurrentUserId(string userName)
        {
            try
            {
                var userId = (from u in ContextDb.Users
                              where u.Login == userName
                              select u.Id).FirstOrDefault();

                return Convert.ToInt32(userId);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                return default(int);
            }
        }
        public string GetTaskName(int id)
        {
            try
            {
                var taskName = (from t in ContextDb.Tasks
                                   where t.Id == id
                                   select t.TaskName).FirstOrDefault();
                return taskName;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        public string GetUserName(int userId)
        {
            var result = default(string);
            try
            {
                var user = ContextDb.Users.FirstOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    result = user.Login;
                }
                return result;

            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }
            return result;
        }

        public string GetProjectName(int projectId)
        {
            var result = default(string);

            try
            {
                var project = ContextDb.Projects.FirstOrDefault(p => p.Id == projectId);

                if (project != null)
                {
                    result = project.Name;
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }
            return result;
        }

        public bool OverLimitTime(string userName, int newTime, DateTime? date, int taskId = 0)
        {
            try
            {
                var sumTimeOnDay = (from t in ContextDb.Tasks
                                    where t.UserId == GetCurrentUserId(userName)
                                    where t.CreateDate == date
                                    where t.Id != taskId
                                    select new { t.UserId, t.Time, t.Id }).ToList();
                var sum = sumTimeOnDay.Select(t => t.Time).Sum();
                
                var maxValue = Convert.ToInt32(ConfigurationManager.AppSettings["maxTimeValue"]);

                if ((sum + newTime) > maxValue)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }
            return false;
        }
        
    }
}
