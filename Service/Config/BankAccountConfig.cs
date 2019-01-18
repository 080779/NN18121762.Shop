using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class BankAccountConfig : EntityTypeConfiguration<BankAccountEntity>
    {
        public BankAccountConfig()
        {
            ToTable("tb_bankaccounts");
            Property(p => p.Name).HasMaxLength(50).IsRequired();
            Property(p => p.BankAccount).HasMaxLength(50).IsRequired();
            Property(p => p.BankName).HasMaxLength(50).IsRequired();
            Property(p => p.Description).HasMaxLength(100);            
            HasRequired(p => p.User).WithMany().HasForeignKey(p => p.UserId).WillCascadeOnDelete(false);
        }
    }
}
