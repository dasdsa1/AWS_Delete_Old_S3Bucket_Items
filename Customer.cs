using System;
using System.ComponentModel.DataAnnotations;

namespace DFGE_lambda
{
    public class Customer
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "You must enter a valid Company name")]
        [Display(Name = "Company name:")]
        [StringLength(50, ErrorMessage = "The value cannot exceed 50 characters")]
        public string company_name { get; set; }

        [Required(ErrorMessage = "You must enter a valid contact name for this customer")]
        [Display(Name = "Primary contact name:")]
        [StringLength(50, ErrorMessage = "The value cannot exceed 50 characters")]
        public string primaryContact_name { get; set; }

        [Required(ErrorMessage = "You must enter a valid contact email for this customer")]
        [Display(Name = "Primary contact email:")]
        [StringLength(50, ErrorMessage = "The value cannot exceed 50 characters")]
        public string primaryContact_email { get; set; }

        [Display(Name = "Is customer active?")]
        public bool is_customer_active { get; set; }
        public bool isDeleted { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime create_date { get; set; }

        [StringLength(50)]
        public string created_by_user_id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime modified_date { get; set; }

        [StringLength(50)]
        public string modified_by_user_id { get; set; }

        [StringLength(200)]
        public string s3_folderPath_files { get; set; }

        [StringLength(200)]
        public string s3_folderPath_recycleBin { get; set; }
    }
}