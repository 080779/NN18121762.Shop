using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class GoodsConfig : EntityTypeConfiguration<GoodsEntity>
    {
        public GoodsConfig()
        {
            ToTable("tb_goods");
            Property(p => p.Name).HasMaxLength(256).IsRequired();
            Property(p => p.Code).HasMaxLength(30).IsRequired();
            Property(p => p.Standard).HasMaxLength(30).IsRequired();
            Property(p => p.Description).IsRequired();
            //HasRequired(g => g.GoodsArea).WithMany().HasForeignKey(g => g.GoodsAreaId).WillCascadeOnDelete(false);
            HasRequired(g => g.GoodsType).WithMany().HasForeignKey(g => g.GoodsTypeId).WillCascadeOnDelete(false);
            HasRequired(g => g.GoodsSecondType).WithMany().HasForeignKey(g => g.GoodsSecondTypeId).WillCascadeOnDelete(false);
            //HasRequired(g => g.GoodsImg).WithMany().HasForeignKey(g => g.GoodsImgId).WillCascadeOnDelete(false);
        }
    }
}
