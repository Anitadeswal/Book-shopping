using Ecom_project_first_Book.DataAccess.Data;
using Ecom_project_first_Book.DataAccess.Repositry.IRepositry;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecom_project_first_Book.DataAccess.Repositry
{
    public class Repositry<T> : IRepositry<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbset;
        public Repositry(ApplicationDbContext context)
        {
           _context= context;
            dbset = _context.Set<T>();
        }

        public void Add(T entity)
        {
           dbset.Add(entity);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> filter = null, 
            string includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if (filter != null)
            {
                query= query.Where(filter);

            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new[] { ','},
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(includeProp);

                }
            }
            return query.FirstOrDefault();
            
        }

        public T Get(int ID)
        {
            return dbset.Find(ID);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if (filter != null)
            
                query = query.Where(filter);
            
            if (includeProperties != null)
            
                foreach (var includeProp in includeProperties.Split(new[] { ','},
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query= query.Include(includeProp);

                }
            
            if (orderBy != null)
            
                return orderBy(query).ToList();
               
            
            return query.ToList();
            
        }

        public void Remove(T entity)
        {
            dbset.Remove(entity);
        }

        public void Remove(int ID)
        {
            dbset.Remove(Get(ID));

        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbset.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _context.ChangeTracker.Clear();
            dbset.Update(entity);
        }
    }
}
