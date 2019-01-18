using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class IdNameConfig : EntityTypeConfiguration<IdNameEntity>
    {
        public IdNameConfig()
        {
            ToTable("tb_idnames");
            Property(p => p.TypeName).HasMaxLength(50).IsRequired();
            Property(p => p.Name).HasMaxLength(50).IsRequired();
            Property(p => p.Description).HasMaxLength(100);
        }
    }
}
