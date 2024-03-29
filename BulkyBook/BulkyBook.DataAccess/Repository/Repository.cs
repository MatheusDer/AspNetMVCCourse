﻿using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            this.dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if(filter is not null)
                query = query.Where(filter);

            if (includeProperties is not null)
            {
                foreach(var includeProp in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProp);
            }

            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool trakced = true)
        {
            var query = CreateQuery(trakced);

            query = query.Where(filter);

            if (includeProperties is not null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProp);
            }

            return query.FirstOrDefault();
        }

        private IQueryable<T> CreateQuery(bool trakced)
        {
            if (trakced == false)
                return dbSet.AsNoTracking();

            return dbSet;
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Remove(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);

        }
    }
}
