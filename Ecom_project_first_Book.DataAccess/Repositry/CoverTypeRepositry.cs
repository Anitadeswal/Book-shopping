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
    public class CoverTypeRepositry : Repositry<CoverType>,ICoverTypeRepositry
    {
        private readonly ApplicationDbContext _context;
        public CoverTypeRepositry(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
