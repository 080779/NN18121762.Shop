using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class UserConfig : EntityTypeConfiguration<UserEntity>
    {
        public UserConfig()
        {
            ToTable("tb_users");
            Property(p => p.Mobile).HasMaxLength(50).IsRequired();
            Property(p => p.UserCode).HasMaxLength(50);
            Property(p => p.NickName).HasMaxLength(50);
            Property(p => p.Description).HasMaxLength(100);
            Property(p => p.HeadPic).HasMaxLength(256);
            Property(p => p.Salt).HasMaxLength(30);
            Property(p => p.Password).HasMaxLength(50);
            Property(p => p.TradePassword).HasMaxLength(50);
            Property(p => p.ShareCode).HasMaxLength(250);
            Property(p => p.RecommendCode).HasMaxLength(50);
            Property(p => p.RecommendPath);
        }
    }
}
