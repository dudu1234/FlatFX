using FlatFX.BussinessLayer;
using FlatFX.Model.Core;
using FlatFX.Model.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXUnitTest
{
    public partial class FFXUnitTest
    {
        public UserData m_TestUserData = null;

        public UserData TestUser
        {
            get
            {
                if (m_TestUserData == null)
                {
                    using (var context = new FfxContext())
                    {
                        List<UserData> testUsers = context.Users.Where(u => u.UserName.StartsWith("testFFX_")).ToList();
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

                using (var context = new FfxContext())
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

                    context.Users.Add(user);
                    context.SaveChanges();

                    Logger.Instance.WriteSystemTrace("Test - TestAddUser", Consts.eLogOperationStatus.Succeeded, "Add Test User");
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
                using (var context = new FfxContext())
                {
                    if (TestUser == null)
                        return;

                    TestUser.LastName = "Edited @ " + DateTime.Now.ToShortTimeString();
                    context.SaveChanges();
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
                using (var context = new FfxContext())
                {
                    Dictionary<int, UserData> usersToDelete = context.Users.Where(u => u.UserName.StartsWith("testFFX_")).ToDictionary(u2 => u2.UserId);

                    //Remove all temp contact details
                    Dictionary<int, UserData> detailsIdToDelete = context.Users.Where(u => u.UserName.StartsWith("testFFX_")).ToDictionary(u3 => u3.ContactDetailsId);
                    List<ContactDetails> tempUserDetails = context.ContactsDetails.Where(d => detailsIdToDelete.Keys.Contains(d.ContactDetailsId)).ToList();
                    context.ContactsDetails.RemoveRange(tempUserDetails);

                    //remove all temp users
                    context.Users.RemoveRange(usersToDelete.Values);

                    context.SaveChanges();
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
                using (var context = new FfxContext())
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
                    context.UserActions.Add(action);

                    action = new UserActionData
                    {
                        Text = "@@UnitTest@@ test2 " + DateTime.Now.ToShortTimeString(),
                        Time = DateTime.Now,
                        UserId = TestUser.UserId,
                        Priority = 2,
                        IsSucceded = true,
                        IsRemoved = false
                    };
                    context.UserActions.Add(action);

                    context.SaveChanges();
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
                using (var context = new FfxContext())
                {
                    //Remove all temp contact details
                    List<UserActionData> tempActions = context.UserActions.Where(a => a.Text.StartsWith("@@UnitTest@@ test")).ToList();
                    context.UserActions.RemoveRange(tempActions);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception in TestAddAction: " + ex.ToString());
            }
        }
    }
}
