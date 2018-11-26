using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataCoreDomain.Entities
{
   public class Goods_Color_Img_Entity
    {
        public int Id { get; set; }
        public string GoodId { get; set; }
        public string ColorId { get; set; }

        public String Img { get; set; }

        public bool IsDefault { get; set; }

    }
}
