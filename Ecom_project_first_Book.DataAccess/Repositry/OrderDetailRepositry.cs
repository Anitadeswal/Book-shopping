using Ecom_project_first_Book.DataAccess.Data;
using Ecom_project_first_Book.DataAccess.Repositry.IRepositry;
using Ecom_project_first_Book.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom_project_first_Book.DataAccess.Repositry
{
    public class OrderDetailRepositry :Repositry<OrderDetail>,IOrderDetailRepositry
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailRepositry(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}
