using System.ComponentModel.DataAnnotations;

namespace MGR.Login.Application.Models
{
    public class EmailRequest
    {
        [Required]
        [StringLength(30)]
        public string DestinationEmail { get; set; }

        [Required]
        [StringLength(50)]
        public string Subject { get; set; }

        [StringLength(1000)]
        public string Body { get; set; }

        public string Template { get; set; }
    }
}
