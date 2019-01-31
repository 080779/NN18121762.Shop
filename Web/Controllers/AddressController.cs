using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Address;
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
    public class AddressController : ApiController
    {
        public IAddressService addressService { get; set; }

        [HttpPost]
        public async Task<ApiResult> List(AddressListModel model)
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            AddressSearchResult result = await addressService.GetModelListAsync(user.Id, null, null, null, model.PageIndex, model.PageSize);
            AddressListApiModel res = new AddressListApiModel();
            res.pageCount = result.PageCount;
            res.addressList = result.Address.Select(a => new AddressList { id = a.Id,sheng=a.Sheng,shi=a.Shi,qu=a.Qu,address = a.Address, name = a.Name, mobile = a.Mobile, isDefault = a.IsDefault }).ToList();
            return new ApiResult { status = 1, data = res };
        }
        [HttpPost]
        public async Task<ApiResult> Detail(AddressDetailModel model)
        {
            AddressDTO result = await addressService.GetModelAsync(model.Id);
            AddressList apiModel = new AddressList();
            apiModel.address = result.Address;
            apiModel.id = result.Id;
            apiModel.mobile = result.Mobile;
            apiModel.name = result.Name;
            apiModel.isDefault = result.IsDefault;
            return new ApiResult { status = 1, data = apiModel };
        }
        [HttpPost]
        public async Task<ApiResult> Default()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            AddressDTO result = await addressService.GetDefaultModelAsync(user.Id);
            if (result == null)
            {
                return new ApiResult { status = 1 };
            }
            else
            {
                AddressList apiModel = new AddressList();
                apiModel.address = result.Address;
                apiModel.id = result.Id;
                apiModel.mobile = result.Mobile;
                apiModel.name = result.Name;
                return new ApiResult { status = 1, data = apiModel };
            }
        }
        [HttpPost]
        public async Task<ApiResult> Add(AddressAddModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return new ApiResult { status = 0, msg = "收货人姓名不能为空" };
            }
            if (string.IsNullOrEmpty(model.Mobile))
            {
                return new ApiResult { status = 0, msg = "收货人手机号不能为空" };
            }
            if (!Regex.IsMatch(model.Mobile, @"^1\d{10}$"))
            {
                return new ApiResult { status = 0, msg = "收货人手机号格式不正确" };
            }
            if (string.IsNullOrEmpty(model.Address))
            {
                return new ApiResult { status = 0, msg = "收货人地址不能为空" };
            }
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id = await addressService.AddAsync(user.Id, model.Name, model.Mobile,model.Sheng,model.Shi,model.Qu, model.Address, model.IsDefault);
            if (id <= 0)
            {
                return new ApiResult { status = 1, msg = "收货地址添加失败" };
            }
            return new ApiResult { status = 1, msg = "收货地址添加成功" };
        }
        [HttpPost]
        public async Task<ApiResult> Edit(AddressEditModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return new ApiResult { status = 0, msg = "收货人姓名不能为空" };
            }
            if (string.IsNullOrEmpty(model.Mobile))
            {
                return new ApiResult { status = 0, msg = "收货人手机号不能为空" };
            }
            if (!Regex.IsMatch(model.Mobile, @"^1\d{10}$"))
            {
                return new ApiResult { status = 0, msg = "收货人手机号格式不正确" };
            }
            if (string.IsNullOrEmpty(model.Address))
            {
                return new ApiResult { status = 0, msg = "收货人地址不能为空" };
            }
            bool flag = await addressService.UpdateAsync(model.Id, model.Name, model.Mobile, model.Sheng, model.Shi, model.Qu, model.Address, model.IsDefault);
            if (!flag)
            {
                return new ApiResult { status = 0, msg = "收货地址修改失败" };
            }
            return new ApiResult { status = 1, msg = "收货地址修改成功" };
        }
        public async Task<ApiResult> Del(AddressDelModel model)
        {
            bool flag = await addressService.DeleteAsync(model.Id);
            if (!flag)
            {
                return new ApiResult { status = 0, msg = "收货地址删除失败" };
            }
            return new ApiResult { status = 1, msg = "收货地址删除成功" };
        }
    }
}