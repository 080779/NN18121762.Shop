using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.BankAccount;
using IMS.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    public class BankAccountController : ApiController
    {        
        public IBankAccountService bankAccountService { get; set; }

        [HttpPost]
        public async Task<ApiResult> Info()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            BankAccountDTO dto = await bankAccountService.GetModelByUserIdAsync(user.Id);
            BankAccountInfoApiModel apiModel = new BankAccountInfoApiModel();
            if(dto==null)
            {
                return new ApiResult { status = 0, msg = "银行卡为空" };
            }
            apiModel.bankAccount = dto.BankAccount;
            apiModel.bankName = dto.BankName;
            apiModel.id = dto.Id;
            apiModel.name = dto.Name;
            return new ApiResult { status = 1, data=apiModel };
        }
        [HttpPost]
        public async Task<ApiResult> Edit(BankAccountEditModel model)
        {
            if (string.IsNullOrEmpty(model.BankAccount))
            {
                return new ApiResult { status = 0, msg = "银行卡号不能为空" };
            }
            if (string.IsNullOrEmpty(model.BankName))
            {
                return new ApiResult { status = 0, msg = "开户银行不能为空" };
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                return new ApiResult { status = 0, msg = "持卡人姓名不能为空" };
            }
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            bool flag = await bankAccountService.UpdateByUserIdAsync(user.Id, model.Name, model.BankAccount, model.BankName);
            if (!flag)
            {
                return new ApiResult { status = 0, msg = "修改银行卡失败" };
            }
            return new ApiResult { status = 1, msg = "修改银行卡成功" };
        }
    }    
}