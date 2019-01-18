using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class LogisticsConfig : EntityTypeConfiguration<LogisticsEntity>
    {
        public LogisticsConfig()
        {
            ToTable("tb_logistics");
            Property(p => p.Name).HasMaxLength(30).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
