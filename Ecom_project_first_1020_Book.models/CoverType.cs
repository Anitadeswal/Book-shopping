using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom_project_first_Book.Models
{
    public class CoverType
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
