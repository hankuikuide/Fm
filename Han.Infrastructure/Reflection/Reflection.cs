#region copyright
// <copyright file="Reflection.cs" company="ehong"> 
// Copyright (c) ehong. All Right Reserved
// </copyright>
// <author>丁浩</author>
// <datecreated>2012-11-20</datecreated>
#endregion
namespace Han.Infrastructure.Reflection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Han.Cache;
    using Han.EnsureThat;
    using Han.Infrastructure.Extensions;
    using Han.Log;

    internal delegate void GenericSetter(object target, object value);

    internal delegate object GenericGetter(object target);

    internal delegate object FastCreateInstanceHandler();

    internal delegate object FastInvokeHandler(object target, object[] parameters);
    /// <summary>
    /// 提供类型快速访问
    /// </summary>
    public static class Reflection
    {
        #region Static Fields

      
        private static readonly ICacheProvider<FastInvokeHandler> methodCache =
           new LruMemoryCache<FastInvokeHandler>();

        /// <summary>
        ///属性访问器的缓存  value 为 GenericSetter,GenericGetter
        /// </summary>
       private static readonly ICacheProvider<object> propertyAccessorCache =
           new LruMemoryCache<object>();
        /// <summary>
        /// 类型的属性缓存，key 为类型名称 GetPropertys之类方法很费时间
        /// </summary>
       
        private static readonly ICacheProvider<List<PropertyInfo>> typePropertyCache =
           new LruMemoryCache<List<PropertyInfo>>();
        /// <summary>
        /// 属性缓存，可以为属性名称。GetProperty之类方法很费时间
        /// </summary>
        private static readonly ICacheProvider<PropertyInfo> propertyCache =
           new LruMemoryCache<PropertyInfo>();

        private static readonly ICacheProvider<FastCreateInstanceHandler> typeCache =
            new LruMemoryCache<FastCreateInstanceHandler>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">调用方法的对象，静态方法为null</param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object CallMethod(object obj, string methodName, object[] parameters)
        {
            Type type = obj.GetType();
            string key = type.FullName + "$" + "method" + methodName;
            var method= methodCache.GetOrAdd(key, () => GetMethodInvoker(type, methodName));
            return method(obj, parameters);
           
        }
        private static object ChangeType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }
                throw new NotSupportedException();
                //System.ComponentModel.NullableConverter nullableConverter
                //    = new System.ComponentModel.NullableConverter(conversionType);

                // conversionType = nullableConverter.UnderlyingType;
            }
            if (conversionType.GetInterface("IConvertible", true) != null)
            {
                return Convert.ChangeType(value, conversionType, null);
            }
            return value;
            //return Convert.ChangeType(value, conversionType,null);
        }
        /// <summary>
        /// silverlight可用
        /// </summary>
        /// <param name="type">要转换到的类型</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ChangeType2(Type type, object value)
        {
            if ((value == null) && type.IsGenericType)
            {
                return Activator.CreateInstance(type);
            }
            if (value == null)
            {
                return null;
            }
            if (type == value.GetType())
            {
                return value;
            }
            if (type.IsEnum)
            {
                string s = Convert.ToString(value);
                return Enum.Parse(type, s, true);
                //if (value is string)
                //{
                //    return Enum.Parse(type, value as string, true);
                //}
                //return Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type type1 = type.GetGenericArguments()[0];
                object obj1 = ChangeType2(type1, value);
                return Activator.CreateInstance(type, new[] { obj1 });
            }
            if ((value is string) && (type == typeof(Guid)))
            {
                return new Guid(value as string);
            }
            if ((value is string) && (type == typeof(Version)))
            {
                return new Version(value as string);
            }
            if ((value is string) && (type == typeof(bool)))
            {
                if (value.ToString() == "1")
                {
                    return true;
                }
                else if (value.ToString() == "0")
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(value);
                }
                
            }
            if (!(value is IConvertible))
            {
                return value;
            }

            return Convert.ChangeType(value, type, null);
        }

        public static object CreateInstance(string name)
        {
            Type t = Type.GetType(name);
            return CreateInstance(t);
        }

        /// <summary>
        /// 必须有默认构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            try
            {
                var typeFac= typeCache.GetOrAdd(type.FullName,()=> GetInstanceCreator(type));
                return typeFac();
               
            }
            catch (Exception ex)
            {
                
                Logger.Log(Level.Error, ex.StackTrace+ ex.Message+ "通过反射创建对象异常");
                return null;
            }
          
        }
        /// <summary>
        /// 不使用缓存创建对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstanceNoCache(Type type)
        {
            try
            {
                var typeFac= GetInstanceCreator(type);
                return typeFac();

            }
            catch (Exception ex)
            {

                Logger.Log(Level.Error,  "通过反射创建对象异常,不能创建："+type.FullName+ex.Message);
                return null;
            }

        }
        public static object GetPropertyValue(object obj, string propertyName)
        {
            Type type = obj.GetType();
            string key = type.FullName + "$" + "get" + propertyName;
            var properyAccessor= propertyAccessorCache.GetOrAdd(key, () => CreateGetMethod(type, propertyName));
            return ((GenericGetter)properyAccessor)(obj);
           
        }
        public static bool IsContainProperty(object obj, string propertyName)
        {
            Type type = obj.GetType();
            string key = type.FullName + "$" +  propertyName;
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            return propertyInfo != null;
        }
        public static bool ImplementsGenericDefinition(
            Type type, Type genericInterfaceDefinition, out Type implementingType)
        {
            //ValidationUtils.ArgumentNotNull(type, "type");
            //ValidationUtils.ArgumentNotNull(genericInterfaceDefinition, "genericInterfaceDefinition");

            if (!genericInterfaceDefinition.IsInterface || !genericInterfaceDefinition.IsGenericTypeDefinition)
            {
                throw new ArgumentNullException();
            }

            if (type.IsInterface)
            {
                if (type.IsGenericType)
                {
                    Type interfaceDefinition = type.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = type;
                        return true;
                    }
                }
            }

            foreach (Type i in type.GetInterfaces())
            {
                if (i.IsGenericType)
                {
                    Type interfaceDefinition = i.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = i;
                        return true;
                    }
                }
            }

            implementingType = null;
            return false;
        }

        /// <summary>
        /// <![CDATA[IList<CustomType>]]>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsComplexGenericList(Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    Type itemType = type.GetGenericArguments()[0];
                    return !itemType.IsPrimitive();
                    //return !IsPrimitive(itemType);
                }
            }
            return false;
        }

        //public static bool IsPrimitive(Type t)
        //{
        //    // TODO: put any type here that you consider as primitive as I didn't
        //    // quite understand what your definition of primitive type is
        //    return
        //        new[]
        //            {
        //                typeof(string), typeof(char), typeof(byte), typeof(ushort), typeof(short), typeof(uint),
        //                typeof(int), typeof(ulong), typeof(long), typeof(float), typeof(double), typeof(decimal),
        //                typeof(DateTime), typeof(Guid), typeof(char?), typeof(byte?), typeof(ushort?), typeof(short?),
        //                typeof(uint?), typeof(int?), typeof(ulong?), typeof(long?), typeof(float?), typeof(double?),
        //                typeof(decimal?), typeof(DateTime?), typeof(Guid?)
        //            }.Contains(t);
        //}

        /// <summary>
        /// <![CDATA[List<primitive>]]>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitiveGenericList(Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    Type itemType = type.GetGenericArguments()[0];
                    return itemType.IsPrimitive();
                    //return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 返回属性，并加入缓存。属性为空也被缓存。值为null
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static PropertyInfo GetCachedProperty(Type type,string propertyName)
        {
            string pKey = type.FullName + "$" + propertyName;
            return propertyCache.GetOrAdd(pKey, () => type.GetProperty(propertyName));
           
        }
        /// <summary>
        /// 获取属性并缓存
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPropertys(Type type)
        {
            string key = type.FullName;
            return typePropertyCache.GetOrAdd(key, () => type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList());
           
        }
        public static void SetPropertyValue(object obj, object value, string propertyName)
        {
            try
            {
                Type type = obj.GetType();
                string key = type.FullName + "$" + "set" + propertyName;
                var pi = GetCachedProperty(type, propertyName);
                if (pi == null)
                    throw new ArgumentException();
                var ptype = pi.PropertyType;
                var properySetter = propertyAccessorCache.GetOrAdd(key, () => CreateSetMethod(type, propertyName));
                object o = ChangeType2(ptype, value);
                ((GenericSetter)properySetter)(obj, o);
            }
            catch (Exception ex)
            {
                string msgValue = "";
                if (value == null)
                {
                    msgValue = "";
                }
                else
                {
                    msgValue = value.ToString();
                }

                string message = ex.Message + msgValue + "   " + propertyName;
                #if DEBUG
                throw new Exception(message); 
                #endif
                #if Release
                 Logger.Log(Level.Error, string.Format("反射赋值错误，值{0}.{1}",value,message));
                #endif

            }

        }
        /// <summary>
        /// 设置属性A.B.C的值，如果B为空，创建B，然后赋值为C
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <param name="propertyPath"></param>
        /// <returns>设置属性值失败返回false</returns>
        public static bool SetPropertyValueByPath(object obj, object value, string propertyPath)
        {
          
            Ensure.That(obj).IsNotNull();
            Ensure.That(propertyPath).IsNotNullOrEmpty();
            string[] splitter = { "." };
            string[] sourceProperties = propertyPath.Split(splitter, StringSplitOptions.None);
            object tempSourceObj = obj;
            Type tempPropertyType = obj.GetType();
            for (int x = 0; x < sourceProperties.Length; ++x)
            {

                PropertyInfo sourcePropertyInfo = GetCachedProperty(tempPropertyType, sourceProperties[x]);
                if (sourcePropertyInfo == null)
                {
                    Logger.Log(Level.Trace, string.Format("{0}不存在属性：{1} 属性路径：{2}", tempPropertyType.Name,sourceProperties[x], propertyPath));
                    return false;
                    //
                }
                else
                {
                    var subSourceObj = GetPropertyValue(tempSourceObj, sourceProperties[x]);
                    bool isPrimitive = sourcePropertyInfo.PropertyType.IsPrimitive();
                    if(isPrimitive)
                    {
                        //Primitive已经到最后
                        SetPropertyValue(tempSourceObj, value, sourceProperties[x]);
                        return true;
                    }
                    if (subSourceObj == null)
                    {

                        if (x == sourceProperties.Length - 1)
                        {
                            //已经到最后赋值
                            SetPropertyValue(tempSourceObj, value, sourceProperties[x]);
                            return true;
                        }
                        else
                        {
                            subSourceObj = CreateInstance(sourcePropertyInfo.PropertyType);
                            SetPropertyValue(tempSourceObj, subSourceObj, sourceProperties[x]);
                        }
                    }
                   
                    tempSourceObj = subSourceObj;
                }

                tempPropertyType = sourcePropertyInfo.PropertyType;
            }
            return true;
        }
        /// <summary>
        /// 如果路径上某个非最终路径为空，抛出异常
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        public static object GetPropertyValueByPath(object obj, string propertyPath)
        {

            Ensure.That(obj).IsNotNull();
            Ensure.That(propertyPath).IsNotNullOrEmpty();
            string[] splitter = { "." };
            string[] sourceProperties = propertyPath.Split(splitter, StringSplitOptions.None);
            object tempSourceObj = obj;
            Type tempPropertyType = obj.GetType();
            for (int x = 0; x < sourceProperties.Length; ++x)
            {
                //if (tempSourceObj == null) return tempSourceObj;
                tempSourceObj = GetPropertyValue(tempSourceObj, sourceProperties[x]);
            }
            return tempSourceObj;
        }
        /// <summary>
        /// Gets the type of the typed collection's items.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type of the typed collection's items.</returns>
        public static bool TryGetCollectionItemType(Type type, out Type itemType)
        {
            itemType = null;
            Type genericListType;
            if (type.IsArray)
            {
                itemType = type.GetElementType();
                return true;
            }
            else if (ImplementsGenericDefinition(type, typeof(IEnumerable<>), out genericListType))
            {
                if (genericListType.IsGenericTypeDefinition)
                {
                    return false;
                }
                else
                {
                    itemType = genericListType.GetGenericArguments()[0];
                    return true;
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                itemType = null;
                throw new Exception();
            }
            else
            {
                itemType = null;
                return false;
            }
        }

        #endregion

        #region Methods

       

        ///
        /// Creates a dynamic getter for the property
        ///
        private static GenericGetter CreateGetMethod(Type type, string propertyName)
        {
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(propertyName);
            }
            /*
            * If there's no getter return null
            */
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
            {
                return null;
            }

            /*
            * Create the dynamic method
            */
            var arguments = new Type[1];
            arguments[0] = typeof(object);

            var getter = new DynamicMethod(String.Concat("_Get", propertyInfo.Name, "_"), typeof(object), arguments);
            ILGenerator generator = getter.GetILGenerator();
            generator.DeclareLocal(typeof(object));
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
            generator.EmitCall(OpCodes.Callvirt, getMethod, null);

            if (!propertyInfo.PropertyType.IsClass)
            {
                generator.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            generator.Emit(OpCodes.Ret);

            /*
            * Create the delegate and return it
            */
            return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
        }

        ///
        /// Creates a dynamic setter for the property
        ///修改为 silverlight 可用
        ///
        private static GenericSetter CreateSetMethod(Type type, string propertyName)
        {
            PropertyInfo propertyInfo = type.GetProperty(propertyName);

            /*
            * If there's no setter return null
            */
            MethodInfo setMethod = propertyInfo.GetSetMethod();
            if (setMethod == null)
            {
                return null;
            }

            /*
            * Create the dynamic method
            */
            var arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);
            
            var setter = new DynamicMethod(String.Concat("_Set", propertyInfo.Name, "_"), typeof(void), arguments);
            ILGenerator generator = setter.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
            generator.Emit(OpCodes.Ldarg_1);

            if (propertyInfo.PropertyType.IsClass)
            {
                generator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
            }
            else
            {
                generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
            }

            generator.EmitCall(OpCodes.Callvirt, setMethod, null);
            generator.Emit(OpCodes.Ret);

            /*
            * Create the delegate and return it
            */
            return (GenericSetter)setter.CreateDelegate(typeof(GenericSetter));
        }

        /// <summary>Boxes a type if needed.</summary>
        /// <param name="ilGenerator">The MSIL generator.</param>
        /// <param name="type">The type.</param>
        private static void EmitBoxIfNeeded(ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, type);
            }
        }

        /// <summary>Emits the cast to a reference, unboxing if needed.</summary>
        /// <param name="il">The MSIL generator.</param>
        /// <param name="type">The type to cast.</param>
        private static void EmitCastToReference(ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>Emits code to save an integer to the evaluation stack.</summary>
        /// <param name="ilGeneartor">The MSIL generator.</param>
        /// <param name="value">The value to push.</param>
        private static void EmitFastInt(ILGenerator ilGenerator, int value)
        {
            // for small integers, emit the proper opcode
            switch (value)
            {
                case -1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    ilGenerator.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    ilGenerator.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    ilGenerator.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    ilGenerator.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    ilGenerator.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            // for bigger values emit the short or long opcode
            if (value > -129 && value < 128)
            {
                ilGenerator.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldc_I4, value);
            }
        }

        private static FastCreateInstanceHandler GetInstanceCreator(Type type)
        {
            // generates a dynamic method to generate a FastCreateInstanceHandler delegate
            var dynamicMethod = new DynamicMethod(string.Empty, type, new Type[0]);

            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

            // generates code to create a new object of the specified type using the default constructor
            ilGenerator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));

            // returns the value to the caller
            ilGenerator.Emit(OpCodes.Ret);

            // converts the DynamicMethod to a FastCreateInstanceHandler delegate to create the object
            var creator = (FastCreateInstanceHandler)dynamicMethod.CreateDelegate(typeof(FastCreateInstanceHandler));

            return creator;
        }

        private static FastInvokeHandler GetMethodInvoker(Type type, string methodName)
        {
            MethodInfo methodInfo = type.GetMethod(methodName);
            // generates a dynamic method to generate a FastInvokeHandler delegate
            var dynamicMethod = new DynamicMethod(
                string.Empty, typeof(object), new[] { typeof(object), typeof(object[]) });

            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

            ParameterInfo[] parameters = methodInfo.GetParameters();

            var paramTypes = new Type[parameters.Length];

            // copies the parameter types to an array
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                {
                    paramTypes[i] = parameters[i].ParameterType.GetElementType();
                }
                else
                {
                    paramTypes[i] = parameters[i].ParameterType;
                }
            }

            var locals = new LocalBuilder[paramTypes.Length];

            // generates a local variable for each parameter
            for (int i = 0; i < paramTypes.Length; i++)
            {
                locals[i] = ilGenerator.DeclareLocal(paramTypes[i], true);
            }

            // creates code to copy the parameters to the local variables
            for (int i = 0; i < paramTypes.Length; i++)
            {
                ilGenerator.Emit(OpCodes.Ldarg_1);
                EmitFastInt(ilGenerator, i);
                ilGenerator.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(ilGenerator, paramTypes[i]);
                ilGenerator.Emit(OpCodes.Stloc, locals[i]);
            }

            if (!methodInfo.IsStatic)
            {
                // loads the object into the stack
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Castclass, methodInfo.DeclaringType);
            }

            // loads the parameters copied to the local variables into the stack
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                {
                    ilGenerator.Emit(OpCodes.Ldloca_S, locals[i]);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldloc, locals[i]);
                }
            }

            // calls the method
            if (!methodInfo.IsStatic)
            {
                ilGenerator.EmitCall(OpCodes.Callvirt, methodInfo, null);
            }
            else
            {
                ilGenerator.EmitCall(OpCodes.Call, methodInfo, null);
            }

            // creates code for handling the return value
            if (methodInfo.ReturnType == typeof(void))
            {
                ilGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                EmitBoxIfNeeded(ilGenerator, methodInfo.ReturnType);
            }

            // iterates through the parameters updating the parameters passed by ref
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(ilGenerator, i);
                    ilGenerator.Emit(OpCodes.Ldloc, locals[i]);
                    if (locals[i].LocalType.IsValueType)
                    {
                        ilGenerator.Emit(OpCodes.Box, locals[i].LocalType);
                    }
                    ilGenerator.Emit(OpCodes.Stelem_Ref);
                }
            }

            // returns the value to the caller
            ilGenerator.Emit(OpCodes.Ret);

            // converts the DynamicMethod to a FastInvokeHandler delegate to call to the method
            var invoker = (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));

            return invoker;
        }

        #endregion
    }
}