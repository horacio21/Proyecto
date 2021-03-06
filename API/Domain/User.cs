﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(40, ErrorMessage = "The maximun length for field {0} us {1} characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(40, ErrorMessage = "The maximun length for field {0} us {1} characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Picture { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(20, ErrorMessage ="The maximun length for field {0} us {1} characters.")]
        [Display(Name = "Username")]
        [Index("User_Username_Index", IsUnique = true)]
        public string Username { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The maximun length for field {0} us {1} characters.")]
        [Display(Name = "Email")]
        [Index("User_Email_Index", IsUnique = true)]
        public string Email { get; set; }
    }
}
