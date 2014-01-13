using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Amazon.EC2;
using Dapper;
using set_basic_aspnet_mvc.Domain.Entities;
using set_basic_aspnet_mvc.Helpers;

namespace set_basic_aspnet_mvc.Domain.Repositories
{
    public class DapperRepository<TEntity> : IRepository<TEntity> 
           where TEntity : BaseEntity
    {
        //todo: how to pass connection to repository?
        private readonly IDbConnection _dbConnection;

        public TEntity Create(TEntity entity)
        {
            var insertCommand = DapperHelper.CreateInsertSql<TEntity>();
            _dbConnection.Execute(insertCommand, entity);

            //todo:get inserted entity id and set it to return entity
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            var updateCommand = DapperHelper.CreateUpdateSql<TEntity>();
            _dbConnection.Execute(updateCommand, entity);

            //todo:get inserted entity id and set it to return entity
            return entity;
        }

        public void SoftDelete(long id, int deletedBy)
        {
            var softDeleteCommand = DapperHelper.CreateUpdateSqlForSoftDelete<TEntity>();
            _dbConnection.Execute(softDeleteCommand,
                new {DeletedBy = deletedBy, DeletedAt = DateTime.Now, IsDeleted = true, id});
        }

        public void SoftDelete(System.Linq.Expressions.Expression<Func<TEntity, bool>> where, int deletedBy)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            var deleteCommand = DapperHelper.CreateDeleteSql<TEntity>();
            _dbConnection.Execute(deleteCommand, new {id});
        }

        public void Delete(System.Linq.Expressions.Expression<Func<TEntity, bool>> where)
        {
            throw new NotImplementedException();
        }

        public TEntity FindOne(System.Linq.Expressions.Expression<Func<TEntity, bool>> where = null, params System.Linq.Expressions.Expression<Func<TEntity, object>>[] includeProperties)
        {
            return FindAll(where, includeProperties).First();
        }

        public IQueryable<TEntity> FindAll(System.Linq.Expressions.Expression<Func<TEntity, bool>> where = null, params System.Linq.Expressions.Expression<Func<TEntity, object>>[] includeProperties)
        {
            //todo: what is the need of includeProperties?
            var result = _dbConnection.Query<TEntity>("").AsQueryable();
            
            if (where != null)
                result = result.Where(where);

            return result;
        }

        public IQueryable<TEntity> AsQueryable(IQueryable<TEntity> existing, System.Linq.Expressions.Expression<Func<TEntity, bool>> where = null)
        {
            throw new NotImplementedException();
        }

        public bool SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}