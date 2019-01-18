using IMS.Common;
using IMS.Common.Enums;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Goods;
using IMS.Web.Models.Order;
using IMS.Web.WxPay;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using static IMS.Common.Enums.MyEnumHelper;

namespace IMS.Web.Controllers
{
    public class OrderController : ApiController
    {
        private string doMain = System.Configuration.ConfigurationManager.AppSettings["DoMain"];
        private static ILog log = LogManager.GetLogger(typeof(OrderController));
        public IOrderService orderService { get; set; }
        public IOrderListService orderListService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IUserService userService { get; set; }
        public IOrderApplyService orderApplyService { get; set; }
        public IGoodsService goodsService { get; set; }
        public IGoodsImgService goodsImgService { get; set; }
        public IGoodsCarService goodsCarService { get; set; }
        public ISettingService settingService { get; set; }

        [HttpPost]
        public async Task<ApiResult> List(OrderListModel model)
        {
            string parm = doMain;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            OrderSearchResult result = await orderService.GetModelListAsync(user.Id, model.OrderStateId, null, null, null, null, model.PageIndex, model.PageSize);
            OrderListApiModel res = new OrderListApiModel();
            res.PageCount = result.PageCount;
            res.Orders = result.Orders.Where(o=>o.IsRated==false).Select(o => new Order
            {
                amount = o.Amount,
                code = o.Code,
                createTime = o.CreateTime,
                postFee = o.PostFee,
                deliver = o.Deliver,
                deliveryCode = o.DeliverCode,
                deliveryName = o.DeliverName,
                id = o.Id,
                orderStateId = o.OrderStateId,
                orderStateName = o.OrderStateName,
                payTypeId = o.PayTypeId,
                payTypeName = o.PayTypeName,
                receiverName = o.ReceiverName,
                receiverMobile = o.ReceiverMobile,
                receiverAddress = o.ReceiverAddress,
                payTime = o.PayTime,
                consignTime = o.ConsignTime,
                endTime = o.EndTime,
                closeTime = o.CloseTime,
                discountAmount = o.DiscountAmount,
                totalAmount = orderListService.GetModelList(o.Id).Sum(ol => ol.TotalFee),
                OrderGoods = orderListService.GetModelList(o.Id).Select(l => new OrderGoods {
                    name = l.GoodsName,
                    number = l.Number,
                    price = l.Price,
                    realityPrice = l.RealityPrice,
                    totalFee = l.TotalFee,
                    inventory = l.Inventory,
                    imgUrl = parm + l.ImgUrl
                }).ToList()
            }).ToList();
            return new ApiResult { status = 1, data = res };
        }
        [HttpPost]
        public async Task<ApiResult> GoodsList(OrderGoodsListModel model)
        {
            string parm = doMain;
            var res = await orderListService.GetModelListAsync(model.OrderId, null, null, null, model.PageIndex, model.PageSize);
            var result = new OrderGoodsListApiModel();
            result.pageCount = res.PageCount;
            result.goodsLists = res.OrderLists.Select(o => new OrderList
            {
                goodsName = o.GoodsName,
                id = o.Id,
                imgUrl = parm + o.ImgUrl,
                isReturn = o.IsReturn,
                number = o.Number,
                orderCode = o.OrderCode,
                orderId = o.OrderId,
                price = o.Price,
                tealityPrice = o.RealityPrice,
                totalFee = o.TotalFee,
                inventory = o.Inventory,
                discountFee = Math.Truncate(o.DiscountFee * 100) / 100
            });
            return new ApiResult { status = 1, data = result };
        }

        [HttpPost]
        public async Task<ApiResult> GoodsSelect(OrderGoodsSelectModel model)
        {
            await orderListService.ReSetIsReturnAsync(model.OrderId);
            bool flag = await orderListService.SetIsReturnAsync(model.Id);
            if (!flag)
            {
                return new ApiResult { status = 0, msg = "选择出错" };
            }
            return new ApiResult { status = 1, msg = "成功" };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> Detail(OrderDetailModel model)
        {
            string parm = doMain;
            var o = await orderService.GetModelAsync(model.Id);
            if (o == null)
            {
                return new ApiResult { status = 0, msg = "订单不存在" };
            }
            Order order = new Order
            {
                amount = o.Amount,
                code = o.Code,
                createTime = o.CreateTime,
                postFee = o.PostFee,
                deliver = o.Deliver,
                deliveryCode = o.DeliverCode,
                deliveryName = o.DeliverName,
                id = o.Id,
                orderStateId = o.OrderStateId,
                orderStateName = o.OrderStateName,
                payTypeId = o.PayTypeId,
                payTypeName = o.PayTypeName,
                receiverName = o.ReceiverName,
                receiverMobile = o.ReceiverMobile,
                receiverAddress = o.ReceiverAddress,
                payTime = o.PayTime,
                consignTime = o.ConsignTime,
                endTime = o.EndTime,
                closeTime = o.CloseTime,
                totalAmount = orderListService.GetModelList(o.Id).Sum(ol => ol.TotalFee),
                OrderGoods = orderListService.GetModelList(o.Id).Select(l => new OrderGoods
                {
                    name = l.GoodsName,
                    number = l.Number,
                    price = l.Price,
                    realityPrice = l.RealityPrice,
                    totalFee = l.TotalFee,
                    inventory = l.Inventory,
                    imgUrl = parm + l.ImgUrl
                }).ToList()
            };
            return new ApiResult { status = 1, data = order };
        }

        public async Task<ApiResult> Place(OrderPlaceModel model)
        {
            GoodsCarDTO dto = new GoodsCarDTO();
            var goods = await goodsService.GetModelAsync(model.GoodsId);
            if (goods == null)
            {
                return new ApiResult { status = 0, msg = "商品不存在" };
            }
            if (goods.Inventory < model.Number)
            {
                return new ApiResult { status = 0, msg = "商品库存不足" };
            }
            dto.GoodsId = model.GoodsId;
            dto.ImgUrl = goodsImgService.GetFirstImg(model.GoodsId);
            dto.Name = goods.Name;
            dto.Number = model.Number;
            dto.RealityPrice = goods.RealityPrice;
            dto.Price = goods.Price;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            await orderApplyService.DeleteListAsync(user.Id);
            dto.UserId = user.Id;
            dto.GoodsAmount = dto.RealityPrice * dto.Number;
            long id = await orderApplyService.AddAsync(dto);
            if (id <= 0)
            {
                return new ApiResult { status = 0, msg = "下单失败" };
            }
            return new ApiResult { status = 1, msg = "下单成功" };
        }

        public async Task<ApiResult> Places()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            await orderApplyService.DeleteListAsync(user.Id);
            var res = await goodsCarService.GetModelListAsync(user.Id);
            if (res.Count() <= 0)
            {
                return new ApiResult { status = 0, msg = "购物车没有商品" };
            }
            long id = await orderApplyService.AddAsync(res);
            if (id <= 0)
            {
                return new ApiResult { status = 0, msg = "下单失败" };
            }
            return new ApiResult { status = 1, msg = "下单成功" };
        }

        public async Task<ApiResult> PlaceList()
        {
            string parm = doMain;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var res = await orderApplyService.GetModelListAsync(user.Id);
            OrderPlaceListApiModel model = new OrderPlaceListApiModel();
            model.totalAmount = res.ToTalAmount;
            model.orderPlaces = res.OrderApplies.Select(o => new OrderPlace { GoodsId = o.GoodsId, GoodsName = o.GoodsName, ImgUrl =parm + o.ImgUrl, Number = o.Number, Price = o.Price, TotalFee = o.TotalFee, UserId = o.UserId }).ToList();
            return new ApiResult { status = 1, data = model };
        }

        [HttpPost]
        public async Task<ApiResult> Apply(OrderApplyModel model)
        {
            int orderStateId = (int)OrderStateEnum.待付款;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id = await orderService.AddAsync(user.Id, model.AddressId, model.PayTypeId, orderStateId, model.GoodsId, model.Number);
            if (id <= 0)
            {
                return new ApiResult { status = 0, msg = "生成订单失败" };
            }
            long payTypeId = await idNameService.GetIdByNameAsync("余额");
            OrderDTO dto = await orderService.GetModelAsync(id);
            if (payTypeId == model.PayTypeId)
            {
                long payResId = await userService.BalancePayAsync(id);
                if (payResId == -1)
                {
                    return new ApiResult { status = 0, msg = "用户不存在" };
                }
                if (payResId == -2)
                {
                    return new ApiResult { status = 0, msg = "用户账户余额不足" };
                }
                if (payResId == -3)
                {
                    return new ApiResult { status = 0, msg = "订单不存在" };
                }
                if (id == -6)
                {
                    return new ApiResult { status = 0, msg = "用户账户已经冻结", data = id };
                }
                orderStateId = (int)OrderStateEnum.待发货;
                await orderService.UpdateAsync(id, null, null, orderStateId);
            }
            return new ApiResult { status = 1, msg = "支付成功" };
        }
        [HttpPost]
        public async Task<ApiResult> Applys(OrderApplysModel model)
        {
            int orderStateId = (int)OrderStateEnum.待付款;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var dtos = await orderApplyService.GetModelListAsync(user.Id);
            if (dtos.OrderApplies.Count() <= 0)
            {
                return new ApiResult { status = 0, msg = "下单列表无商品" };
            }
            long id = await orderService.AddAsync(model.DeliveryTypeId,0, user.Id, model.AddressId, model.PayTypeId, orderStateId, dtos.OrderApplies);
            if (id <= 0)
            {
                if(id==-4)
                {
                    return new ApiResult { status = 0, msg = "订单中有商品已经下架，请重新下单", data = 0 };
                }
                if (id == -6)
                {
                    return new ApiResult { status = 0, msg = "用户账户已经冻结", data = id };
                }
                return new ApiResult { status = 0, msg = "生成订单失败"};
            }
            await goodsCarService.DeleteListAsync(user.Id);
            await orderApplyService.DeleteListAsync(user.Id);

            OrderDTO dto = await orderService.GetModelAsync(id);
            if (model.PayTypeId==(int)PayTypeEnum.微信)//余额
            {
                long payResId = await userService.BalancePayAsync(id);
                if (payResId == -1)
                {
                    return new ApiResult { status = 0, msg = "订单不存在"};
                }
                if (payResId == -2)
                {
                    return new ApiResult { status = 0, msg = "用户不存在", data = id };
                }
                if (payResId == -3)
                {
                    return new ApiResult { status = 0, msg = "商品库存不足", data = id };
                }
                if (payResId == -4)
                {
                    return new ApiResult { status = 0, msg = "用户账户余额不足", data = id };
                }
                if (id == -6)
                {
                    return new ApiResult { status = 0, msg = "用户账户已经冻结", data = id };
                }
            }            
            return new ApiResult { status = 1, msg = "支付成功",data=id };
        }

        [HttpPost]
        public async Task<ApiResult> ReApplys(OrderReApplysModel model)
        {
            var order = await orderService.GetModelAsync(model.OrderId);
            if(order==null)
            {
                return new ApiResult { status = 0, msg = "订单不存在" };
            }
            long payResId = await userService.BalancePayAsync(model.OrderId);
            if (payResId == -1)
            {
                return new ApiResult { status = 0, msg = "订单不存在" };
            }
            if (payResId == -2)
            {
                return new ApiResult { status = 0, msg = "用户不存在", data = order.Id };
            }
            if (payResId == -5)
            {
                return new ApiResult { status = 0, msg = "订单中有商品已经下架，请重新下单", data = order.Id };
            }
            if (payResId == -3)
            {
                return new ApiResult { status = 0, msg = "商品库存不足", data = order.Id };
            }
            if (payResId == -4)
            {
                return new ApiResult { status = 0, msg = "用户账户余额不足", data = order.Id };
            }

            return new ApiResult { status = 1, msg = "支付成功", data = order.Id };
        }
        [HttpPost]
        [AllowAnonymous]
        public ApiResult State()
        {
            //var res = await idNameService.GetByTypeNameAsync("订单状态");
            var res = MyEnumHelper.GetEnumList<OrderStateEnum>();
            return new ApiResult { status = 1, data = res };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> Discount()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            bool isDiscount = await userService.CheckIsDiscountAsync(user.Id);
            string res = await settingService.GetParmByNameAsync("复购会员优惠折扣");
            decimal discount;
            if (!isDiscount)
            {
                return new ApiResult { status = 1, data = 1};
            }
            else if(!decimal.TryParse(res,out discount))
            {
                return new ApiResult { status = 1, data = 1 };
            } 
            else
            {
                return new ApiResult { status = 1, data = (discount * 10) / 100 };
            }
        }

        [HttpPost]
        public async Task<ApiResult> Pay(OrderPayModel model)
        {
            int orderStateId = (int)OrderStateEnum.待付款;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var dtos = await orderApplyService.GetModelListAsync(user.Id);
            if (dtos.OrderApplies.Count() <= 0)
            {
                return new ApiResult { status = 0, msg = "下单列表无商品" };
            }
            long id = await orderService.AddAsync(model.DeliveryTypeId, 0, user.Id, model.AddressId, model.PayTypeId, orderStateId, dtos.OrderApplies);
            if (id <= 0)
            {
                if (id == -4)
                {
                    return new ApiResult { status = 0, msg = "订单中有商品已经下架，请重新下单", data = 0 };
                }
                if (id == -6)
                {
                    return new ApiResult { status = 0, msg = "用户账户已经冻结", data = id };
                }
                return new ApiResult { status = 0, msg = "生成订单失败" };
            }
            OrderDTO order = await orderService.GetModelAsync(id);
            await goodsCarService.DeleteListAsync(user.Id);
            await orderApplyService.DeleteListAsync(user.Id);
            int payTypeId1 = (int)PayTypeEnum.微信;

            if (payTypeId1 != model.PayTypeId)
            {
                return new ApiResult { status = 0, msg = "请选择微信支付" };
            }

            WeChatPay weChatPay = new WeChatPay();
            weChatPay.body = "订单支付";
            weChatPay.out_trade_no = order.Code;
            weChatPay.openid = user.Code.Substring(3, 28);
            weChatPay.total_fee = Math.Truncate(order.Amount * 100).ToString();
            string parm = HttpClientHelper.BuildParam(weChatPay);
            string key= System.Configuration.ConfigurationManager.AppSettings["KEY"];
            parm = parm + "&key=" + key;
            string sign = CommonHelper.GetMD5(parm);
            HttpClient httpClient = new HttpClient();
            string xml = HttpClientHelper.ObjSerializeXml(weChatPay, sign);

            //CacheHelper.SetCache("App_Order_Pay" + weChatPay.out_trade_no, sign, DateTime.UtcNow.AddMinutes(2), TimeSpan.Zero);

            string res = await HttpClientHelper.GetResponseByPostXMLAsync(httpClient, xml, "https://api.mch.weixin.qq.com/pay/unifiedorder");
            if (!res.Contains("SUCCESS"))
            {
                return new ApiResult { status = 0, msg = res + "参数:" + parm };
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(res);
            XmlNode Child = xmlDoc.SelectSingleNode("xml/prepay_id");

            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);            
            GetWechat getWeChat = new GetWechat();
            getWeChat.timeStamp= Convert.ToInt64(ts.TotalSeconds).ToString();
            getWeChat.package = "prepay_id" + Child.InnerText;
            parm = HttpClientHelper.BuildParam(getWeChat);
            parm = parm.Replace("prepay_id", "prepay_id=");
            parm = parm + "&key=" + key;
            string paySign = CommonHelper.GetMD5(parm);

            GetWechat1 getWeChat1 = new GetWechat1();
            getWeChat1.appId = getWeChat.appId;
            getWeChat1.nonceStr = getWeChat.nonceStr;
            getWeChat1.package = "prepay_id=" + Child.InnerText;
            getWeChat1.signType = getWeChat.signType;
            getWeChat1.timeStamp = getWeChat.timeStamp;
            getWeChat1.paySign = paySign;
            getWeChat1.orderId = id;

            log.DebugFormat($"微信支付统一下单：时间{DateTime.Now},回调地址：{weChatPay.notify_url}");
            return new ApiResult { status = 1, data = getWeChat1 };
        }

        [HttpPost]
        public async Task<ApiResult> RePay(OrderReApplysModel model)
        {
            //long orderStateId = await idNameService.GetIdByNameAsync("待付款");
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);

            OrderDTO order = await orderService.GetModelAsync(model.OrderId);
            if(order==null)
            {
                return new ApiResult { status = 0, msg = "订单不存在" };
            }
            long id = await orderService.ValidOrder(order.Id);
            if(id==-1)
            {
                return new ApiResult { status = 0, msg = "订单不存在" };
            }
            if (id == -2)
            {
                return new ApiResult { status = 0, msg = "会员不存在", data = order.Id };
            }
            if (id == -4)
            {
                return new ApiResult { status = 0, msg = "订单中有商品已经下架，请重新下单", data = order.Id };
            }
            if (id == -3)
            {
                return new ApiResult { status = 0, msg = "商品库存不足", data = order.Id };
            }
            if (id == -6)
            {
                return new ApiResult { status = 0, msg = "用户账户已经冻结", data = id };
            }
            WeChatPay weChatPay = new WeChatPay();
            weChatPay.body = "订单支付";
            weChatPay.out_trade_no = order.Code;
            weChatPay.openid = user.Code.Substring(3, 28);
            weChatPay.total_fee = Math.Truncate(order.Amount * 100).ToString();
            string parm = HttpClientHelper.BuildParam(weChatPay);
            string key = System.Configuration.ConfigurationManager.AppSettings["KEY"];
            parm = parm + "&key=" + key;
            string sign = CommonHelper.GetMD5(parm);
            HttpClient httpClient = new HttpClient();
            string xml = HttpClientHelper.ObjSerializeXml(weChatPay, sign);

            //CacheHelper.SetCache("App_Order_Pay" + weChatPay.out_trade_no, sign, DateTime.UtcNow.AddMinutes(2), TimeSpan.Zero);

            string res = await HttpClientHelper.GetResponseByPostXMLAsync(httpClient, xml, "https://api.mch.weixin.qq.com/pay/unifiedorder");
            if (!res.Contains("SUCCESS"))
            {
                return new ApiResult { status = 0, msg = res };
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(res);
            XmlNode Child = xmlDoc.SelectSingleNode("xml/prepay_id");

            log.DebugFormat($"待支付订单微信支付统一下单，时间：{DateTime.Now}");

            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            GetWechat getWeChat = new GetWechat();
            getWeChat.timeStamp = Convert.ToInt64(ts.TotalSeconds).ToString();
            getWeChat.package = "prepay_id" + Child.InnerText;
            parm = HttpClientHelper.BuildParam(getWeChat);
            parm = parm.Replace("prepay_id", "prepay_id=");
            parm = parm + "&key=" + key;
            string paySign = CommonHelper.GetMD5(parm);

            GetWechat1 getWeChat1 = new GetWechat1();
            getWeChat1.appId = getWeChat.appId;
            getWeChat1.nonceStr = getWeChat.nonceStr;
            getWeChat1.package = "prepay_id=" + Child.InnerText;
            getWeChat1.signType = getWeChat.signType;
            getWeChat1.timeStamp = getWeChat.timeStamp;
            getWeChat1.paySign = paySign;
            getWeChat1.orderId = order.Id;

            return new ApiResult { status = 1, data = getWeChat1 };
        }

        [HttpPost]
        public async Task<ApiResult> Receipt(OrderReceiptModel model)
        {
            var order = await orderService.GetModelAsync(model.OrderId);
            if(order==null)
            {
                return new ApiResult { status = 0, msg = "订单不存在" };
            }
            int orderStateId = (int)OrderStateEnum.已完成;
            bool flag = await orderService.Receipt(model.OrderId,orderStateId);
            if(!flag)
            {
                return new ApiResult { status = 0, msg = "确认收货失败" };
            }
            return new ApiResult { status = 1, msg = "确认收货成功" };
        }

        [HttpPost]
        public ApiResult DeliveryType()
        {
            OrderDeliveryTypetModel model = new OrderDeliveryTypetModel();
            List<DeliveryType> deliveryTypes = new List<DeliveryType>();
            deliveryTypes.Add(new DeliveryType { id = 1, tpye = "有快递单号" });
            deliveryTypes.Add(new DeliveryType { id = 2, tpye = "无需物流" });
            deliveryTypes.Add(new DeliveryType { id = 3, tpye = "同城自取" });
            model.deliveryTypes = deliveryTypes;
            return new ApiResult { status = 1, data= model };
        }

        [HttpPost]
        public async Task<ApiResult> GetDeliveryCode(OrderDeliveryCodeModel model)
        {
            var order = await orderService.GetModelAsync(model.OrderId);
            if(order==null)
            {
                return new ApiResult { status = 0, msg = "订单不存在" };
            }
            string deliverCode = "";
            string deliverName = "";
            if(!string.IsNullOrEmpty(order.DeliverCode))
            {
                deliverCode = order.DeliverCode;
            }
            if(!string.IsNullOrEmpty(order.DeliverName))
            {
                deliverName = order.DeliverName;
            }
            return new ApiResult { status = 1, data = new { deliverCode = deliverCode, deliverName = deliverName } };
        }

        [HttpPost]
        public async Task<ApiResult> Del(OrderDelModel model)
        {
            bool flag = await orderService.FrontMarkDel(model.Id);
            if(!flag)
            {
                return new ApiResult { status = 0, msg = "订单删除失败" };
            }
            return new ApiResult { status = 1, msg = "订单删除成功" };
        }

        [HttpPost]
        public async Task<ApiResult> Valid(OrderValidModel model)
        {
            if(string.IsNullOrEmpty(model.TradePassword))
            {
                return new ApiResult { status = 0, msg = "交易密码不能为空" };
            }
            long tradePwd;
            if(!long.TryParse(model.TradePassword,out tradePwd))
            {
                return new ApiResult { status = 0, msg = "交易密码必须是六位数字" };
            }
            if(model.TradePassword.Length!=6)
            {
                return new ApiResult { status = 0, msg = "交易密码必须是六位数字" };
            }
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id = await userService.CheckTradePasswordAsync(user.Id, model.TradePassword);
            if (id==-1)
            {
                return new ApiResult { status = 0, msg = "会员不存在" };
            }
            if (id == -2)
            {
                return new ApiResult { status = 0, msg = "交易密码错误" };
            }
            return new ApiResult { status = 1, msg = "交易密码验证成功" };
        }

        [HttpPost]
        public async Task<ApiResult> Cancel(OrderCancelModel model)
        {
            var res = await orderService.OrderCancelAsync(model.OrderId);
            if (!res)
            {
                return new ApiResult { status = 0, msg = "取消订单失败" };
            }
            return new ApiResult { status = 1, msg = "取消订单成功" };
        }

        //[HttpGet]
        //public ApiResult TestWechat(string code)
        //{
        //    long id = userService.WeChatPay(code);
        //    return new ApiResult { status = 1, msg = id.ToString() };
        //}

        public class GetWechat
        {
            public string appId { get; set; } = System.Configuration.ConfigurationManager.AppSettings["APPID"];
            public string timeStamp { get; set; }
            public string nonceStr { get; set; } = CommonHelper.GetCaptcha(10);
            public string package { get; set; }
            public string signType { get; set; } = "MD5";
        }

        public class GetWechat1
        {
            public string appId { get; set; } 
            public string timeStamp { get; set; }
            public string nonceStr { get; set; }
            public string package { get; set; }
            public string signType { get; set; }
            public string paySign { get; set; }
            public long orderId { get; set; }
        }
    }
}