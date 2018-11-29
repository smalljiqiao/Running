using EFDataCoreDomain.Entities;
using EFDataCoreDomain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace EFDataCoreDomain.Repositories
{

  public abstract  class BaseRepository<TEntity, TPrimaryKey> : IRepository<TEntity,TPrimaryKey> where TEntity :DbEntityRoot<TPrimaryKey>
    {
        protected DbContext _dbContext;
        public BaseRepository(DbContext dbContext)
        {
            if (dbContext == null) { throw new FieldAccessException("数据库上下文为NULL"); }
            else _dbContext = dbContext;
        }

        public void Delete(TEntity entity, bool autoSave = true)
        {
            this._dbContext.Remove(entity);
        }

        public void Delete(TPrimaryKey id, bool autoSave = true)
        {
            this._dbContext.Remove(Get(id, true));
        }
        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">lambda表达式</param>
        /// <param name="autoSave">是否自动保存</param>
        public void Delete(Expression<Func<TEntity, bool>> where, bool autoSave = true)
        {
            this._dbContext.Set<TEntity>().Where(where).ToList().ForEach(p => _dbContext.Set<TEntity>().Remove(p));
            if (autoSave)
                Save();
        }

        public DbSet<TEntity> Entities()
        {
            return this._dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> EntitiesAsNoTracking()
        {
            return this._dbContext.Set<TEntity>().AsNoTracking();
        }
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        private IQueryable<TEntity> GetEntites(bool isTrack)
        {
            return isTrack ? this.Entities() : this.EntitiesAsNoTracking();
        }
        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, bool isTrack = false)
        {
                return this.GetEntites(isTrack).FirstOrDefault(predicate);
        }
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        public TEntity Get(TPrimaryKey id, bool isTrack = false)
        {
          return  this.GetEntites(isTrack).FirstOrDefault(CreateEqualityExpressionForId(id));
        }
        /// <summary>
        /// 根据主键构建判断表达式
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));
            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );
            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        public List<TEntity> GetAllList(bool isTrack = false)
        {
           return this.GetEntites(isTrack).ToList();
        }
        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate, bool isTrack = false)
        {
          return  this.GetEntites(isTrack).Where(predicate).ToList();
        }

        public TEntity Insert(TEntity entity, bool autoSave = true)
        {
            _dbContext.Set<TEntity>().Add(entity);
            if (autoSave)
                Save();
            return entity;
        }

        public TEntity InsertOrUpdate(TEntity entity, bool autoSave = true)
        {
            throw new Exception("");
        }

        public void Save()
        {
            if(_dbContext!=null)
            this._dbContext.SaveChanges();
        }

        public int SaveChange()
        {
            if (_dbContext != null)
            {
                var count = _dbContext.SaveChanges();
                return count;
            }
            else { return 0; }
        }

        public TEntity Update(TEntity entity, bool autoSave = true)
        {
            throw new NotImplementedException();
        }
    }
}
