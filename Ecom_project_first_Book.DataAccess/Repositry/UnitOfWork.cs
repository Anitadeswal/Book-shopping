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
    

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Catogery = new CategoryRepositry(_context);
            CoverType = new CoverTypeRepositry(_context);
            Product = new ProductRepositry(_context);
            Company = new CompanyRepositry(_context);
            ApplicationUser = new ApplicationUserRepositry(_context);
            SP_CALL = new SP_CALL(_context);
            ShoppingCart = new ShoppingCartRepositry(_context);
            OrderDetail= new OrderDetailRepositry(_context);
            OrderHeader= new OrderHeaderRepositry(_context);
        }

        public ICategoryRepositry Catogery {private set; get; }

        public ICoverTypeRepositry CoverType { private set; get; }

        public IProductRepositry Product { private set; get; }
        public IComapanyRepositry Company { private set; get; }
        public IApplicationUserRepositry ApplicationUser { private set; get; }

        public ISP_CALL SP_CALL { private set; get; }

        public IShoppingCartRepositry ShoppingCart { private set; get; }


        public IOrderDetailRepositry OrderDetail { private set; get; }


        public IOrderHeaderRepositry OrderHeader { private set; get; }


        public void save()
        {
            _context.SaveChanges();
        }
    }
}
