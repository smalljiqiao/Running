using EFDataCoreDomain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EFDataCoreDomain.IRepositories
{
   public interface IRepository
    {
    }

    /// <summary>
    /// 一般是视图直接继承
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IQueryRepository<TEntity, TPrimaryKey> : IRepository where TEntity : DbEntityRoot<TPrimaryKey> {

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns>跟踪的数据集</returns>
        DbSet<TEntity> Entities();

        /// <summary>
        /// 无需跟踪的数据集
        /// </summary>
        /// <returns>不跟踪的数据集</returns>
        IQueryable<TEntity> EntitiesAsNoTracking();
        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        List<TEntity> GetAllList(bool isTrack = false);
        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate, bool isTrack = false);
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        TEntity Get(TPrimaryKey id, bool isTrack = false);
        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, bool isTrack = false);

    }

    /// <summary>
    /// 表实体继承
    /// </summary>
    /// <typeparam name="TEntity">表对应的实体类</typeparam>
    /// <typeparam name="TPrimary">实体类对应的主键类型</typeparam>
    public interface IRepository<TEntity, TPrimary> : IQueryRepository<TEntity, TPrimary>, IRepository where TEntity : DbEntityRoot<TPrimary> {
        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        TEntity Insert(TEntity entity, bool autoSave = true);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        TEntity Update(TEntity entity, bool autoSave = true);


        /// <summary>
        /// 添加或者更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        TEntity InsertOrUpdate(TEntity entity, bool autoSave = true);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        void Delete(TEntity entity, bool autoSave = true);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <param name="autoSave">是否立即执行保存</param>
        void Delete(TPrimary id, bool autoSave = true);

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">lambda表达式</param>
        /// <param name="autoSave">是否自动保存</param>
        void Delete(Expression<Func<TEntity, bool>> where, bool autoSave = true);

        void Save();

        int SaveChange();
    }


}
