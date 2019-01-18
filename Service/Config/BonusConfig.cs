using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class BonusConfig : EntityTypeConfiguration<BonusEntity>
    {
        public BonusConfig()
        {
            ToTable("tb_bonus");
        
        }
    }
}
