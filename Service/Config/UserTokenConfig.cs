using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class UserTokenConfig : EntityTypeConfiguration<UserTokenEntity>
    {
        public UserTokenConfig()
        {
            ToTable("tb_usertokens");
            Property(p => p.Token).HasMaxLength(1024);
        }
    }
}
