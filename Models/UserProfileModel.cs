using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenea_new.Models
{
    public class UserProfileModel
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public string? UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public Address Address { get; set; }
        public string PhoneNumber { get; set; }
        [NotMapped]
        public IFormFile ResumeFile { get; set; }
        public string Resume {  get; set; }
        public string ProfilePhoto { get; set; }

    }

    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
    }
}
