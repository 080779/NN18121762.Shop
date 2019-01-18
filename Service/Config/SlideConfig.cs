using IMS.Service.Entity;
using System.Data.Entity.ModelConfiguration;

namespace IMS.Service.Config
{
    class SlideConfig : EntityTypeConfiguration<SlideEntity>
    {
        public SlideConfig()
        {
            ToTable("tb_slides");
            Property(p => p.Name).HasMaxLength(50).IsRequired();
            Property(p => p.Code).HasMaxLength(50).IsRequired();
            Property(p => p.Url).HasMaxLength(256);
            Property(p => p.ImgUrl).HasMaxLength(256);
        }
    }
}
