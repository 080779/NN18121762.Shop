using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class BonusRatioConfig : EntityTypeConfiguration<BonusRatioEntity>
    {
        public BonusRatioConfig()
        {
            ToTable("tb_bonusratios");
        }
    }
}
