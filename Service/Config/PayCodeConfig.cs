using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class PayCodeConfig : EntityTypeConfiguration<PayCodeEntity>
    {
        public PayCodeConfig()
        {
            ToTable("tb_paycodes");
            Property(p => p.Name).HasMaxLength(30).IsRequired();
            Property(p => p.Description).HasMaxLength(256);
            Property(p => p.CodeUrl).HasMaxLength(256);
            HasRequired(p => p.User).WithMany().HasForeignKey(p => p.UserId).WillCascadeOnDelete(false);
        }
    }
}
