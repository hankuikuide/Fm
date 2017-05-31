namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Han.DbLight.Mapper;

    public interface ITableDao<TDomain>
        where TDomain : class, new()
    {
        #region Public Properties

        //Capability Capability { get; set; }

        string DefaultAlias { get; set; }

        /// <summary>
        /// ��Ĭ��ӳ�����
        /// </summary>
        ColumnMapperStrategy DefaultColumnMapperStrategy { get; set; }

        SqlLog SqlLog { get; }

        /// <summary>
        /// column attribute����ı�ṹ
        /// </summary>
        Table TableInfoFromAttr { get; }

        /// <summary>
        /// ���Զ���ı�ṹ
        /// </summary>
        Table TableInfoFromProperty { get; }

       // string TableName { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="where"></param>
        /// <param name="columnMapperStrategy"> </param>
        /// <returns></returns>
        int Count(Expression<Func<TDomain, bool>> where, ColumnMapperStrategy columnMapperStrategy);

        int Count(Expression<Func<TDomain, bool>> where);

        /// <summary>
        /// ��������count(*)
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// ������count(cols),�ǿ�������
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="expression">��</param>
        /// <returns></returns>
        int Count<TProp>(Expression<Func<TDomain, TProp>> expression);

        /// <summary>
        /// ������count(cols),�ǿ�������
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <param name="columnMapperStrategy"></param>
        /// <returns></returns>
        int Count<TProp>(Expression<Func<TDomain, TProp>> expression, Expression<Func<TDomain, bool>> where, ColumnMapperStrategy columnMapperStrategy);

        /// <summary>
        /// ɾ����������
        /// </summary>
        void Delete();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where">��֧��ʵ�������磺User.Roles.Count==1</param>
        void Delete(Expression<Func<TDomain, bool>> where);

        void Delete(Expression<Func<TDomain, bool>> where, ColumnMapperStrategy columnMapperStrategy);

        void Insert(TDomain domain, ColumnMapperStrategy columnMapperStrategy, List<Expression<Func<TDomain, object>>> cols);

        void Insert(TDomain domain, List<Expression<Func<TDomain, object>>> cols);

        void Insert(TDomain domain, params Expression<Func<TDomain, object>>[] cols);


        /// <summary>
        /// �������߱�������б����������Ϊ�������
        /// </summary>
        /// <param name="domain"> These objects can be POCOs, Anonymous, NameValueCollections, or Expandos. Objects</param>
        /// <param name="columnMapperStrategy"> </param>
        /// <param name="cols"> </param>
        /// <returns></returns>
        void Save(TDomain domain, ColumnMapperStrategy columnMapperStrategy, List<Expression<Func<TDomain, object>>> cols);

        void Save(TDomain domain, List<Expression<Func<TDomain, object>>> cols);

        void Save(TDomain domain, params Expression<Func<TDomain, object>>[] cols);

        void Save(List<TDomain> domains, params Expression<Func<TDomain, object>>[] cols);

        List<TDomain> Select(Expression<Func<TDomain, bool>> where, ColumnMapperStrategy columnMapperStrategy, Expression<Func<TDomain, object>>[] cols);

        List<TDomain> SelectByExp(Expression<Func<TDomain, bool>> where, params Expression<Func<TDomain, object>>[] cols);

        void Update(TDomain domain, ColumnMapperStrategy columnMapperStrategy, List<Expression<Func<TDomain, object>>> cols);

        void Update(TDomain domain, List<Expression<Func<TDomain, object>>> cols);

        void Update(TDomain domain, params Expression<Func<TDomain, object>>[] cols);

        /// <summary>
        /// �������¡�ʹ�������뻺�档�Ż�������
        /// </summary>
        /// <param name="domains"></param>
        /// <param name="cols"></param>
        //       [System.Runtime.CompilerServices.MethodImpl(
        //System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        //void Update(List<TDomain> domains, params Expression<Func<TDomain, object>>[] cols);

        #endregion
    }
}