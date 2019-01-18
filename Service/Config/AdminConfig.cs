using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class AdminConfig:EntityTypeConfiguration<AdminEntity>
    {
        public AdminConfig()
        {
            ToTable("tb_admins");
            Property(a => a.Mobile).HasMaxLength(50).IsRequired();
            Property(a => a.Description).HasMaxLength(100);
            Property(a => a.Salt).HasMaxLength(20).IsRequired().IsUnicode();
            Property(a => a.Password).HasMaxLength(100).IsRequired().IsUnicode();
        }
    }
}
