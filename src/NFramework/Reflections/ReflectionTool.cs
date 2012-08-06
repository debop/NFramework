using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// Reflection 관련 Helper class
    /// </summary>
    public static partial class ReflectionTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Public Key Token
        /// </summary>
        public static string PublicKeyTokenLiteral = "PublicKeyToken";

#if !SILVERLIGHT
        /// <summary>
        /// 현 어플리케이션 도메인 안에서 지정한 어셈블리명을 가진 Assembly를 로드한다.	(예: LoadAssembly("NSoft.NFramework.dll"); )
        /// </summary>
        /// <param name="assemblyName">어셈블리명</param>
        /// <returns>지정된 이름을 가진 Assembly 객체를 반환한다.</returns>
        public static Assembly LoadAssembly(string assemblyName) {
            return LoadAssembly(assemblyName, AppDomain.CurrentDomain);
        }

        /// <summary>
        /// 지정된 어플리케이션 도메인에서 지정한 어셈블리명을 가진 Assembly를 로드한다. (예: LoadAssembly("NSoft.NFramework.dll"); )
        /// </summary>
        /// <param name="assemblyName">어셈블리명</param>
        /// <param name="domain">어플리케이션 도메인</param>
        /// <returns>해당 어셈블리</returns>
        public static Assembly LoadAssembly(string assemblyName, AppDomain domain) {
            return domain.Load(Path.GetFileNameWithoutExtension(assemblyName));
        }

        /// <summary>
        /// 지정한 Type이 속한 Assembly에 있는 모든 Namespace 정보를 추출한다.
        /// </summary>
        /// <param name="type">Type 인스턴스</param>
        /// <param name="namespaces">Namespace 컬렉션</param>
        public static void GetNamespaces(this Type type, ICollection<string> namespaces) {
            type.ShouldNotBeNull("type");
            GetNamespaces(Assembly.GetAssembly(type), namespaces);
        }

        /// <summary>
        /// 지정한 Application Domain에 속한 Assembly에 있는 모든 Namespace 정보를 추출한다.
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="namespaces"></param>
        public static void GetNamespaces(this AppDomain domain, ICollection<string> namespaces) {
            var assemblies = (domain ?? AppDomain.CurrentDomain).GetAssemblies();

            foreach(var assembly in assemblies)
                GetNamespaces(assembly, namespaces);
        }
#endif

        /// <summary>
        /// 지정한 어셈블리에 속한 Assembly에 있는 모든 Namespace 정보를 추출한다.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="namespaces">어셈블리에 있는 모든 Namespace의 컬렉션</param>
        public static void GetNamespaces(this Assembly assembly, ICollection<string> namespaces) {
            assembly.ShouldNotBeNull("assembly");

            if(IsDebugEnabled)
                log.Debug("Get all namespace from assembly, and fill to namespace. assembly=[{0}]", assembly.FullName, namespaces);

            foreach(var runtime in assembly.GetModules())
                foreach(var type in runtime.GetTypes()) {
                    if(type.Namespace.IsNotEmpty())
                        if(namespaces.Contains(type.Namespace) == false && type.IsPublic)
                            namespaces.Add(type.Namespace);
                }
        }

        /// <summary>
        /// 지정된 어셈블리에 있는 지정된 형식 이름의 형식을 구한다.
        /// </summary>
        /// <param name="assembly">형식이 정의된 어셈블리명</param>
        /// <param name="typeFullName">형식의 전체 이름</param>
        /// <returns>존재하면 Type 반환, 없을 시에는 null값 반환</returns>
        public static Type GetType(this Assembly assembly, string typeFullName) {
            assembly.ShouldNotBeNull("assembly");
            typeFullName.ShouldNotBeWhiteSpace("typeFullName");

            if(IsDebugEnabled)
                log.Debug("Get Type from assembly with type name. assembly=[{0}], typeFullName=[{1}]", assembly.FullName, typeFullName);

            return assembly.GetTypes().FirstOrDefault(t => t.FullName.EqualTo(typeFullName));
        }

        /// <summary>
        /// 지정된 형식에 내재된(Nested) 형식을 찾는다.
        /// </summary>
        /// <param name="type">조사할 Type 인스턴스</param>
        /// <returns>내재된 (Nested) 형식 목록</returns>
        public static Type[] GetNestedTypes(this Type type) {
            type.ShouldNotBeNull("type");

            if(IsDebugEnabled)
                log.Debug("Get all nested type from the specified type. type=[{0}]", type.FullName);

            Type[] nested;

            if(type == null || type.IsEnum || type.IsInterface) {
                nested = new Type[0];
            }
            else {
                nested = type.GetNestedTypes(
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);
            }
            return nested;
        }

        /// <summary>
        /// 지정된 Assembly에 SerializableAttribute를 지정한 Type의 Full Name을 얻는다.
        /// </summary>
        /// <param name="assembly">어셈블리</param>
        /// <param name="typeNames"></param>
        public static void GetSerializableTypeNames(this Assembly assembly, ICollection<string> typeNames) {
            assembly.ShouldNotBeNull("assembly");

            foreach(var type in assembly.GetTypes().Where(IsSerializable))
                typeNames.Add(type.FullName);
        }

        /// <summary>
        /// 지정된 Class가 SerializableAttribute 를 가지고 있는지 검사한다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSerializable(this Type type) {
            type.ShouldNotBeNull("type");
            return ((type.Attributes & TypeAttributes.Serializable) == TypeAttributes.Serializable);
        }

#if !SILVERLIGHT
        /// <summary>
        /// 해당 어셈블리의 참조 어셈블리들의 이름을 가져온다.
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="dependList"></param>
        public static void GetReferencedAssemblyNames(string assemblyPath, ICollection<string> dependList) {
            assemblyPath.ShouldNotBeWhiteSpace("assemblyPath");

            if(IsDebugEnabled)
                log.Debug("해당 어셈블리의 참조 어셈블리들의 이름을 가져온다... assemblyPath=[{0}]", assemblyPath);

            var assembly
                = File.Exists(assemblyPath)
                      ? Assembly.LoadFrom(assemblyPath)
                      : Assembly.Load(assemblyPath);

            foreach(var asm in assembly.GetReferencedAssemblies().Where(a => !dependList.Contains(a.FullName)))
                GetReferencedAssemblyNames(asm.FullName, dependList);
        }
#endif

        /// <summary>
        /// 기본 Class를 상속 받은 클래스 형식들을 가져온다.
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="baseType"></param>
        /// <param name="drivedClasses"></param>
        public static void GetDrivedClasses(string assemblyPath, Type baseType, IList drivedClasses) {
            assemblyPath.ShouldNotBeWhiteSpace("assemblyPath");

            var assembly
                = File.Exists(assemblyPath)
                      ? Assembly.LoadFrom(assemblyPath)
                      : Assembly.Load(assemblyPath);

            if(assembly == null)
                return;

            foreach(var type in assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)))
                drivedClasses.Add(type);
        }

        /// <summary>
        /// Obsoleted - <see cref="ReflectionTool.CanAssign(object, Type)"/>을 사용하세요.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="castingType"></param>
        /// <returns></returns>
        /// <seealso cref="CanAssign"/>
        [Obsolete("ReflectionTool.CanAssign(object,type) 을 사용하세요.")]
        public static bool CanCasting(object o, Type castingType) {
            return CanAssign(o, castingType);
        }

        /// <summary>
        /// 지정된 오브젝트 객체를 원하는 타입으로 형변환이 가능한지 알아본다.
        /// </summary>
        /// <param name="o">대상 객체</param>
        /// <param name="assignType">변경할 타입</param>
        /// <returns>변경 가능 여부</returns>
        public static bool CanAssign(object o, Type assignType) {
            return (o != null) && o.GetType().IsAssignableFrom(assignType);
        }

        /// <summary>
        /// 지정된 오브젝트 객체를 원하는 타입으로 형변환이 가능한지 알아본다.
        /// </summary>
        /// <param name="o">대상 객체</param>
        /// <typeparam name="T">변경할 타입</typeparam>
        /// <returns>변경 가능 여부</returns>
        public static bool CanAssign<T>(object o) {
            return (o != null) && o.GetType().IsAssignableFrom(typeof(T));
        }

        /// <summary>
        /// 지정된 오브젝트 객체의 타입이 기본 타입과 같거나 상속한 타입인지 검사한다.
        /// </summary>
        /// <param name="o">검사할 객체</param>
        /// <param name="baseType">기본 타입</param>
        /// <returns>검사할 객체가 기본 타입과 같거나 상속된 타입이면 True, 아니면 False 이다. </returns>
        public static bool IsSameOrSubclassOf(object o, Type baseType) {
            return (o != null) && IsSameOrSubclassOf(o.GetType(), baseType);
        }

        /// <summary>
        /// 지정된 srcType이 baseType과 같거나 상속받은 클래스인가를 검사한다.
        /// </summary>
        /// <param name="srcType">검사할 타입</param>
        /// <param name="baseType">기본 타입</param>
        /// <returns></returns>
        public static bool IsSameOrSubclassOf(this Type srcType, Type baseType) {
            return srcType == baseType || IsSubClass(srcType, baseType);
        }

        public static bool IsSubClass(this Type srcType, Type baseType) {
            Type implentingType;
            return IsSubClass(srcType, baseType, out implentingType);
        }

        public static bool IsSubClass(this Type srcType, Type baseType, out Type implentatingType) {
            srcType.ShouldNotBeNull("srcType");
            baseType.ShouldNotBeNull("baseType");

            var result = IsSubClassInternal(srcType, srcType, baseType, out implentatingType);

            if(IsDebugEnabled)
                log.Debug("형식[{0}]은 BaseType[{1}]을 상속한 형식인지 판단합니다. 결과=[{2}]", srcType.Name, baseType.Name, result);

            return result;
        }

        /// <summary>
        /// <see cref="Type.IsSubclassOf(Type)"/> 이 interface 또는 Generic type에 대한 상속 관계를 파악하지 않습니다.
        /// 이 함수에서는 interface 상속/구현 관계, Generic type의 상속 관계도 파악하여, Subclass인지를 파악합니다.
        /// </summary>
        private static bool IsSubClassInternal(Type initialType, Type currentType, Type check, out Type implementingType) {
            if(currentType == check) {
                implementingType = currentType;
                return true;
            }
            // don't get interfaces for an interface unless the initial type is an interface
            if(check.IsInterface && (initialType.IsInterface || currentType == initialType)) {
                foreach(Type t in currentType.GetInterfaces()) {
                    if(IsSubClassInternal(initialType, t, check, out implementingType)) {
                        // interface 자체를 반환해서는 안되고, 구현체(implementor)를 반환한다.
                        if(check == implementingType)
                            implementingType = currentType;
                        return true;
                    }
                }
            }
            if(currentType.IsGenericType && currentType.IsGenericTypeDefinition == false) {
                if(IsSubClassInternal(initialType, currentType.GetGenericTypeDefinition(), check, out implementingType)) {
                    implementingType = currentType;
                    return true;
                }
            }

            if(currentType.BaseType == null) {
                implementingType = null;
                return false;
            }

            return IsSubClassInternal(initialType, currentType.BaseType, check, out implementingType);
        }

        public static bool HasInterface(this Type srcType, Type interfaceType) {
            return srcType.GetInterfaces().Contains(interfaceType);
        }

        /// <summary>
        /// 지정된 Method에 지정된 Attribute 형식이 적용되었는지 확인한다.
        /// </summary>
        /// <param name="mi">메소드 정보 개체</param>
        /// <param name="attributeType">사용자 정의 Attribute</param>
        /// <param name="inherit">메소드의 상속 Chain도 검사할 것인지 여부를 지정합니다.</param>
        /// <returns></returns>
        public static bool HasCustomAttributes(this MethodInfo mi, Type attributeType, bool inherit = false) {
            mi.ShouldNotBeNull("mi");
            attributeType.ShouldNotBeNull("attributeType");

            return (mi.GetCustomAttributes(attributeType, inherit).Length > 0);
        }

        /// <summary>
        /// 지정된 속성에 해당 Attribute 속성의 지정 여부를 검사한다.
        /// </summary>
        /// <param name="fi">속성 정보</param>
        /// <param name="attributeType">Attribute 타입</param>
        /// <param name="inherit">상속 Chain도 검사할 것인지 여부를 지정합니다.</param>
        /// <returns></returns>
        public static bool HasCustomAttributes(this FieldInfo fi, Type attributeType, bool inherit = false) {
            fi.ShouldNotBeNull("fi");
            attributeType.ShouldNotBeNull("attributeType");

            return fi.IsDefined(attributeType, inherit);
        }

#if !SILVERLIGHT

        /// <summary>
        /// 지정된 컴포넌트의 Attribute 정보중에 기본 클래스에서 상속되었는지 여부를 알 수 있는 <c>InheritanceAttrbute</c> 클래스를 가져온다.
        /// </summary>
        /// <param name="component"></param>
        /// <returns>상속여부를 알 수 있는 Attribute가 없다면 null을 반환한다.</returns>
        public static InheritanceAttribute GetInheritanceAttribute(this Component component) {
            component.ShouldNotBeNull("component");

            var attrs = TypeDescriptor.GetAttributes(component);

            return attrs.OfType<InheritanceAttribute>().FirstOrDefault();
        }

#endif

        /// <summary>
        /// 지정된 인스턴스의 속성 정보를 가져온다.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static IList<PropertyInfo> GetPropertyInfos(this Type objectType) {
            objectType.ShouldNotBeNull("objectType");

            var result = new List<PropertyInfo>();

            var propInfos = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

            result.AddRange(propInfos);

            // ObjectToString() 을 가지고 HashCode를 작성할 때, PropertyInfo의 순서 때문에 
            // 같은 속성들을 가져도 다르게 나오는 것을 방지하기 위해서이다.
            result.Sort((a, b) => string.Compare(a.Name, b.Name));

            #region << Obsolete Code >>

            //PropertyDescriptorCollection propDescriptors = TypeDescriptor.GetProperties(objectType);

            //foreach (PropertyDescriptor propDescriptor in propDescriptors)
            //{
            //    try
            //    {
            //        // BindigFlags.Instance를 하지 않으면 public static 속성도 가져온다.
            //        PropertyInfo pi = objectType.GetProperty(propDescriptor.Name, BindingFlags.Public | BindingFlags.Instance);
            //        result.Add(pi);
            //    }
            //    catch(AmbiguousMatchException ame)
            //    {
            //        if (IsDebugEnabled)
            //        {
            //            log.Debug("인스턴스의 속성정보를 가져오기는데 실패했습니다. ObjectType={0}, Property Name={1}",
            //                           objectType.FullName, propDescriptor.Name);
            //            log.Debug("속성 조회시에 예외가 발생했습니다.", ame);
            //        }
            //    }
            //    catch(Exception ex)
            //    {
            //        if (log.IsWarnEnabled)
            //        {
            //            log.WarnException("인스턴스의 속성정보를 가져오기는데 실패했습니다. ObjectType={0}, Property Name={1}",
            //                           objectType.FullName, propDescriptor.Name);
            //            log.WarnException("속성 조회시에 예외가 발생했습니다.", ex);
            //        }
            //    }
            //}

            #endregion

            return result;
        }

        /// <summary>
        /// 지정된 속성정보가 Index를 가지는 속성인지 판단합니다.
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static bool IsIndexedProperty(this PropertyInfo pi) {
            pi.ShouldNotBeNull("pi");
            return (pi.GetIndexParameters().Length > 0);
        }

        /// <summary>
        /// Nullable{T} 를 고려해서 속성 형식을 가져온다.
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static Type GetPropertyType(Type propertyType) {
            var type = propertyType;

            // nullable 인지 파악한다.
            //
            if(type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                return Nullable.GetUnderlyingType(type);

            return type;
        }

#if !SILVERLIGHT

        /// <summary>
        /// 지정된 형식이 정의된 어셈블리의 공개키 토큰을 구한다.
        /// </summary>
        /// <param name="type">타입</param>
        /// <returns>공개 키 토큰</returns>
        public static string GetPublicKeyToken(this Type type) {
            type.ShouldNotBeNull("type");
            return GetPublicKeyToken(Assembly.GetAssembly(type));
        }

#endif

        /// <summary>
        /// 지정된 어셈블리의 Public Key 토큰을 가져온다.
        /// </summary>
        /// <param name="assembly">어셈블리</param>
        /// <returns>공개 키 토큰</returns>
        public static string GetPublicKeyToken(this Assembly assembly) {
            assembly.ShouldNotBeNull("assembly");

            if(IsDebugEnabled)
                log.Debug("어셈블리에서 Public key token을 찾습니다... assembly=[{0}]", assembly.FullName);

            var asmNames = assembly.FullName.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

            for(var i = asmNames.Length - 1; i > 0; i--) {
                if(asmNames[i - 1].Length < PublicKeyTokenLiteral.Length)
                    continue;

                var subStr = asmNames[i - 1].Substring(asmNames[i - 1].Length - PublicKeyTokenLiteral.Length);

                if(subStr.Equals(PublicKeyTokenLiteral)) {
                    if(asmNames[i].Length < 17)
                        return asmNames[i];
                    return asmNames[i].Substring(0, 16);
                }
            }

            if(log.IsWarnEnabled)
                log.Warn("어셈블리에 Public key token을 찾지 못했습니다!!! assembly=[{0}]", assembly.FullName);

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static Type GetMemberUnderlyingType(MemberInfo member) {
            member.ShouldNotBeNull("member");

            switch(member.MemberType) {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                default:
                    throw new ArgumentException("MemberType 속성값이 MemberTypes.Field or Property or Event 여야합니다.", "member");
            }
        }
    }
}