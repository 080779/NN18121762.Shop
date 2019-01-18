using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class OrderApplyConfig : EntityTypeConfiguration<OrderApplyEntity>
    {
        public OrderApplyConfig()
        {
            ToTable("tb_orderapplies");

            Property(p => p.GoodsName).HasMaxLength(50).IsRequired();
            Property(p => p.ImgUrl).HasMaxLength(156).IsRequired();
        }
    }
}
