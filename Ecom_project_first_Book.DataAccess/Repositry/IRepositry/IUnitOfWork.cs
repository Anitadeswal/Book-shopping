using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom_project_first_Book.DataAccess.Repositry.IRepositry
{
    public interface IUnitOfWork
    {
        ICategoryRepositry Catogery { get; }        //property
        ICoverTypeRepositry CoverType { get; }
        IProductRepositry Product { get; }
        IComapanyRepositry Company { get; }
        IApplicationUserRepositry ApplicationUser { get; }
        ISP_CALL SP_CALL { get; }
        IShoppingCartRepositry ShoppingCart { get; }
        IOrderDetailRepositry OrderDetail { get; }

        IOrderHeaderRepositry OrderHeader { get; }

        void save();
    }
}
