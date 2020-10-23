using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.DataAccess
{
    public class DocStatusView
    {
        // Primary key used to refer to the model
        [Key]
        [Required]
        public String PropertyId { get; set; }

        [Required]
        public bool Agreement { get; set; }

        [Required]
        public bool Appraisal { get; set; }

        [Required]
        public bool SiteMap { get; set; }

        [Required]
        public bool Resume { get; set; }

        [Required]
        public bool Paperwork { get; set; }
    }
}
