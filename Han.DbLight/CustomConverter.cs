
//namespace Han.DbLight
//{
//    using System;

//    using Han.Cache;

//    /// <summary>
//    /// 对象值与数据库值转换，每个类一个
//    /// </summary>
//    public class  CustomConverter
//    {
//        private LruMemoryCache<Func<object,string>> toDbStringConverterCache=new LruMemoryCache<Func<object,string>>();
//        private LruMemoryCache<Func<object,object>> fromDbValueConverterCache = new LruMemoryCache<Func<object,object>>();
//        /// <summary>
//        /// 增加对象值到数据库值的转换器
//        /// </summary>
//        /// <param name="key">属性名称</param>
//        /// <param name="converter">对象属性值转换为数据库值，结果为拼sql用到的字符串</param>
//        public void AddToDbStringConverter(string key, Func<object,string> converter)
//        {
//            this.toDbStringConverterCache.Add(key, converter);
//        }
//        public Func<object,string> GetToDbValueConverter(string key)
//        {
//            return this.toDbStringConverterCache.Get(key);
//        }
//        /// <summary>
//        /// 增加数据库值到对象值的转换器
//        /// </summary>
//        /// <param name="col">列名称</param>
//        /// <param name="converter">数据库值到对象值的转换器，用于数据库结果到对象属性的自定义映射</param>
//        public void AddFromDbValueConverter(string col, Func<object,object> converter)
//        {
//            this.fromDbValueConverterCache.Add(col, converter);
//        }
//        public Func<object,object> GetFromDbValueConverter(string col)
//        {
//            return this.fromDbValueConverterCache.Get(col);
//        }
//    }
//}