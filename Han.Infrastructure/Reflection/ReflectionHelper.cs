#region copyright
// <copyright file="ReflectionHelper.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-11-20</datecreated>
#endregion
namespace Han.Infrastructure.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Utility class that handles various
    /// functions dealing with reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Determine if a property exists in an object
        /// </summary>
        /// <param name="propertyName">Name of the property </param>
        /// <param name="srcObject">the object to inspect</param>
        /// <returns>true if the property exists, false otherwise</returns>
        /// <exception cref="ArgumentNullException">if srcObject is null</exception>
        /// <exception cref="ArgumentException">if propertName is empty or null </exception>
        /// --------------------------------------------------------------------
        public static bool Exists(string propertyName, object srcObject)
        {
            if (srcObject == null)
            {
                throw new ArgumentNullException("srcObject");
            }

            if ((propertyName == null) || (propertyName == String.Empty) || (propertyName.Length == 0))
            {
                throw new ArgumentException("Property name cannot be empty or null.");
            }

            PropertyInfo propInfoSrcObj = srcObject.GetType().GetProperty(propertyName);

            return (propInfoSrcObj != null);
        }

        public static List<PropertyInfo> GetProperties<T>(BindingFlags bindingAttr)
        {
            return GetProperties(typeof(T), bindingAttr);
        }

        public static List<PropertyInfo> GetProperties(Type type, BindingFlags bindingAttr)
        {
            var targetMembers = new List<PropertyInfo>(type.GetProperties(bindingAttr));
            return targetMembers;
        }

        public static PropertyInfo GetProperty<Source>(string PropertyPath)
        {
            try
            {
                if (string.IsNullOrEmpty(PropertyPath))
                {
                    return null;
                }
                string[] Splitter = { "." };
                string[] SourceProperties = PropertyPath.Split(Splitter, StringSplitOptions.None);
                Type PropertyType = typeof(Source);
                PropertyInfo PropertyInfo = PropertyType.GetProperty(SourceProperties[0]);
                PropertyType = PropertyInfo.PropertyType;
                for (int x = 1; x < SourceProperties.Length; ++x)
                {
                    PropertyInfo = PropertyType.GetProperty(SourceProperties[x]);
                    PropertyType = PropertyInfo.PropertyType;
                }
                return PropertyInfo;
            }
            catch
            {
                throw;
            }
        }

        public static PropertyInfo GetProperty(Type objType, string PropertyPath)
        {
            try
            {
                if (string.IsNullOrEmpty(PropertyPath))
                {
                    return null;
                }
                string[] Splitter = { "." };
                string[] SourceProperties = PropertyPath.Split(Splitter, StringSplitOptions.None);
                PropertyInfo PropertyInfo = objType.GetProperty(SourceProperties[0]);
                objType = PropertyInfo.PropertyType;
                for (int x = 1; x < SourceProperties.Length; ++x)
                {
                    PropertyInfo = objType.GetProperty(SourceProperties[x]);
                    objType = PropertyInfo.PropertyType;
                }
                return PropertyInfo;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a property's parent object
        /// </summary>
        /// <param name="SourceObject">Source object</param>
        /// <param name="PropertyPath">Path of the property (ex: Prop1.Prop2.Prop3 would be
        /// the Prop1 of the source object, which then has a Prop2 on it, which in turn
        /// has a Prop3 on it.)</param>
        /// <param name="PropertyInfo">Property info that is sent back</param>
        /// <returns>The property's parent object</returns>
        public static object GetPropertyParent(object SourceObject, string PropertyPath, out PropertyInfo PropertyInfo)
        {
            try
            {
                if (SourceObject == null)
                {
                    PropertyInfo = null;
                    return null;
                }
                string[] Splitter = { "." };
                string[] SourceProperties = PropertyPath.Split(Splitter, StringSplitOptions.None);
                object TempSourceProperty = SourceObject;
                Type PropertyType = SourceObject.GetType();
                PropertyInfo = PropertyType.GetProperty(SourceProperties[0]);
                PropertyType = PropertyInfo.PropertyType;
                for (int x = 1; x < SourceProperties.Length; ++x)
                {
                    if (TempSourceProperty != null)
                    {
                        TempSourceProperty = PropertyInfo.GetValue(TempSourceProperty, null);
                    }
                    PropertyInfo = PropertyType.GetProperty(SourceProperties[x]);
                    PropertyType = PropertyInfo.PropertyType;
                }
                return TempSourceProperty;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a property's value
        /// </summary>
        /// <param name="SourceObject">object who contains the property</param>
        /// <param name="PropertyPath">Path of the property (ex: Prop1.Prop2.Prop3 would be
        /// the Prop1 of the source object, which then has a Prop2 on it, which in turn
        /// has a Prop3 on it.)</param>
        /// <returns>The value contained in the property or null if the property can not
        /// be reached</returns>
        public static object GetPropertyValue(object SourceObject, string PropertyPath)
        {
            try
            {
                if (SourceObject == null || string.IsNullOrEmpty(PropertyPath))
                {
                    return null;
                }
                string[] Splitter = { "." };
                string[] SourceProperties = PropertyPath.Split(Splitter, StringSplitOptions.None);
                object TempSourceProperty = SourceObject;
                Type PropertyType = SourceObject.GetType();
                for (int x = 0; x < SourceProperties.Length; ++x)
                {
                    PropertyInfo SourcePropertyInfo = PropertyType.GetProperty(SourceProperties[x]);
                    if (SourcePropertyInfo == null)
                    {
                        return null;
                    }
                    TempSourceProperty = SourcePropertyInfo.GetValue(TempSourceProperty, null);
                    if (TempSourceProperty == null)
                    {
                        return null;
                    }
                    PropertyType = SourcePropertyInfo.PropertyType;
                }
                return TempSourceProperty;
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}