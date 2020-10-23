using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.DataAccess
{
    /// <summary>
    /// This class defines the database schema that is used to back the MVC backend.
    /// The class is suitable for any sql compattible backend. 
    /// For demonstration purposes we are using SQLite as the backend for the model.
    /// </summary>
    public class Document
    {
        // Primary key used to refer to the model
        [Key]
        [Required]
        public String Id { get; set; }

        [Required]
        public String PropertyId { get; set; }

        [Required]
        public String DocType { get; set; }

        [Required]
        public String FileName { get; set; }

        [Required]
        public Byte[] DocBlob { get; set; }
    }
}
