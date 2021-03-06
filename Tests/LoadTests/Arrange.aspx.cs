﻿using System;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Services;

namespace SitefinityWebApp.LoadTests
{

    public partial class Setup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var setup = Request.Params["setup"];
            if (!string.IsNullOrWhiteSpace(setup))
            {
                if (setup == "true")
                    CreateAdminUsers();
                else if (setup == "false")
                    DeleteAdminUsers();
            }
        }

        private static void CreateAdminUsers()
        {
            var userManager = UserManager.GetManager("Default");
            int usersCount = GetUsersCount();

            using (new Telerik.Sitefinity.Data.ElevatedModeRegion(userManager))
            {
                string testUserUsername = string.Format(userNameFormat, usersCount);
                var testUser = userManager.GetUser(testUserUsername);
                if (testUser == null)
                {
                    lock (usersLock)
                    {
                        testUser = userManager.GetUser(testUserUsername);
                        if (testUser == null)
                        {
                            System.Web.Security.MembershipCreateStatus status;
                            RoleManager roleManager = RoleManager.GetManager("AppRoles");
                            for (int i = usersCount; i > 0; --i)
                            {
                                string username = string.Format(userNameFormat, i);
                                if (userManager.GetUser(username) == null)
                                {
                                    var user = userManager.CreateUser(
                                        username,
                                        password,
                                        string.Format("{0}@email.com", username),
                                        string.Format("Question{0}", i),
                                        string.Format("Answer{0}", i), true, null, out status);
                                    userManager.SaveChanges();

                                    using (new Telerik.Sitefinity.Data.ElevatedModeRegion(roleManager))
                                    {
                                        var role = roleManager.GetRole("Administrators");
                                        user = userManager.GetUser(username);
                                        roleManager.AddUserToRole(user, role);
                                        roleManager.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }



        private void DeleteAdminUsers()
        {
            int usersCount = GetUsersCount();
            var userManager = UserManager.GetManager("Default");

            lock (usersLock)
            {
                using (new Telerik.Sitefinity.Data.ElevatedModeRegion(userManager))
                {
                    for (int i = usersCount; i > 0; --i)
                    {
                        string username = string.Format(userNameFormat, i);
                        var user = userManager.GetUser(username);
                        if (user != null)
                        {
                            try
                            {
                                userManager.Delete(user);
                                userManager.SaveChanges();
                            }
                            catch (Telerik.OpenAccess.Exceptions.NoSuchObjectException) { } //someone (probably other thread) has deleted the item 
                        }
                    }
                }
            }
        }

        private static int GetUsersCount()
        {
            int usersCount = 0;
            var usersParameter = SystemManager.CurrentHttpContext.Request[Setup.usersCountQueryStringParamName];
            if (string.IsNullOrWhiteSpace(usersParameter) ||
                !int.TryParse(usersParameter, out usersCount) ||
                usersCount <= 0)
            {
                usersCount = Setup.usersCountDefaultValue;
            }
            return usersCount;
        }

        private const string usersCountQueryStringParamName = "usersCount";
        private const int usersCountDefaultValue = 100;

        private static object usersLock = new object();
        private const string userNameFormat = "user{0}";
        private const string password = "password";
    }
}