using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ITWALA.BusinessModel
{
    public class MyMembershipProvider
    {
        private readonly MyDatabaseEntities _myDatabaseEntities = new MyDatabaseEntities();
        public MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
            bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion,
            string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user =_myDatabaseEntities.Users.SingleOrDefault(u => u.Username == username);
            if (user != null)
            {
                if (user.Password == oldPassword)
                {
                    user.Password = newPassword;
                    _myDatabaseEntities.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser(User updatedUser)
        {
            var dbUser = GetUser(updatedUser.Username);
            if (dbUser == null)
            {
                return false;
            }
            else
            {
                var user = _myDatabaseEntities.Users.Single(u => u.Username == updatedUser.Username);
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                _myDatabaseEntities.SaveChanges();
                return true;
            }
        }

        public bool ValidateUser(string username, string password)
        {       
            try
            {
                var user =
                    _myDatabaseEntities.Users.SingleOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public User GetUserByUserid(int userid)
        {
            var user = _myDatabaseEntities.Users.SingleOrDefault(u => u.Userid == userid);
            return user;
        }

        public User GetUser(string username)
        {
            var user = _myDatabaseEntities.Users.SingleOrDefault(u => u.Username == username);
            return user;
        }

        public string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public bool IsNewUser(string username, string email)
        {
            var user = _myDatabaseEntities.Users.FirstOrDefault(u => u.Username == username || u.Email == email);
            if (user != null)
            {
                return false;
            }
            return true;
        }
        public bool CreateNewUser(User user)
        {
            try
            {
                _myDatabaseEntities.Users.Add(user);
                _myDatabaseEntities.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}