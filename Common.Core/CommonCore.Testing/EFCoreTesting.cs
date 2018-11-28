using EFDataCoreDomain.Contexts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using HB.Common.Core.Framework.Helpers;

namespace CommonCore.Testing
{
    [TestClass]
    public class EFCoreTesting
    {
        /// <summary>
        /// 如果想查询不追踪要引入
        /// using Microsoft.EntityFrameworkCore;
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            using (var context = new QGDDbContexts()) {
                var query = context.goods_Color_Img_Entity.AsNoTracking().Where(p => p.IsDefault == false);
                var ss = query.ToList();
                Console.Write("ddddd545252");
                context.SaveChanges();
            }
        }
        [TestMethod]
        public void originalSQl() {

            using (var context = new QGDDbContexts()) {
                var query = context.goods_Color_Img_Entity.FromSql("select top(10) * from dbo.tb_Goods_Color_Img").ToList();
                Console.Write(JsonHelper.ToJson(query));

            }
        }

    }
}
