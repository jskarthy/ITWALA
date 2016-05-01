using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using ITWALA.Models;
using AutoMapper;
namespace ITWALA.Mappers
{
    //public partial class User
    //{
    //    public string Username { get; set; }
    //    public string Password { get; set; }

    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Email { get; set; }
    //}
    public class UserMapper
    {
        public static User MapToUserType(RegisterViewModel model)
        {
            Mapper.CreateMap<RegisterViewModel, User>();
            return Mapper.Map<User>(model);
            //User user = new User();
            //user.Username = model.Username;
            //user.Password = model.Password;
            //user.FirstName = "";
            //user.LastName = "";
            //user.Email = model.Email;
            //return user;
        }
    }
}