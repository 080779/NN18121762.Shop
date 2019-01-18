using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class AdminLogConfig:EntityTypeConfiguration<AdminLogEntity>
    {
        public AdminLogConfig()
        {
            ToTable("tb_adminLogs");
            HasRequired(a => a.Admin).WithMany().HasForeignKey(a => a.AdminId).WillCascadeOnDelete(false);
            HasRequired(a => a.PermissionType).WithMany().HasForeignKey(a => a.PermissionTypeId).WillCascadeOnDelete(false);
            Property(a => a.Description).HasMaxLength(500);
            Property(a => a.IpAddress).HasMaxLength(50);
            Property(a => a.Tip).HasMaxLength(500);
        }
    }
}
