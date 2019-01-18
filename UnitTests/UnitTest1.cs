using System;
using System.Threading.Tasks;
using IMS.Common;
using IMS.Service.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private OrderService orderService =new OrderService();
        private GoodsCarService goodsCarService = new GoodsCarService();
        private OrderApplyService orderApplyService = new OrderApplyService();
        private UserService userService = new UserService();
        private TakeCashService takeCashService = new TakeCashService();
        [TestMethod]
        public async Task TestMethod1()
        {
            //long userId = 7;
            //int num = 2;
            //var carid = await goodsCarService.AddAsync(userId, 4, num);
            //await goodsCarService.UpdateAsync(carid, num, true);
            //var res = await goodsCarService.GetModelListAsync(userId);
            //await orderApplyService.AddAsync(res);
            //var dtos = await orderApplyService.GetModelListAsync(userId);
            //long id = await orderService.AddAsync(null, 0, userId, 124, 1, 1, dtos.OrderApplies);
            //await goodsCarService.DeleteListAsync(userId);
            //await orderApplyService.DeleteListAsync(userId);
            await userService.WeChatPayAsync("dd201901172022586787570");
        }
        //[TestMethod]
        //public async Task TestMethod()
        //{
        //    //await takeCashService.AddAsync(2, 0, 100, "佣金提现");
        //    //await userService.AddAsync("18677055512","123456",null,0,"15615615616",null,null);
        //    var res = await userService.GetModelTeamListAsync(1, 2, null, null, null, 1, 50);
        //}
    }
}
