using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class GoodsAreaConfig : EntityTypeConfiguration<GoodsAreaEntity>
    {
        public GoodsAreaConfig()
        {
            ToTable("tb_goodsareas");
            Property(p => p.Title).HasMaxLength(50).IsRequired();
            Property(p => p.Note).HasMaxLength(256).IsRequired();
            Property(p => p.Description).HasMaxLength(256);
        }
    }
}
