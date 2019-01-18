using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class OrderListConfig : EntityTypeConfiguration<OrderListEntity>
    {
        public OrderListConfig()
        {
            ToTable("tb_orderlists");

            Property(p => p.ImgUrl).HasMaxLength(156).IsRequired();

            HasRequired(p => p.Goods).WithMany().HasForeignKey(p => p.GoodsId).WillCascadeOnDelete(false);
        }
    }
}
