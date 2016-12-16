using System;
using System.ComponentModel.DataAnnotations;

namespace JB007.Models.Models
{
    public class RegistrationModel
    {
        #region Properties
        public Int64 Id { get; set; }
        public Guid TenantId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PhoneNumber { get; set; }  
        public string TimeZone { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Mail Id required.")]
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$",ErrorMessage = "Mail Id not valid.")]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "Password required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string RetypePassword { get; set; }
        #endregion
    }
}