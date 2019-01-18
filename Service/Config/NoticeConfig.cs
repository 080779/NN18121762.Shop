using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class NoticeConfig : EntityTypeConfiguration<NoticeEntity>
    {
        public NoticeConfig()
        {
            ToTable("tb_notices");
            Property(p => p.Code).HasMaxLength(30).IsRequired();
            Property(p => p.Content).HasMaxLength(2048);
            Property(p => p.Url).HasMaxLength(256);
        }
    }
}
