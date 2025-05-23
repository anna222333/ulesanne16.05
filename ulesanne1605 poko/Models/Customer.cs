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

    [Required]
    [ForeignKey("City")]
    [DisplayName("City")]
    public int CityId { get; set; }
    public virtual City City { get; set; }

    [Required(ErrorMessage = "Please choose the Customer Photo")]
    [MaxLength(500)]
   
    public string PhotoUrl { get; set; }

    [NotMapped]
    [Display(Name = "Photo")]
    public IFormFile ProfilePhoto { get; set; }

}