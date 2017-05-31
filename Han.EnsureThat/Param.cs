#region copyright
// <copyright file="Param.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>¶¡ºÆ</author>
// <datecreated>2012-11-07</datecreated>
#endregion
namespace Han.EnsureThat
{
    public abstract class Param
    {
        #region Constants

        public const string DefaultName = "";

        #endregion

        #region Fields

        public readonly string Name;

        #endregion

        #region Constructors and Destructors

        protected Param(string name)
        {
            this.Name = name;
        }

        #endregion
    }

    public class Param<T> : Param
    {
        #region Fields

        public readonly T Value;

        #endregion

        #region Constructors and Destructors

        internal Param(string name, T value)
            : base(name)
        {
            this.Value = value;
        }

        #endregion
    }
}