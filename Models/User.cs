using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenea_new.Models
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [ForeignKey("Profile")]
        public int? ProfileId { get; set; }

        [ForeignKey("Address")]
        public int? AddressID { get; set; }


    }
}
