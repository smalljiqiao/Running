using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataCoreDomain.Entities
{
   public class BaseEntity
    {
        public String CreateUserId { get; set; } 

        public DateTime CreateTime { get; set; }

        public string UpdateUserId { get; set; }

        public DateTime? UpdateTime { get; set; }
      
        /// <summary>
        /// 是否已经删除
        /// </summary>
        public int IsDelete { get; set; }

    }
}
