using IMS.Common;
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
    public class AdminService : IAdminService
    {
        public AdminDTO ToDTO(AdminEntity entity)
        {
            AdminDTO dto = new AdminDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Mobile = entity.Mobile;
            dto.IsEnabled = entity.IsEnabled;
            dto.PermissionIds = entity.Permissions.Select(p => p.Id).ToArray();
            return dto;
        }
        public async Task<long> AddAsync(string adminMobile,string mobile, string description, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                using (var scope = dbc.Database.BeginTransaction())
                {
                    try
                    {
                        var user = dbc.GetAll<AdminEntity>().SingleOrDefault(a => a.Mobile == mobile);
                        if (user != null)
                        {
                            return -2;
                        }

                        AdminEntity entity = new AdminEntity();
                        entity.Mobile = mobile;
                        entity.Description = description;
                        entity.Salt = CommonHelper.GetCaptcha(4);
                        entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                        dbc.Admins.Add(entity);
                        await dbc.SaveChangesAsync();
                        scope.Commit();
                        return entity.Id;
                    }
                    catch(Exception ex)
                    {
                        scope.Rollback();
                        return -1;
                    }
                }                    
            }
        }
        public async Task<bool> UpdateAsync(long id, string mobile, string description, string password, long[] permissionIds)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if(entity==null)
                {
                    return false;
                }

                entity.Mobile = mobile;
                entity.Description = description;
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                entity.Permissions.Clear();
                var perms = dbc.GetAll<PermissionEntity>().Where(p => permissionIds.Contains(p.Id));
                await perms.ForEachAsync(p => entity.Permissions.Add(p));
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateAsync(long id, long[] permissionIds)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Permissions.Clear();
                var perms = dbc.GetAll<PermissionEntity>().Where(p => permissionIds.Contains(p.Id));
                await perms.ForEachAsync(p => entity.Permissions.Add(p));
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateAsync(long id, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                await dbc.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return false;
                }

                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> FrozenAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return false;
                }

                entity.IsEnabled = !entity.IsEnabled;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<AdminDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().AsNoTracking().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<string> GetMobileAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                return await dbc.GetStringPropertyAsync<AdminEntity>(a => a.Id == id, a => a.Mobile);
            }
        }

        public async Task<AdminSearchResult> GetModelListAsync(string isAdmin,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AdminSearchResult result = new AdminSearchResult();
                var admins = dbc.GetAll<AdminEntity>().AsNoTracking();
                if(isAdmin!="admin")
                {
                    admins = admins.Where(a => a.Mobile== isAdmin);
                }
                else
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        admins = admins.Where(a => a.Mobile.Contains(keyword));
                    }
                    if (startTime != null)
                    {
                        admins = admins.Where(a => a.CreateTime>=startTime);
                    }
                    if (endTime != null)
                    {
                        admins = admins.Where(a => SqlFunctions.DateDiff("day",endTime, a.CreateTime) <=0);
                    }
                }
                result.PageCount = (int)Math.Ceiling((await admins.LongCountAsync()) * 1.0f / pageSize);
                var adminsResult= await admins.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Admins = adminsResult.Select(a => ToDTO(a)).ToArray();                
                return result;
            }
        }

        public async Task<AdminSearchResult> GetModelListHasPerAsync(string isAdmin, string mobile, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AdminSearchResult result = new AdminSearchResult();
                var admins = dbc.GetAll<AdminEntity>().AsNoTracking();
                if (isAdmin != "admin")
                {
                    admins = admins.Where(a => a.Mobile != "admin");
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    admins = admins.Where(a => a.Mobile.Contains(mobile));
                }
                if (startTime != null)
                {
                    admins = admins.Where(a => a.CreateTime.Year >= startTime.Value.Year && a.CreateTime.Month >= startTime.Value.Month && a.CreateTime.Day >= startTime.Value.Day);
                }
                if (endTime != null)
                {
                    admins = admins.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await admins.LongCountAsync()) * 1.0f / pageSize);
                var adminsResult = await admins.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Admins = adminsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public bool HasPermission(long id, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var admin = dbc.GetAll<AdminEntity>().Include(a=>a.Permissions).AsNoTracking().SingleOrDefault(a => a.Id == id);
                if(admin==null)
                {
                    return false;
                }
                return admin.Permissions.Any(p => p.Description.Contains(description));
            }
        }

        public async Task<long> CheckLogin(string mobile, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var admin = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Mobile == mobile);
                if (admin == null)
                {
                    return -1;
                }
                if(!admin.IsEnabled)
                {
                    return -2;
                }
                string pwd = CommonHelper.GetMD5(password + admin.Salt);
                if (admin.Password!=pwd)
                {
                    return -3;
                }
                return admin.Id;
            }
        }

        public async Task<bool> DelAll()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                await dbc.Database.ExecuteSqlCommandAsync("exec del_all");
                return true;
            }
        }
    }
}
