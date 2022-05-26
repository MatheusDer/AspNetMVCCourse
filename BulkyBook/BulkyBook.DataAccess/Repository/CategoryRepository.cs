using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) 
            : base(context)
        { 
            _context = context;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Category cateagory)
        {
            _context.Update(cateagory);
        }
    }
}
