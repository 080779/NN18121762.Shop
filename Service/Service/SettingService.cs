using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class SettingService : ISettingService
    {
        private SettingDTO ToDTO(SettingEntity entity)
        {
            SettingDTO dto = new SettingDTO();
            dto.Id = entity.Id;
            dto.LevelId = entity.LevelId;
            dto.Name = entity.Name;
            dto.Param = entity.Param;
            dto.Remark = entity.Remark;
            dto.TypeId = entity.TypeId;
            dto.TypeName = entity.TypeName;
            return dto;
        }

        public async Task<bool> EditAsync(long id, string parameter)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Param = parameter;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> EditAsync(params SettingSetDTO[] settings)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                foreach (SettingSetDTO setting in settings)
                {
                    SettingEntity entity = await dbc.GetAll<SettingEntity>().SingleOrDefaultAsync(g => g.Id == setting.Id);
                    if (entity == null)
                    {
                        return false;
                    }
                    entity.Param = setting.Param;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<string> GetParmByNameAsync(string name)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                return await dbc.GetStringPropertyAsync<SettingEntity>(s => s.Name == name, s => s.Param);
            }
        }

        public async Task<SettingDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SettingEntity entity = await dbc.GetAll<SettingEntity>().AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<SettingDTO[]> GetModelListIsEnableAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<SettingEntity>().AsNoTracking().Where(s => s.IsEnabled == true);
                var res = await entities.ToListAsync();
                return res.Select(s => ToDTO(s)).ToArray();
            }
        }
    }
}
