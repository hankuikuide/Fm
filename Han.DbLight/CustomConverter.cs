
//namespace Han.DbLight
//{
//    using System;

//    using Han.Cache;

//    /// <summary>
//    /// ����ֵ�����ݿ�ֵת����ÿ����һ��
//    /// </summary>
//    public class  CustomConverter
//    {
//        private LruMemoryCache<Func<object,string>> toDbStringConverterCache=new LruMemoryCache<Func<object,string>>();
//        private LruMemoryCache<Func<object,object>> fromDbValueConverterCache = new LruMemoryCache<Func<object,object>>();
//        /// <summary>
//        /// ���Ӷ���ֵ�����ݿ�ֵ��ת����
//        /// </summary>
//        /// <param name="key">��������</param>
//        /// <param name="converter">��������ֵת��Ϊ���ݿ�ֵ�����Ϊƴsql�õ����ַ���</param>
//        public void AddToDbStringConverter(string key, Func<object,string> converter)
//        {
//            this.toDbStringConverterCache.Add(key, converter);
//        }
//        public Func<object,string> GetToDbValueConverter(string key)
//        {
//            return this.toDbStringConverterCache.Get(key);
//        }
//        /// <summary>
//        /// �������ݿ�ֵ������ֵ��ת����
//        /// </summary>
//        /// <param name="col">������</param>
//        /// <param name="converter">���ݿ�ֵ������ֵ��ת�������������ݿ������������Ե��Զ���ӳ��</param>
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