using EFDataCoreDomain.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EFDataCoreDomain.Services
{
   public class Goods_Color_Img_Service: BaseService
    {
        public void selectData() {
            var query = dbContexts.goods_Color_Img_Entity.AsNoTracking().Where(p => p.IsDefault == false);


        }
    }
}
