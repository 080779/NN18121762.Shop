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
    public class BankAccountService : IBankAccountService
    {
        public BankAccountDTO ToDTO(BankAccountEntity entity)
        {
            BankAccountDTO dto = new BankAccountDTO();
            dto.Name = entity.Name;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.BankAccount = entity.BankAccount;
            dto.BankName = entity.BankName;
            dto.Description = entity.Description;
            dto.UserId = entity.UserId;
            return dto;
        }

        public async Task<long> AddAsync(long userId, string name, string bankAccount, string bankName)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BankAccountEntity entity = new BankAccountEntity();
                entity.UserId = userId;
                entity.Name = name;
                entity.BankAccount = bankAccount;
                entity.BankName = bankName;
                dbc.BankAccounts.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> UpdateAsync(long id, string name, string bankAccount, string bankName)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BankAccountEntity entity = await dbc.GetAll<BankAccountEntity>().SingleOrDefaultAsync(b=>b.Id==id); 
                if(entity==null)
                {
                    return false;
                }
                entity.Name = name;
                entity.BankAccount = bankAccount;
                entity.BankName = bankName;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateByUserIdAsync(long id, string name, string bankAccount, string bankName)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BankAccountEntity entity = await dbc.GetAll<BankAccountEntity>().SingleOrDefaultAsync(b => b.UserId == id);
                if (entity == null)
                {
                    entity = new BankAccountEntity();
                    entity.Name = name;
                    entity.BankAccount = bankAccount;
                    entity.BankName = bankName;
                    entity.UserId = id;
                    dbc.BankAccounts.Add(entity);
                }
                else
                {
                    entity.Name = name;
                    entity.BankAccount = bankAccount;
                    entity.BankName = bankName;
                }                
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BankAccountEntity entity = await dbc.GetAll<BankAccountEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<BankAccountDTO[]> GetModelListByUserIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<BankAccountEntity>().AsNoTracking().Where(b=>b.IsNull==false && b.UserId==id);
                var bankAccounts = await entities.ToListAsync();
                return bankAccounts.Select(p => ToDTO(p)).ToArray();
            }
        }

        public async Task<BankAccountDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<BankAccountEntity>().AsNoTracking().SingleOrDefaultAsync(b => b.IsNull == false && b.Id == id);
                if(entity==null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<BankAccountDTO> GetModelByUserIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<BankAccountEntity>().AsNoTracking().SingleOrDefaultAsync(b => b.IsNull == false && b.UserId == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public Task<BankAccountSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
