using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class AddressConfig : EntityTypeConfiguration<AddressEntity>
    {
        public AddressConfig()
        {
            ToTable("tb_addresses");
            Property(p => p.Name).HasMaxLength(30).IsRequired();
            Property(p => p.Mobile).HasMaxLength(50).IsRequired();
            Property(p => p.Address).HasMaxLength(256);
            Property(p => p.Description).HasMaxLength(100);
            Property(p => p.Sheng).HasMaxLength(50);
            Property(p => p.Shi).HasMaxLength(50);
            Property(p => p.Qu).HasMaxLength(50);
        }
    }
}
