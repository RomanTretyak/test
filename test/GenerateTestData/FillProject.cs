using System;
using System.Linq;
using DataModels;
using LinqToDB;
using test.Models;

namespace test.GenerateTestData
{
    public class FillProject
    {
        BaseLinq baseLinq = new BaseLinq();
        Random rnd = new Random();
        public bool FillDataProject()
        {
            try
            {
                for (int i = 1; i < 1001; i++)
                {
                    string name = "Проект_" + i;
                    string code = i.ToString();
                    baseLinq.ContextDb.Insert(new Project
                    {
                        Name = name,
                        Code = code,
                        StatusId =  rnd.Next(3,4)
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString()); 
            }
            return false;
        }

        //public bool FillDataUser()
        //{
        //    try
        //    {
        //        for (int i = 1; i < 10001; i++)
        //        {
        //            string login = "Test_" + i.ToString();
        //            string pass = "TestDBLogin!1";
        //            string fio = "Name_" + i.ToString();

        //            baseLinq.ContextDb.Insert(new User
        //            {
        //                Login = login,
        //                Password = pass,
        //                FIO = fio,
        //                RoleId = rnd.Next(1,2),
        //                StatusId = rnd.Next(3,4)
        //            });
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.GetLogger().LogError(ex.ToString()); 
        //    }
        //    return false;
        //}

        public bool FillUserToProject()
        {
            try
            {
                int userMaxId = baseLinq.ContextDb.Users.Max(x => x.Id);
                int userMinId = baseLinq.ContextDb.Users.Min(x => x.Id);

                int projectMaxId = baseLinq.ContextDb.Projects.Max(x => x.Id);
                int projectMinId = baseLinq.ContextDb.Projects.Min(x => x.Id);

                for (int i = 1; i < 10001; i++)
                {
                    baseLinq.ContextDb.Insert(new UsersToProject
                    {
                        ProjectId = rnd.Next(projectMinId, projectMaxId),
                        UserId = rnd.Next(userMinId, userMaxId)
                    });
                }
               
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString()); 
            }
            return false;
        }
        DateTime RandomDay()
        {
            DateTime start = new DateTime(2015, 1, 1);
            Random gen = new Random();

            int range = (DateTime.Now - start).Days;
            return start.AddDays(gen.Next(range));
        }
        public bool FillDataTask()
        {
            try
            {
                int projectMaxId = baseLinq.ContextDb.Projects.Max(x => x.Id);
                int projectMinId = baseLinq.ContextDb.Projects.Min(x => x.Id);

                int userMaxId = baseLinq.ContextDb.Users.Max(x => x.Id);
                int userMinId = baseLinq.ContextDb.Users.Min(x => x.Id);

                for (int i = 1; i < 1000001; i++)
                {
                    string taskName = "Задача_" + i;
                    string descriptionTask = "Описание задачи_" + i;
                    int userId = (rnd.Next(userMinId, userMaxId));
                    int time = (rnd.Next(5, 30));
                    DateTime createDate = RandomDay();

                    var isFreeTime = baseLinq.OverLimitTime(baseLinq.GetUserName(userId), time, createDate);
                    if (isFreeTime)
                    {
                        baseLinq.ContextDb.Insert(new Task
                        {
                            TaskName = taskName,
                            Description = descriptionTask,
                            StatusId = rnd.Next(3, 4),
                            ProjectId = rnd.Next(projectMinId, projectMaxId),
                            CreateDate = createDate,
                            Time = time,
                            UserId = userId
                        });
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString()); 
            }
            return false;
        }
    }
    
}