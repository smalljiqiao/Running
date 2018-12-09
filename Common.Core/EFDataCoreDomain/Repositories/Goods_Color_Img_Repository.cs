using EFDataCoreDomain.Contexts;
using EFDataCoreDomain.Entities;
using EFDataCoreDomain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataCoreDomain.Repositories
{
    public class Goods_Color_Img_Repository : BaseRepository<Goods_Color_Img_Entity, int>, IGoods_Color_Img_Repository {
        public Goods_Color_Img_Repository(QGDDbContexts dbContexts):base(dbContexts)
        {

        }

    }
}
