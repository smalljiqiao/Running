using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFDataCoreDomain.Entities
{
    [Table("tb_Goods_Color_Img")]
   public class Goods_Color_Img_Entity
    {
        public int Id { get; set; }

        [Column("GoodsId")]
        public string GoodId { get; set; }
        public string ColorId { get; set; }
        public String Img { get; set; }
        public bool IsDefault { get; set; }

    }
}
