using Domain.Abstract;
using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUI.Controllers;

namespace UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Rates()
        {
            //Организация (arrange)
            Mock<IUserRepository> mock = new Mock<IUserRepository>();
            mock.Setup(m => m.Users).Returns(new List<User>
            {
                new User{UserId=1,Login="Login1"},
                new User{UserId=2,Login="Login2"},
                new User{UserId=3,Login="Login3"},
                new User{UserId=4,Login="Login4"},
                new User{UserId=5,Login="Login5"},
                new User{UserId=6,Login="Login6"}
            });

            AdminController controller = new AdminController(mock.Object);

            //Действие (act)
            List<User> result = ((IEnumerable<User>)controller.Index().ViewData.Model).ToList();

            //Утверждение (assert)
            Assert.AreEqual(result.Count(), 6);
            Assert.AreEqual(result[0].Login, "Login1");
            Assert.AreEqual(result[1].Login, "Login2");
        }

        [TestMethod]
        public void Can_Edit_Rate()
        {
            //Организация (arrange)
            Mock<IUserRepository> mock = new Mock<IUserRepository>();
            mock.Setup(m => m.Users).Returns(new List<User>
            {
                new User{UserId=1,Login="Login1"},
                new User{UserId=2,Login="Login2"},
                new User{UserId=3,Login="Login3"},
                new User{UserId=4,Login="Login4"},
                new User{UserId=5,Login="Login5"},
                new User{UserId=6,Login="Login6"}
            });

            AdminController controller = new AdminController(mock.Object);

            //Действие (act)
            User user1 = controller.Edit(1).ViewData.Model as User;
            User user2 = controller.Edit(2).ViewData.Model as User;
            User user3 = controller.Edit(3).ViewData.Model as User;

            //Утверждение (assert)
            Assert.AreEqual(1, user1.UserId);
            Assert.AreEqual(2, user2.UserId);
            Assert.AreEqual(3, user3.UserId);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Rate()
        {
            //Организация (arrange)
            Mock<IUserRepository> mock = new Mock<IUserRepository>();
            mock.Setup(m => m.Users).Returns(new List<User>
            {
                new User{UserId=1,Login="Login1"},
                new User{UserId=2,Login="Login2"},
                new User{UserId=3,Login="Login3"},
                new User{UserId=4,Login="Login4"},
                new User{UserId=5,Login="Login5"},
                new User{UserId=6,Login="Login6"}
            });

            AdminController controller = new AdminController(mock.Object);

            //Действие (act)
            User result = controller.Edit(7).ViewData.Model as User;


            //Утверждение (assert)
            Assert.IsNull(result);
        }
    }
}
