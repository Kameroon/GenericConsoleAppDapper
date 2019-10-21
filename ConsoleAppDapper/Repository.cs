using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDapper
{
    public abstract class Repository<T> : IRepository<T> //where T : EntityBase, IAggregateRoot
    {
        private readonly string _tableName;

        internal IDbConnection Connection
        {
            get
            {
                return new SqlConnection(ConfigurationManager.ConnectionStrings["SmsQuizConnection"].ConnectionString);
            }
        }

        public Repository(string tableName)
        {
            _tableName = tableName;
        }

        internal virtual dynamic Mapping(T item)
        {
            return item;
        }

        public virtual void Add(T item)
        {
            using (IDbConnection cn = Connection)
            {
                var parameters = (object)Mapping(item);
                cn.Open();
                //item.ID = cn.Insert<Guid>(_tableName, parameters);
            }
        }

        public virtual void Update(T item)
        {
            using (IDbConnection cn = Connection)
            {
                var parameters = (object)Mapping(item);
                cn.Open();
                //cn.Update(_tableName, parameters);
            }
        }

        public virtual void Remove(T item)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                //cn.Execute("DELETE FROM " + _tableName + " WHERE ID=@ID", new { ID = item.ID });
            }
        }

        public virtual T FindByID(Guid id)
        {
            T item = default(T);

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                item = cn.Query<T>("SELECT * FROM " + _tableName + " WHERE ID=@ID", new { ID = id }).SingleOrDefault();
            }

            return item;
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> items = null;

            // extract the dynamic sql query and parameters from predicate
            var result = DynamicQuery.GetDynamicQuery(_tableName, predicate);

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                items = cn.Query<T>(result.Sql, (object)result.Param);
            }

            return items;
        }

        public virtual IEnumerable<T> FindAll()
        {
            IEnumerable<T> items = null;

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                items = cn.Query<T>("SELECT * FROM " + _tableName);
            }

            return items;
        }
    }

    public static class DapperExtensions
    {
        public static T Insert<T>(this IDbConnection cnn, string tableName, dynamic param)
        {
            IEnumerable<T> result = SqlMapper.Query<T>(cnn, DynamicQuery.GetInsertQuery(tableName, param), param);
            return result.First();
        }

        public static void Update(this IDbConnection cnn, string tableName, dynamic param)
        {
            SqlMapper.Execute(cnn, DynamicQuery.GetUpdateQuery(tableName, param), param);
        }
    }
}
