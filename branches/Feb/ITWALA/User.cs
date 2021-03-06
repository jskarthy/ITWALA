//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace ITWALA
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public int Userid { get; set; }
        
        public string Username { get; set; }
        public string Password { get; set; }
        [Display(Name = "First Name")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Enter First Name in Alphanumeric Format")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Enter Last Name in Alphanumeric Format")]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
