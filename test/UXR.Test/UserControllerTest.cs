using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UXR;
using UXR.Models.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using UXR.Controllers;
using System.Web.Mvc;
using UXR.Models;
using Microsoft.AspNet.Identity;
using UXR.ViewModels;
using System.Web;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Security.Principal;

namespace UXR.Test
{
    [TestClass]
    public class UserControllerTest
    {
        private const string APPROVED_ROLE_NAME = "Approved";
        private const string ADMIN_ROLE_NAME = "Admin";
        private const string SUPERADMIN_ROLE_NAME = "Superadmin";

        private List<IdentityRole> _roles = new List<IdentityRole>()
            {
                new IdentityRole(APPROVED_ROLE_NAME),
                new IdentityRole(ADMIN_ROLE_NAME),
                new IdentityRole(SUPERADMIN_ROLE_NAME)
            };

        private const string NEW_USER1_NAME = "newuser1@email.com";
        private ApplicationUser _newUser1 = new ApplicationUser()
        {
            UserName = NEW_USER1_NAME,
            Email = NEW_USER1_NAME,
            Id = NEW_USER1_NAME
        };

        private const string NEW_USER2_NAME = "newuser1@email.com";
        private ApplicationUser _newUser2 = new ApplicationUser()
        {
            UserName = NEW_USER2_NAME,
            Email = NEW_USER2_NAME
        };

        private const string NEW_USER3_NAME = "newuser1@email.com";
        private ApplicationUser _newUser3 = new ApplicationUser()
        {
            UserName = NEW_USER3_NAME,
            Email = NEW_USER3_NAME
        };

        private const string APPROVED_USER_NAME = "approveduser@email.com";
        private ApplicationUser _approvedUser = new ApplicationUser()
        {
            UserName = APPROVED_USER_NAME,
            Email = APPROVED_USER_NAME,
            Id = APPROVED_USER_NAME
        };

        private const string ADMIN_USER_NAME = "adminuser@email.com";
        private ApplicationUser _adminUser = new ApplicationUser()
        {
            UserName = ADMIN_USER_NAME,
            Email = ADMIN_USER_NAME,
            Id = ADMIN_USER_NAME
        };

        private const string SUPERADMIN_USER_NAME = "superadminuser@email.com";
        private ApplicationUser _superadminUser = new ApplicationUser()
        {
            UserName = SUPERADMIN_USER_NAME,
            Email = SUPERADMIN_USER_NAME,
            Id = SUPERADMIN_USER_NAME
        };

        [TestInitialize]
        public void Initialize()
        {
            _approvedUser.Roles.Add(new IdentityUserRole() { RoleId = _roles[0].Id, UserId = _approvedUser.Id});

            _adminUser.Roles.Add(new IdentityUserRole() { RoleId = _roles[0].Id, UserId = _adminUser.Id });
            _adminUser.Roles.Add(new IdentityUserRole() { RoleId = _roles[1].Id, UserId = _adminUser.Id });

            _superadminUser.Roles.Add(new IdentityUserRole() { RoleId = _roles[0].Id, UserId = _superadminUser.Id });
            _superadminUser.Roles.Add(new IdentityUserRole() { RoleId = _roles[1].Id, UserId = _superadminUser.Id });
            _superadminUser.Roles.Add(new IdentityUserRole() { RoleId = _roles[2].Id, UserId = _superadminUser.Id });
        }

        private UserController PrepareController(List<ApplicationUser> users, ApplicationUserManager userManager)
        {
            var dbContext = new Mock<IIdentityDbContext<ApplicationUser>>();

            var rolesSet = Testing.Mocks.GetQueryableMockedDbSet<IdentityRole>(_roles);
            var usersSet = Testing.Mocks.GetQueryableMockedDbSet<ApplicationUser>(
                users);
            dbContext.Setup(m => m.Roles).Returns(() => rolesSet);
            dbContext.Setup(m => m.Users).Returns(() => usersSet);

            var database = new IdentityDatabase(dbContext.Object);

            return new UserController(database, userManager);
        }

        private ApplicationUserManager BaseUserManager()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            return new ApplicationUserManager(userStore.Object);
        }


        [TestMethod]
        public void Test_Index_ManageUsersNotNull()
        {
            var controller = PrepareController(new List<ApplicationUser>(), BaseUserManager());
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test_Index_NoUsersExistEmptyList()
        {
            var controller = PrepareController(new List<ApplicationUser>(), BaseUserManager());
            var result = controller.Index() as ViewResult;
            ManageUsersListViewModel manageUsersListViewModel = result.Model as ManageUsersListViewModel;

            Assert.IsTrue(manageUsersListViewModel.Users.Count == 0);
        }

        [TestMethod]
        public void Test_Index_ReturnsNewUser()
        {
            var controller = PrepareController(new List<ApplicationUser>() { _newUser1 },
                BaseUserManager());
            var result = controller.Index() as ViewResult;
            ManageUsersListViewModel manageUsersListViewModel = result.Model as ManageUsersListViewModel;

            Assert.IsTrue(manageUsersListViewModel.Users.Count == 1 
                && manageUsersListViewModel.Users[0].Username == NEW_USER1_NAME);
        }

        [TestMethod]
        public void Test_Index_ReturnsApprovedUser()
        {
            var controller = PrepareController(new List<ApplicationUser>() { _approvedUser },
                BaseUserManager());
            var result = controller.Index() as ViewResult;
            ManageUsersListViewModel manageUsersListViewModel = result.Model as ManageUsersListViewModel;

            Assert.IsTrue(manageUsersListViewModel.Users.Count == 1
                && manageUsersListViewModel.Users[0].Username == APPROVED_USER_NAME
                && manageUsersListViewModel.Users[0].Approved);
        }

        [TestMethod]
        public void Test_Index_ReturnsAdminUser()
        {
            var controller = PrepareController(new List<ApplicationUser>() { _adminUser },
                BaseUserManager());
            var result = controller.Index() as ViewResult;
            ManageUsersListViewModel manageUsersListViewModel = result.Model as ManageUsersListViewModel;

            Assert.IsTrue(manageUsersListViewModel.Users.Count == 1
                && manageUsersListViewModel.Users[0].Username == ADMIN_USER_NAME
                && manageUsersListViewModel.Users[0].Approved
                && manageUsersListViewModel.Users[0].IsAdmin);
        }

        [TestMethod]
        public void Test_Index_ReturnsMultipleNewUsers()
        {
            var controller = PrepareController(new List<ApplicationUser>() { _newUser1, _newUser2, _newUser3 },
                BaseUserManager());
            var result = controller.Index() as ViewResult;
            ManageUsersListViewModel manageUsersListViewModel = result.Model as ManageUsersListViewModel;

            Assert.IsTrue(manageUsersListViewModel.Users.Count == 3
                && manageUsersListViewModel.Users[0].Username == NEW_USER1_NAME
                && manageUsersListViewModel.Users[1].Username == NEW_USER2_NAME
                && manageUsersListViewModel.Users[2].Username == NEW_USER3_NAME);
        }

        [TestMethod]
        public void Test_Index_ReturnsMultipleDifferentUsers()
        {
            var controller = PrepareController(new List<ApplicationUser>() { _newUser1, _newUser2,
                _newUser3, _approvedUser, _adminUser, _superadminUser },
                BaseUserManager());
            var result = controller.Index() as ViewResult;
            ManageUsersListViewModel manageUsersListViewModel = result.Model as ManageUsersListViewModel;

            Assert.IsTrue(manageUsersListViewModel.Users.Count == 6
                && manageUsersListViewModel.Users[0].Username == NEW_USER1_NAME
                && manageUsersListViewModel.Users[1].Username == NEW_USER2_NAME
                && manageUsersListViewModel.Users[2].Username == NEW_USER3_NAME
                && manageUsersListViewModel.Users[3].Username == APPROVED_USER_NAME
                && manageUsersListViewModel.Users[3].Approved
                && manageUsersListViewModel.Users[4].Username == ADMIN_USER_NAME
                && manageUsersListViewModel.Users[4].Approved
                && manageUsersListViewModel.Users[4].IsAdmin
                && manageUsersListViewModel.Users[5].Username == SUPERADMIN_USER_NAME
                && manageUsersListViewModel.Users[5].Approved
                && manageUsersListViewModel.Users[5].IsAdmin);
        }

        private const string APP_URL = @"http://www.site.com";
        private const string FAKE_APP_URL = @"http://www.fakesite.com";

        private void SetControllerContext(UserController controller, bool validReferrer, string[] roles)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.Url).Returns(new Uri(APP_URL));
            if (validReferrer)
            {
                request.Setup(r => r.UrlReferrer).Returns(new Uri(APP_URL));
            }
            else
            {
                request.Setup(r => r.UrlReferrer).Returns(new Uri(FAKE_APP_URL));
            }
            context.Setup(c => c.Request).Returns(request.Object);

            var dummyIdentity = new GenericIdentity("DummyUser");
            var principal = new GenericPrincipal(dummyIdentity, roles);

            context.Setup(t => t.User).Returns(principal);

            controller.ControllerContext = new ControllerContext(context.Object,
                new RouteData(), controller);
        }

        [TestMethod]
        public void Test_Index_UpdateUsersInvalidReferrer()
        {
            var controller = PrepareController(new List<ApplicationUser>(),
                BaseUserManager());

            SetControllerContext(controller, false, new string[] { ADMIN_ROLE_NAME});


            var usersViewModel = new ManageUsersListViewModel()
            {
                Users = new List<ManageUserViewModel>()
            };

            var result = controller.Index(usersViewModel);


            Assert.IsTrue(result.Exception.InnerExceptions.Count == 1 && result.Exception.InnerException is UnauthorizedAccessException);
        }


        [TestMethod]
        public void Test_Index_ApprovesUser()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            userManager.Setup(um => um.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = PrepareController(new List<ApplicationUser>() { _newUser1 },
                userManager.Object);

            SetControllerContext(controller, true, null);


            var usersViewModel = new ManageUsersListViewModel()
            {
                Users = new List<ManageUserViewModel>()
                {
                    new ManageUserViewModel()
                    {
                        Id = _newUser1.Id,
                        Username = _newUser1.UserName,
                        Approved = true,
                        IsAdmin = false
                    }
                }
            };

            var result = controller.Index(usersViewModel);

            userManager.Verify(um => um.AddToRoleAsync(It.Is<string>(userId => userId == _newUser1.Id), 
                It.Is<string>(role => role == APPROVED_ROLE_NAME)), Times.Once);
        }

        [TestMethod]
        public void Test_Index_PromotesToAdmin()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            userManager.Setup(um => um.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = PrepareController(new List<ApplicationUser>() { _approvedUser },
                userManager.Object);

            SetControllerContext(controller, true, new string[] { SUPERADMIN_ROLE_NAME });


            var usersViewModel = new ManageUsersListViewModel()
            {
                Users = new List<ManageUserViewModel>()
                {
                    new ManageUserViewModel()
                    {
                        Id = _approvedUser.Id,
                        Username = _approvedUser.UserName,
                        Approved = true,
                        IsAdmin = true
                    }
                }
            };

            var result = controller.Index(usersViewModel);

            userManager.Verify(um => um.AddToRoleAsync(It.Is<string>(userId => userId == _approvedUser.Id),
                It.Is<string>(role => role == ADMIN_ROLE_NAME)), Times.Once);
        }

        [TestMethod]
        public void Test_Index_DemotesFromAdmin()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            userManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = PrepareController(new List<ApplicationUser>() { _adminUser },
                userManager.Object);

            SetControllerContext(controller, true, new string[] { SUPERADMIN_ROLE_NAME });


            var usersViewModel = new ManageUsersListViewModel()
            {
                Users = new List<ManageUserViewModel>()
                {
                    new ManageUserViewModel()
                    {
                        Id = _adminUser.Id,
                        Username = _adminUser.UserName,
                        Approved = true,
                        IsAdmin = false
                    }
                }
            };

            var result = controller.Index(usersViewModel);

            userManager.Verify(um => um.RemoveFromRoleAsync(It.Is<string>(userId => userId == _adminUser.Id),
                It.Is<string>(role => role == ADMIN_ROLE_NAME)), Times.Once);
        }

        [TestMethod]
        public void Test_Index_WontDemoteSuperadmin()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            userManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = PrepareController(new List<ApplicationUser>() { _superadminUser },
                userManager.Object);

            SetControllerContext(controller, true, new string[] { SUPERADMIN_ROLE_NAME });


            var usersViewModel = new ManageUsersListViewModel()
            {
                Users = new List<ManageUserViewModel>()
                {
                    new ManageUserViewModel()
                    {
                        Id = _superadminUser.Id,
                        Username = _superadminUser.UserName,
                        Approved = true,
                        IsAdmin = false
                    }
                }
            };

            var result = controller.Index(usersViewModel);

            userManager.Verify(um => um.RemoveFromRoleAsync(It.Is<string>(userId => userId == _superadminUser.Id),
                It.Is<string>(role => role == ADMIN_ROLE_NAME)), Times.Never);
        }
    }
}
