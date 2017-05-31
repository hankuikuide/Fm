using System;
using System.Collections.Generic;
using Han.Infrastructure;

namespace Han.Infrastructure
{
    /// <summary>
    /// 系统配置帮助
    /// </summary>
    public sealed class BaseConfigHelper
    {
        private static readonly LogHelper logHelper = new LogHelper(typeof(BaseConfigHelper));
        private static BaseConfigHelper _instance = null;
        private static readonly object _obj = new object();

        /// <summary>
        /// 单例,通过它访问系统配置
        /// </summary>
        public static BaseConfigHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_obj)
                    {
                        if (_instance == null)
                        {
                            _instance = new BaseConfigHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 系统核心配置信息
        /// </summary>
        private static Dictionary<string, object> ParamConfigs = new Dictionary<string, object>();

        /// <summary>
        /// 更新核心配置信息
        /// </summary>
        /// <param name="configs"></param>
        public static void RefreshSysConfig(Dictionary<string, object> configs)
        {
            ParamConfigs = configs;
        }
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="key">配置Key</param>
        /// <returns></returns>
        public string GetValue(string key)
        {

            try
            {
                return ParamConfigs[key]?.ToString();
            }
            catch (Exception ex)
            {
                logHelper.Error(key + "字典值未找到");

                throw ex;
            }

            //lock (ParamConfigs)
            //{
            //    return ParamConfigs[key].Value;
            //}
        }

        public int GetInt(string key)
        {
            lock (ParamConfigs)
            {
                return int.Parse(this.GetValue(key));
                //return int.Parse(ParamConfigs[key].Value);.
            }
        }

        public bool GetBool(string key)
        {
            lock (ParamConfigs)
            {
                return bool.Parse(this.GetValue(key));
            }
        }

        public decimal GetDecimal(string key)
        {
            lock (ParamConfigs)
            {
                return decimal.Parse(this.GetValue(key));
            }
        }

        public double GetDouble(string key)
        {
            lock (ParamConfigs)
            {
                return Convert.ToDouble(this.GetValue(key));
            }
        }

        public DateTime GetDate(string key)
        {
            lock (ParamConfigs)
            {
                return DateTime.Parse(this.GetValue(key));
            }
        }
    }
}
