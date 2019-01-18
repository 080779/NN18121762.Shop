using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class PermissionService : IPermissionService
    {
        public PermissionDTO ToDTO(PermissionEntity entity)
        {
            PermissionDTO dto = new PermissionDTO();
            dto.Description = entity.Description;
            dto.Name = entity.Name;
            dto.PermissionTypeId = entity.PermissionTypeId;
            dto.PermissionTypeName = entity.PermissionType.Name;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            return dto;
        }
        public async Task<PermissionDTO[]> GetByTypeIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<PermissionEntity>().Include(p=>p.PermissionType).AsNoTracking().Where(p => p.PermissionTypeId == id);
                var permissions = await entities.ToListAsync();
                return permissions.Select(p => ToDTO(p)).ToArray();
            }
        }

        public PermissionDTO GetByDesc(string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = dbc.GetAll<PermissionEntity>().Include(p => p.PermissionType).AsNoTracking().SingleOrDefault(p => p.Description == description);
                return ToDTO(entity);
            }
        }        
    }
}
