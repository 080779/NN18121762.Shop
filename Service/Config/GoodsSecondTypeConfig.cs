using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class GoodsSecondTypeConfig : EntityTypeConfiguration<GoodsSecondTypeEntity>
    {
        public GoodsSecondTypeConfig()
        {
            ToTable("tb_goodssecondtypes");
            Property(p => p.Name).HasMaxLength(50).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
