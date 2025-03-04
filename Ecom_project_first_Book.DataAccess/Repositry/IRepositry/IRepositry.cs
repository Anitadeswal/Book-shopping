using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecom_project_first_Book.DataAccess.Repositry.IRepositry
{
    public interface IRepositry<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void Remove(int ID);
        void RemoveRange(IEnumerable<T> entities);
        T Get(int ID);
        IEnumerable<T> GetAll(
            Expression<Func<T,bool>>filter=null,
            Func<IQueryable<T>,IOrderedQueryable<T>> orderBy=null,
            string includeProperties=null  //Category  //CoverType

            );
        T FirstOrDefault(
            Expression<Func<T,bool>> filter=null,
            string includeProperties=null

            );

    }
}
