using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class PermissionConfig:EntityTypeConfiguration<PermissionEntity>
    {
        public PermissionConfig()
        {
            ToTable("tb_permissions");
            HasMany(p => p.Admins).WithMany(a => a.Permissions).Map(p => p.ToTable("tb_adminpermissions").MapLeftKey("AdminId").MapRightKey("PermissionId"));
            HasRequired(p => p.PermissionType).WithMany().HasForeignKey(p=>p.PermissionTypeId).WillCascadeOnDelete(false);
            Property(p => p.Name).HasMaxLength(50).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
