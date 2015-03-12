using System;
using System.Linq;
using System.Web.Security;
using DataModels;
using test.Models;

namespace test.Provider
{
    public class CustomRoleProvider: RoleProvider
    {
        public string GetOneRoleForUser(string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    return default(string);
                }
                else
                {
                    var roles = GetRolesForUser(userName);
                    if (roles == null || roles.Length == 0)
                    {
                        return default(string);
                    }
                    else
                    {
                        return roles[0];
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }

            return default(string);
        }

        public override string[] GetRolesForUser(string userName)
        {
            var role = new string[] { };
            try
            {
                using (var db = new TestDBDB())
                {

                    // Получаем пользователя
                    var user = (from u in db.Users
                                where u.Login == userName
                                select u).FirstOrDefault();
                    if (user != null)
                    {
                        // Получаем роль
                        var userRole = db.Roles.Where(r => r.Id == user.RoleId).
                            Select(r => r.Name).ToArray();

                        role = userRole;
                    }

                }
                return role;
            }
            catch(Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
            }
            return role;
        }
        public override void CreateRole(string roleName)
        {
            //var newRole = new  { RoleName = roleName };
            //UsersContext db = new UsersContext();
            //db.webpages_Roles.Add(newRole);
            //db.SaveChanges();
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            var outputResult = false;

            using (var db = new TestDBDB())
            {
                try
                {
                    var user = (from u in db.Users
                                where u.Login == username
                                select u).FirstOrDefault();
                    if (user != null)
                    {

                        var userRole = db.Roles.Find(user.Id);
                        var roles = db.Roles.Find(userRole.Id);

                        if (roles.Name == roleName)
                        {
                            outputResult = true;
                        }
                    }
                }
                catch
                {
                    outputResult = false;
                }
            }
            return outputResult;
        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}