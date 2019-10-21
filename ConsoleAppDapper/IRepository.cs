using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleAppDapper
{
    public interface IRepository<T> //where T : EntityBase, IAggregateRoot
    {
        void Add(T item);
        void Remove(T item);
        void Update(T item);
        T FindByID(Guid id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        IEnumerable<T> FindAll();
    }
}
