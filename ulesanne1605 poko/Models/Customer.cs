using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ulesanne1605_poko.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(75)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(75)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        [Display(Name = "E-mail")]
        public string EmailId { get; set; }

        [Required]
        [DisplayName("Country")]
        [NotMapped]
        public int CountryId { get; set; }

       // [ForeignKey("CountryId")]
        //public virtual Country Country { get; set; }

        [Required]
        [ForeignKey("City")]
        [DisplayName("City")]
        public int CityId { get; set; }
        public virtual City City { get; set; }

        [MaxLength(500)]
        public string PhotoUrl { get; set; }

        [NotMapped]
        [Display(Name = "Photo")]
        public IFormFile ProfilePhoto { get; set; }

    }
}