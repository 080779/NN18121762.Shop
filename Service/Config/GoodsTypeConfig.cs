using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class GoodsTypeConfig : EntityTypeConfiguration<GoodsTypeEntity>
    {
        public GoodsTypeConfig()
        {
            ToTable("tb_goodstypes");
            Property(p => p.Name).HasMaxLength(50).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
