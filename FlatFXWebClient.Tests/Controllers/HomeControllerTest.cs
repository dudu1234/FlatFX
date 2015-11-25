using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlatFXWebClient;
using FlatFXWebClient.Controllers;

namespace FlatFXWebClient.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}


/*

    public partial class FFXUnitTest
    {
        public UserData m_TestUserData = null;

        public UserData TestUser
        {
            get
            {
                if (m_TestUserData == null)
                {
                    using (var db = new ApplicationDBContext())
                    {
                        List<UserData> testUsers = db.Users.Where(u => u.UserName.StartsWith("testFFX_")).ToList();
                        if (testUsers.Count > 0)
                        {
                            m_TestUserData = testUsers[0];
                        }
                    }
                }

                return m_TestUserData;
            }
        }
        [TestMethod]
        public void TestAddUser()
        {
            try
            {
                UserData user = null;

                using (var db = new ApplicationDBContext())
                {
                    user = new UserData
                    {
                        UserName = "testFFX_" + DateTime.Now.ToShortDateString() + DateTime.Now.ToLongTimeString(),
                        FirstName = "test",
                        LastName = DateTime.Now.ToShortDateString() + DateTime.Now.ToLongTimeString(),
                        Password = "123",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        Status = Consts.eUserStatus.Active,
                        Role = "QA",
                        ContactDetails = new ContactDetails
                        {
                            Email = "info@flatfx.com",
                            Address = "Salzburg",
                            MobilePhone = "0548-222222",
                            WebSite = "www.FlatFX.com"
                        },
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    Logger.Instance.WriteSystemTrace("Test - TestAddUser", Consts.eLogOperationStatus.Succeeded, "Add Test user");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteSystemTrace("Run Test: TestAddUser", Consts.eLogOperationStatus.Failed, "");
                Assert.Fail("Exception in TestAddUser: " + ex.ToString());
                //Assert.IsNotNull(ex, "Exception in TestAddUser: " + ex.ToString());
            }
        }
        [TestMethod]
        public void TestEditUser()
        {
            try
            {
                using (var db = new ApplicationDBContext())
                {
                    if (TestUser == null)
                        return;

                    TestUser.LastName = "Edited @ " + DateTime.Now.ToShortTimeString();
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception in TestEditUser: " + ex.ToString());
            }
        }
        [TestMethod]
        public void TestDeleteUser()
        {
            try
            {
                using (var db = new ApplicationDBContext())
                {
                    Dictionary<int, UserData> usersToDelete = db.Users.Where(u => u.UserName.StartsWith("testFFX_")).ToDictionary(u2 => u2.UserId);

                    //Remove all temp contact details
                    Dictionary<int, UserData> detailsIdToDelete = db.Users.Where(u => u.UserName.StartsWith("testFFX_")).ToDictionary(u3 => u3.ContactDetailsId);
                    List<ContactDetails> tempUserDetails = db.ContactsDetails.Where(d => detailsIdToDelete.Keys.Contains(d.ContactDetailsId)).ToList();
                    db.ContactsDetails.RemoveRange(tempUserDetails);

                    //remove all temp users
                    db.Users.RemoveRange(usersToDelete.Values);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception in TestDeleteUser: " + ex.ToString());
            }
        }
        [TestMethod]
        public void TestAddActions()
        {
            try
            {
                using (var db = new ApplicationDBContext())
                {
                    if (TestUser == null)
                        return;

                    UserActionData action = new UserActionData
                    {
                        Text = "@@UnitTest@@ test1 " + DateTime.Now.ToShortTimeString(),
                        Time = DateTime.Now,
                        UserId = TestUser.UserId,
                        Priority = 2,
                        IsSucceded = true,
                        IsRemoved = false
                    };
                    db.UserActions.Add(action);

                    action = new UserActionData
                    {
                        Text = "@@UnitTest@@ test2 " + DateTime.Now.ToShortTimeString(),
                        Time = DateTime.Now,
                        UserId = TestUser.UserId,
                        Priority = 2,
                        IsSucceded = true,
                        IsRemoved = false
                    };
                    db.UserActions.Add(action);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception in TestAddAction: " + ex.ToString());
            }
        }
        [TestMethod]
        public void TestDeleteActions()
        {
            try
            {
                using (var db = new ApplicationDBContext())
                {
                    //Remove all temp contact details
                    List<UserActionData> tempActions = db.UserActions.Where(a => a.Text.StartsWith("@@UnitTest@@ test")).ToList();
                    db.UserActions.RemoveRange(tempActions);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception in TestAddAction: " + ex.ToString());
            }
        }
    }
*/