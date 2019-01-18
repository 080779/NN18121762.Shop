using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class GoodsImgConfig : EntityTypeConfiguration<GoodsImgEntity>
    {
        public GoodsImgConfig()
        {
            ToTable("tb_goodsimgs");
            Property(p => p.Name).HasMaxLength(30).IsRequired();
            Property(p => p.ImgUrl).HasMaxLength(256);
            Property(p => p.Description).HasMaxLength(100);
            HasRequired(g => g.Goods).WithMany().HasForeignKey(g => g.GoodsId).WillCascadeOnDelete(false);
        }
    }
}
