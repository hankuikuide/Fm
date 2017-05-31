
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Han.Infrastructure
{
    public class ConfigHelper
    {
        private static object obj = new object();
        private static ConfigHelper instance = null;

        private const string Const_Key_Delimiter = ":";

        private Stack<string> stack = new Stack<string>();
        private Dictionary<string, object> Configuration = new Dictionary<string, object>();

        public static ConfigHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new ConfigHelper();
                        }
                    }
                }
                return instance;
            }
        }

        public ConfigHelper()
        {
            this.Load("config.json");
            this.Load("configChange.json");
        }

        public ConfigHelper(string[] files)
        {
            foreach (var item in files)
            {
                this.Load(item);
            }
        }

        public string Get(string key)
        {
            try
            {
                return Configuration[key].ToString();
            }
            catch
            {
                return null;
            }
        }

        public Dictionary<string, string> GetSection(string key)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var item in Configuration)
            {
                if (item.Key.Contains(key))
                {
                    result.Add(item.Key.Replace(key + ":", ""), item.Value.ToString());

                }
            }

            return result;
        }

        public void Load(string fileName)
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName)))
            {
                var content = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));

                // 过滤单行注释 
                var result = Regex.Replace(content, @"\s//(.*)", string.Empty);

                //实例化JavaScriptSerializer类的新实例
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                //将指定的 JSON 字符串转换为 Dictionary<string, object> 类型的对象
                var dict = serializer.Deserialize<Dictionary<string, object>>(result);

                addConfiguration(dict);
            }
        }

        public void addConfiguration(Dictionary<string, object> dict)
        {
            foreach (var item in dict)
            {
                Type type = item.Value.GetType();
                //key值入栈
                stack.Push(item.Key);

                if (type.Name.Contains("Dictionary"))
                {
                    addConfiguration((Dictionary<string, Object>)item.Value);
                }
                else if (type.Name.Contains("ArrayList"))
                {
                    ArrayList conns = item.Value as ArrayList;

                    for (int index = 0; index < conns.Count; index++)
                    {
                        Dictionary<string, Object> con = (Dictionary<string, Object>)conns[index];

                        addConfiguration(con);
                    }
                }
                else
                {
                    var currentPath = string.Join(Const_Key_Delimiter, stack.Reverse());

                    // 如果key重复，则覆盖则值
                    if (Configuration.Keys.Contains<string>(currentPath))
                    {
                        Configuration[currentPath] = item.Value.ToString();
                    }
                    else
                    {
                        Configuration.Add(currentPath, item.Value.ToString());
                    }
                }

                //key值出栈
                stack.Pop();
            }
        }

        public string GetValue(string key)
        {
            lock (Configuration)
            {
                return Configuration[key].ToString();
            }
        }

        public int GetInt(string key)
        {
            lock (Configuration)
            {
                return int.Parse(Configuration[key].ToString());
            }
        }

        public bool GetBool(string key)
        {
            lock (Configuration)
            {
                return bool.Parse(Configuration[key].ToString());
            }
        }

        public decimal GetDecimal(string key)
        {
            lock (Configuration)
            {
                return decimal.Parse(Configuration[key].ToString());
            }
        }

        public double GetDouble(string key)
        {
            lock (Configuration)
            {
                return Convert.ToDouble(Configuration[key]);
            }
        }

        public DateTime GetDate(string key)
        {
            lock (Configuration)
            {
                return DateTime.Parse(Configuration[key].ToString());
            }
        }
    }
}