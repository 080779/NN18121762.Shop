using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class GoodsCarConfig : EntityTypeConfiguration<GoodsCarEntity>
    {
        public GoodsCarConfig()
        {
            ToTable("tb_goodscars");
            HasRequired(g => g.Goods).WithMany().HasForeignKey(g => g.GoodsId).WillCascadeOnDelete(false);
        }
    }
}
