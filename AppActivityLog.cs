using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DFGE_lambda
{
    public class AppActivityLog
    {
        [Key]
        public int id { get; set; }

        [StringLength(255)]
        public string user_id { get; set; }

        [StringLength(255)]
        public int customer_id { get; set; }

        [StringLength(50)]
        public string controller { get; set; }

        [StringLength(50)]
        public string method { get; set; }

        [StringLength(50)]
        public string entry_data_action { get; set; }

        [StringLength(50)]
        public string entry_data_action_message { get; set; }

        [StringLength(50)]
        public string entry_data { get; set; }

        [StringLength(50)]
        public string client_ip { get; set; }

        [StringLength(50)]
        public string client_location { get; set; }

        [StringLength(50)]
        public string country_name { get; set; }

        [StringLength(255)]
        public string client_location_country_flag { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime entry_date { get; set; }
    }
}
