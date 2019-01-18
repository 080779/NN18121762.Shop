using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class MessageConfig : EntityTypeConfiguration<MessageEntity>
    {
        public MessageConfig()
        {
            ToTable("tb_messages");
            Property(p => p.Mobile).HasMaxLength(50).IsRequired();
            Property(p => p.Content).HasMaxLength(256);
        }
    }
}
