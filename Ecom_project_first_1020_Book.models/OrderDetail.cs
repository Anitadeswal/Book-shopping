using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom_project_first_Book.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderHeaderID { get; set; }
        [ForeignKey("OrderHeaderID")]
        public OrderHeader OrderHeader { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }

    }
}
