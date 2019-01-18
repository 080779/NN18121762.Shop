using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service
{
    public static class EFCoreExtensions
    {
        #region 根据参数表名获取参数值
        /// <summary>
        /// 根据参数表名获取参数值异步扩展方法,返回string类型
        /// </summary>
        /// <param name="dbc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async static Task<string> GetStringParamAsync(this MyDbContext dbc, string name)
        {
            return await dbc.GetStringPropertyAsync<SettingEntity>(s => s.Name == name, s => s.Param);
        }

        /// <summary>
        /// 根据参数表名获取参数值扩展方法,返回string类型
        /// </summary>
        /// <param name="dbc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetStringParam(this MyDbContext dbc, string name)
        {
            return dbc.GetStringProperty<SettingEntity>(s => s.Name == name, s => s.Param);
        }

        /// <summary>
        /// 根据参数表名获取参数值异步扩展方法,返回decimal类型
        /// </summary>
        /// <param name="dbc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async static Task<decimal> GetDecimalParamAsync(this MyDbContext dbc, string name)
        {
            decimal param;
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.Name == name, s => s.Param), out param);
            return param;
        }

        /// <summary>
        /// 根据参数表名获取参数值扩展方法,返回decimal类型
        /// </summary>
        /// <param name="dbc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static decimal GetDecimalParam(this MyDbContext dbc, string name)
        {
            decimal param;
            decimal.TryParse(dbc.GetStringProperty<SettingEntity>(s => s.Name == name, s => s.Param), out param);
            return param;
        }

        /// <summary>
        /// 根据参数表名获取参数值异步扩展方法,返回int类型
        /// </summary>
        /// <param name="dbc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async static Task<int> GetIntParamAsync(this MyDbContext dbc, string name)
        {
            int param;
            int.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.Name == name, s => s.Param), out param);
            return param;
        }

        /// <summary>
        /// 根据参数表名获取参数值扩展方法,返回int类型
        /// </summary>
        /// <param name="dbc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetIntParam(this MyDbContext dbc, string name)
        {
            int param;
            int.TryParse(dbc.GetStringProperty<SettingEntity>(s => s.Name == name, s => s.Param), out param);
            return param;
        }
        #endregion
    }
}
