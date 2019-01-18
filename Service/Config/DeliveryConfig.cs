using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class DeliveryConfig : EntityTypeConfiguration<DeliveryEntity>
    {
        public DeliveryConfig()
        {
            ToTable("tb_deliveries");

            Property(p => p.DeliveryName).HasMaxLength(50);
            Property(p => p.DeliveryCode).HasMaxLength(50);
            Property(p => p.ReceiverName).HasMaxLength(50);
            Property(p => p.ReceiverMobile).HasMaxLength(50);
            Property(p => p.ReceiverAddress).HasMaxLength(50);
            HasKey(r => r.OrderId);
            //HasRequired(p => p.Order).WithOptional(p => p.Delivery).WillCascadeOnDelete(false);
        }
    }
}
