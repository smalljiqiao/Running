using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataCoreDomain.Entities
{
    public abstract class DbEntityRoot<TPrimary> { }

    public abstract class DbEntityBase<TPrimary> : DbEntityRoot<TPrimary>
    {
        public TPrimary Id { get; set; }
        public String CreateUserId { get; set; }

        public DateTime CreateTime { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否已经删除
        /// </summary>
        public int IsDelete { get; set; }

    }


    public abstract class DbEntity<TPrimary> : DbEntityBase<TPrimary>
    {

    }
}
