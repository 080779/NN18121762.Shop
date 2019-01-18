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
    public class MessageService : IMessageService
    {
        private MessageDTO ToDTO(MessageEntity entity)
        {
            MessageDTO dto = new MessageDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.UserId = entity.UserId;
            dto.Content = entity.Content;
            dto.Mobile = entity.Mobile;
            dto.Flag = entity.Flag;
            return dto;
        }

        public async Task<long> AddAsync(long? userId, string mobile, string content, int? flag)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                MessageEntity entity = new MessageEntity();
                entity.UserId = userId;
                entity.Content = content;
                entity.Mobile = mobile;
                entity.Flag = flag;
                dbc.Messages.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }
    }
}
