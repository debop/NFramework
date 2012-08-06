using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// <see cref="Type"/>과 관련된 Utility Class.
    /// </summary>
    public static class TypeTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static HashSet<string> NumericTypes = new HashSet<string>
                                                     {
                                                         "Byte",
                                                         "Int16",
                                                         "Int32",
                                                         "Int64",
                                                         "SByte",
                                                         "UInt16",
                                                         "UInt32",
                                                         "UInt64",
                                                         "Decimal",
                                                         "Double",
                                                         "Single"
                                                     };

        /// <summary>
        /// 지정된 객체의 수형을 반환한다. 객체가 null이면, null을 반환한다.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetObjectType(this object obj) {
            return (obj != null) ? obj.GetType() : null;
        }

        /// <summary>
        /// 인스턴스의 속성 정보를 "속성 명=속성 값" 형태로 만들어 반환합니다.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetProperties(object obj) {
            if(obj == null)
                return string.Empty;

            var result = new StringBuilder();

            GetProperties(obj, result);

            return result.ToString();
        }

        /// <summary>
        /// 인스턴스의 속성 정보를 "속성 명=속성 값" 형태로 만들어 반환합니다.
        /// </summary>
        /// <param name="obj">대상 인스턴스</param>
        /// <param name="result">속성정보를 담을 객체</param>
        public static void GetProperties(object obj, StringBuilder result) {
            result.ShouldNotBeNull("result");

            if(obj == null)
                return;

            var type = obj.GetType();
            var pinfos = type.GetProperties();

            if(pinfos.Length > 0) {
                result.AppendLine("Property of {0}" + type.Name);

                foreach(PropertyInfo pi in pinfos)
                    result.AppendFormat("{0}={1}", pi.Name, pi.GetValue(obj, null)).AppendLine();
            }
            else {
                result.AppendFormat("Properties of [{0}] is not found.", type.Name);
            }
        }

        /// <summary>
        /// 인스턴스의 속성명, 값을 지정된 IDictionary에 채운다.
        /// </summary>
        /// <param name="obj">대상 인스턴스</param>
        /// <param name="props">속성정보를 담을 <see cref="IDictionary"/> 객체</param>
        public static void GetProperties(object obj, IDictionary props) {
            if(obj == null)
                return;

            var type = obj.GetType();
            var pinfos = type.GetProperties();

            foreach(var pi in pinfos)
                props.Add(pi.Name, pi.GetValue(obj, null));
        }

        /// <summary>
        /// 해당 객체의 Field 정보를 string으로 반환한다.
        /// </summary>
        /// <param name="obj">개체</param>
        /// <returns>Filed 정보</returns>
        public static string GetFields(object obj) {
            if(obj == null)
                return string.Empty;

            var result = new StringBuilder();
            GetFields(obj, result);

            return result.ToString();
        }

        /// <summary>
        /// 해당 객체의 Field 정보를 지정된 StringBuilder에 채운다.
        /// </summary>
        /// <param name="obj">개체</param>
        /// <param name="result"></param>
        public static void GetFields(object obj, StringBuilder result) {
            if(obj == null)
                return;

            var fields = new Dictionary<string, object>();
            GetFields(obj, fields);

            if(fields.Count > 0) {
                result.AppendFormat("Fields of [{0}]", obj.GetType().Name).AppendLine();

                foreach(var entiry in fields)
                    result.AppendFormat("{0}={1}", entiry.Key, entiry.Value).AppendLine();
            }
            else {
                result.AppendFormat("[{0}] has no Field", obj.GetType().Name);
            }
        }

        /// <summary>
        /// 해당 객체의 Field 정보를 지정된 Hashtable에 채운다.
        /// </summary>
        /// <param name="obj">개체</param>
        /// <param name="infos">필드 정보를 컬렉션 개체</param>
        public static void GetFields(object obj, IDictionary infos) {
            var type = obj.GetType();
            var fields = type.GetFields();

            foreach(FieldInfo field in fields)
                infos.Add(field.Name, field.GetValue(obj));
        }

        /// <summary>
        /// 특정 수형의 기본값을 반환합니다. ValueType인 경우는 기본 생성자를 통해 값을 반환하고, ValueType이 아닌 경우에는 null을 반환합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetTypeDefaultValue(this Type type) {
            if(type == null)
                return null;

            if(IsDebugEnabled)
                log.Debug("특정 수형[{0}]의 기본값을 구합니다...", type.FullName);

            object result = null;

            if(type.IsValueType)
                result = ActivatorTool.CreateInstance(type);

            if(IsDebugEnabled)
                log.Debug("특정 수형[{0}]의 기본값=[{1}] 입니다.", type.FullName, result ?? "null");

            return result;
        }

        /// <summary>
        /// 지정된 수형이 인자가 없는 기본 생성자를 제공하는지 여부를 반환합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasDefaultConstructor(this Type type) {
            type.ShouldNotBeNull("type");

            if(type.IsValueType)
                return true;

            return (type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null) != null);
        }

        /// <summary>
        /// 간단한 수형인지 판단한다.
        /// </summary>
        /// <remarks>
        /// <see cref="System.Type.IsPrimitive"/> 를 이면 간단한 수형이고,
        /// stirng, Decimal, DateTime, DBNull 형식이면 SimpleType이라 규정한다.
        /// </remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsSimpleTypeObject(this object value) {
            value.ShouldNotBeNull("value");
            return IsSimpleType(value.GetType());
        }

        /// <summary>
        /// 간단한 수형인지 판단한다.
        /// </summary>
        /// <remarks>
        /// <see cref="System.Type.IsPrimitive"/> 를 이면 간단한 수형이고, 
        /// stirng, Decimal, DateTime, DBNull 형식이면 SimpleType이라 규정한다.
        /// </remarks>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(this Type type) {
            if(type.IsPrimitive)
                return true;

            if(IsSameOrSubclassOf(type, typeof(string)))
                return true;

            if(IsSameOrSubclassOf(type, typeof(DateTime)))
                return true;

            if(IsSameOrSubclassOf(type, typeof(Decimal)))
                return true;

            if(IsSameOrSubclassOf(type, typeof(DBNull)))
                return true;

            return false;
        }

        /// <summary>
        /// check specified object is numeric type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(this object value) {
            if(value == null)
                return false;

            double dbl;
            return Double.TryParse(value.ToString(),
                                   NumberStyles.Any,
                                   NumberFormatInfo.InvariantInfo,
                                   out dbl);
        }

        /// <summary>
        /// <paramref name="type"/>이 Numeric 수형인지 판단합니다. Generic 클래스나 메소드에서 수학연산을 수행하기 위해 꼭 점검해야 합니다.
        /// </summary>
        /// <param name="type">대상 수형</param>
        /// <returns>대상 수형이 Numeric 수형이면 True, 아니면 False</returns>
        public static bool IsNumericType(this Type type) {
            type.ShouldNotBeNull("type");

            var isNumeric = false;
            var typeName = type.Name;

            if(type.IsValueType && NumericTypes.Contains(typeName))
                isNumeric = true;

            else if(typeName == "Nullable`1")
                isNumeric = IsNumericType(type.GetGenericArguments()[0]);

            if(IsDebugEnabled)
                log.Debug("수형[{0}] 이 Numeric 수형입니까? 결과=[{1}]", type.Name, isNumeric);

            return isNumeric;
        }

#if !SILVERLIGHT
        /// <summary>
        /// 지정된 수형이 ICloneable 인터페이스를 상속하여 Clone() 함수를 제공하는 지 여부를 판단한다.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isValueType">Value 타입인지를 </param>
        /// <returns></returns>
        public static bool IsCloneableType(this Type type, out bool isValueType) {
            isValueType = false;

            if(type.IsValueType) {
                isValueType = true;
                return true;
            }

            return typeof(ICloneable).IsAssignableFrom(type);
        }
#endif

        /// <summary>
        /// 지정된 인스턴스가 지정된 타입의 인스턴스인지 검사한다.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsInstanceOfType(this Type type, object target) {
            return type.IsInstanceOfType(target);
        }

        /// <summary>
        /// 인스턴스 생성을 할 수 있는 타입인가? 즉 new 로 인스턴스를 생성할 수 있는 형식인가?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInstantiatableType(this Type type) {
            type.ShouldNotBeNull("type");

            if(type.IsAbstract || type.IsInterface || type.IsArray || type.IsGenericTypeDefinition || type == typeof(void))
                return false;

            if(HasDefaultConstructor(type) == false)
                return false;

            return true;
        }

        /// <summary>
        /// 지정한 타입이 null을 할당 받을 수 있는지 검사합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type) {
            type.ShouldNotBeNull("type");

            if(type.IsValueType)
                return type.IsNullableType();

            return true;
        }

        /// <summary>
        /// 지정한 타입이 Nullable{T} 형식인지 검사합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type) {
            type.ShouldNotBeNull("type");
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// 지정된 형식이 SerializableAttribute를 지정하였는지 판단한다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSerializable(this Type type) {
            return ((type.Attributes & TypeAttributes.Serializable) == TypeAttributes.Serializable);
        }

        /// <summary>
        /// System.Type.Name 값에서 Namespace를 뺀 class name 만을 뽑아낸다.
        /// </summary>
        /// <param name="type">대상 인스턴스의 타입 객체</param>
        /// <returns>클래스명</returns>
        public static string SimpleClassName(this Type type) {
            if(type == null)
                return string.Empty;

            string name = type.Name;
            int index = name.IndexOfAny(new[] { '<', '{', '`' });

            if(index >= 0)
                name = name.Substring(0, index);

            return name;
        }

        /// <summary>
        /// 지정된 오브젝트 객체를 원하는 타입으로 형변환이 가능한지 알아본다.
        /// </summary>
        /// <param name="target">대상 객체</param>
        /// <param name="assignType">변경할 타입</param>
        /// <returns>변경 가능 여부</returns>
        public static bool CanAssign(object target, Type assignType) {
            return (target != null) && target.GetType().IsAssignableFrom(assignType);
        }

        /// <summary>
        /// 지정된 오브젝트 객체의 타입이 기본 타입과 같거나 상속한 타입인지 검사한다.
        /// </summary>
        /// <param name="target">검사할 객체</param>
        /// <param name="baseType">기본 타입</param>
        /// <returns>검사할 객체가 기본 타입과 같거나 상속된 타입이면 True, 아니면 False 이다. </returns>
        public static bool IsSameOrSubclassOf(object target, Type baseType) {
            return (target != null) && IsSameOrSubclassOf(target.GetType(), baseType);
        }

        /// <summary>
        /// 지정된 srcType이 baseType과 같거나 상속받은 클래스인가를 검사한다.
        /// </summary>
        /// <param name="srcType">검사할 타입</param>
        /// <param name="baseType">기본 타입</param>
        /// <returns></returns>
        public static bool IsSameOrSubclassOf(Type srcType, Type baseType) {
            return ReflectionTool.IsSameOrSubclassOf(srcType, baseType);
            //if(srcType == null || baseType == null)
            //    return false;

            //return (Equals(srcType, baseType) || srcType.IsSubclassOf(baseType));
        }

        /// <summary>
        /// 지정된 srcType이 baseType과 같거나 상속받은 클래스인가 또는 baseType이 Interface이고, 이 인터페이스를 구현한 것인지 검사한다.
        /// </summary>
        /// <param name="target">검사할 객체</param>
        /// <param name="baseType">기본 타입</param>
        /// <returns></returns>
        public static bool IsSameOrSubclassOrImplementedOf(object target, Type baseType) {
            if(target == null)
                return false;

            return IsSameOrSubclassOrImplementedOf(target.GetType(), baseType);
        }

        /// <summary>
        /// 지정된 srcType이 baseType과 같거나 상속받은 클래스인가 또는 baseType이 Interface이고, 이 인터페이스를 구현한 것인지 검사한다.
        /// </summary>
        /// <param name="srcType">검사할 타입</param>
        /// <param name="baseType">기본 타입</param>
        /// <returns></returns>
        public static bool IsSameOrSubclassOrImplementedOf(Type srcType, Type baseType) {
            if(srcType == null || baseType == null)
                return false;

            if(IsSameOrSubclassOf(srcType, baseType))
                return true;

            if(baseType.IsInterface)
                return srcType.GetInterfaces().Any(intfType => intfType == baseType);

            return false;
        }

        /// <summary>
        /// Type을 문자열로 표현한다. 형식은 [Type.FullName, AssemblyName] 이 된다. 
        /// (Assembly의 Qualified Name은 version, public key도 포함하지만 여기서는 포함하지 안는다.)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToStringWithAssemblyName(this Type type) {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        /// <summary>
        /// 개방된 Generic 수형을 <paramref name="innerTypes"/> 들로 Closed Generic 수형을 생성합니다.
        /// </summary>
        /// <param name="genericTypeDefinition"></param>
        /// <param name="innerTypes"></param>
        /// <returns></returns>
        public static Type MakeGenericType(Type genericTypeDefinition, params Type[] innerTypes) {
            genericTypeDefinition.ShouldNotBeNull("genericTypeDefinition");
            innerTypes.ShouldNotBeEmpty("innerTypes");
            Guard.Assert(() => genericTypeDefinition.IsGenericTypeDefinition, @"Type [{0}] is not a generic type definition.",
                         genericTypeDefinition);

            return genericTypeDefinition.MakeGenericType(innerTypes);
        }

        /// <summary>
        /// 개방된 Generic 수형을 <paramref name="innerType"/>의 Closed Generic 수형으로 만들고, 인스턴스를 생성해서 반환합니다.
        /// </summary>
        /// <param name="genericTypeDefinition">Opened Generic Type</param>
        /// <param name="innerType">Generic의 내부 수형</param>
        /// <param name="args">생성자의 인자들</param>
        /// <returns></returns>
        public static object CreateGeneric(Type genericTypeDefinition, Type innerType, params object[] args) {
            innerType.ShouldNotBeNull("innerType");
            return CreateGeneric(genericTypeDefinition, new Type[] { innerType }, args);
        }

        /// <summary>
        /// 개방된 Generic 수형을 <paramref name="innerTypes"/>의 Closed Generic 수형으로 만들고, 인스턴스를 생성해서 반환합니다.
        /// </summary>
        /// <param name="genericTypeDefinition">Opened Generic Type</param>
        /// <param name="innerTypes">Generic의 내부 수형</param>
        /// <param name="args">생성자의 인자들</param>
        /// <returns></returns>
        public static object CreateGeneric(Type genericTypeDefinition, Type[] innerTypes, params object[] args) {
            genericTypeDefinition.ShouldNotBeNull("genericTypeDefinition");
            innerTypes.ShouldNotBeNull("innerTypes");

            if(IsDebugEnabled)
                log.Debug("Generic 수형의 인스턴스를 생성합니다. GenericType=[{0}], innerTypes=[{1}], args=[{2}]",
                          genericTypeDefinition.FullName, innerTypes.CollectionToString(), args.CollectionToString());

            Type genericType = MakeGenericType(genericTypeDefinition, innerTypes);
            return ActivatorTool.CreateInstance(genericType, args);
        }
    }
}