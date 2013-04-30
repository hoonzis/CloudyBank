#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Markup;
using System.Xml;
using CloudyBank.Web.Ria.Technical.XamlSerialization;

#if WINDOWS_PHONE
using System.Windows;
#endif

#endregion

namespace SLaB.Utilities.Xaml.Serializer
{
    /// <summary>
    ///   A utility class for serializing objects into Xaml.  Does not recognize UI concepts such as DependencyProperties.
    /// </summary>
    public class XamlSerializer
    {

        private readonly Dictionary<Thread, SerializeState> _State;
        /// <summary>
        ///   A mapping of target types to discovered attached properties.  Derived classes can pre-populate this collection.
        /// </summary>
        protected Dictionary<Type, List<AttachedProperty>> AttachedProperties;
        /// <summary>
        ///   A TypeConverter that knows how to convert built-in (i.e. mscorlib) types to strings.
        /// </summary>
        protected static readonly TypeConverter BuiltIn = new BuiltInTypeConverter();
        private static readonly Dictionary<Type, PropertyInfo> ContentPropertyCache =
            new Dictionary<Type, PropertyInfo>();
        private static readonly Dictionary<PropertyInfo, DefaultValueAttribute> DefaultValueCache =
            new Dictionary<PropertyInfo, DefaultValueAttribute>(PropEquals);
        /// <summary>
        ///   A set of the types that have already had their attached properties discovered.
        /// </summary>
        protected HashSet<Type> DiscoveredTypes;
        private static readonly CultureInfo EnUsCulture = new CultureInfo("en-us");
        private static readonly Dictionary<Type, bool> IsListCache = new Dictionary<Type, bool>();
        private static readonly object NotSetObject = new object();
        /// <summary>
        ///   An IEqualityComparer that compares two PropertyInfo objects for equality.
        /// </summary>
        protected static readonly IEqualityComparer<PropertyInfo> PropEquals =
            new DelegateEqualityComparer<PropertyInfo>(
                (x, y) => x.Name.Equals(y.Name) && x.DeclaringType.Equals(y.DeclaringType),
                p => p.Name.GetHashCode() ^ p.DeclaringType.FullName.GetHashCode());
        private static readonly Dictionary<Type, IEnumerable<PropertyInfo>> PropertiesCache =
            new Dictionary<Type, IEnumerable<PropertyInfo>>();
        /// <summary>
        ///   A set of properties to skip when serializing in order to avoid properties that will inevitably throw.
        /// </summary>
        protected HashSet<PropertyInfo> PropertiesToSkip;
        private static readonly Dictionary<PropertyInfo, Func<object, object>> PropertyGetterCache =
            new Dictionary<PropertyInfo, Func<object, object>>(PropEquals);
        /// <summary>
        ///   An IEqualityComparer that checks to see if two items refer to the same object (reference equality).
        /// </summary>
        protected static readonly IEqualityComparer<object> RefEquals =
            new DelegateEqualityComparer<object>(ReferenceEquals, o => o.GetHashCode());
        private static readonly Dictionary<PropertyInfo, MethodInfo> ShouldSerializeMethodCache =
            new Dictionary<PropertyInfo, MethodInfo>(PropEquals);
        /// <summary>
        ///   Supplemental TypeConverters (in case a supplied TypeConverter is missing or lacks a ConvertTo implementation).
        /// </summary>
        protected IDictionary<Type, TypeConverter> TypeConverters;
        /// <summary>
        ///   The namespace for Xaml language features (commonly prefixed as "x:")
        /// </summary>
        public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
        private static readonly Dictionary<Assembly, IEnumerable<XmlnsDefinitionAttribute>> XmlnsDefinitionCache =
            new Dictionary<Assembly, IEnumerable<XmlnsDefinitionAttribute>>();
        private static readonly Dictionary<Assembly, IEnumerable<XmlnsPrefixAttribute>> XmlnsPrefixCache =
            new Dictionary<Assembly, IEnumerable<XmlnsPrefixAttribute>>();



        /// <summary>
        ///   Constructs a XamlSerializer.
        /// </summary>
        public XamlSerializer()
        {
            this.AttachedProperties = new Dictionary<Type, List<AttachedProperty>>();
            this.DiscoveredTypes = new HashSet<Type>();
            this.TypeConverters = new Dictionary<Type, TypeConverter>();
            this.PropertiesToSkip = new HashSet<PropertyInfo>(PropEquals);
            this._State = new Dictionary<Thread, SerializeState>();
        }



        private SerializeState State
        {
            get { return this._State[Thread.CurrentThread]; }
        }




        /// <summary>
        ///   Searches an assembly for attached properties that can be serialized in Xaml.
        /// </summary>
        /// <param name = "asm">The assembly to search.</param>
        public void DiscoverAttachedProperties(Assembly asm)
        {
            foreach (Type t in asm.GetTypes().Where(t => t.IsPublic))
                DiscoverAttachedProperties(t);
        }

        /// <summary>
        ///   Searches a type for attached properties that can be serialized in Xaml.
        /// </summary>
        /// <param name = "type">The type to search.</param>
        public void DiscoverAttachedProperties(Type type)
        {
            if (this.DiscoveredTypes.Contains(type))
                return;
            try
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                var getters = from m in methods
                              where m.Name.StartsWith("Get") && m.GetParameters().Length == 1
                              select m;
                var setters = from m in methods
                              where m.Name.StartsWith("Set") && m.GetParameters().Length == 2
                              select m;
                var attachedProps = from g in getters
                                    from s in setters
                                    let ap = new AttachedProperty { Getter = g, Setter = s }
                                    where ap.Validate()
                                    select ap;
                attachedProps = attachedProps.Concat(from g in getters.Except(attachedProps.Select(ap => ap.Getter))
                                                     let ap = new AttachedProperty { Getter = g }
                                                     where ap.Validate()
                                                     select ap);
                foreach (var ap in attachedProps)
                {
                    this.PrepareDictionary(ap.TargetType);
                    this.AttachedProperties[ap.TargetType].Add(ap);
                }
                this.DiscoveredTypes.Add(type);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///   Registers an attached property for serialization explicitly based upon its getter and setter.
        /// </summary>
        /// <param name = "getter">The getter for the attached property.</param>
        /// <param name = "setter">The setter for the attached property.</param>
        /// <returns>true if the attached property was valid and registered, otherwise false.</returns>
        public bool RegisterAttachedProperty(MethodInfo getter, MethodInfo setter)
        {
            AttachedProperty prop = new AttachedProperty { Getter = getter, Setter = setter };
            if (prop.Validate())
            {
                this.PrepareDictionary(prop.TargetType);
                this.AttachedProperties[prop.TargetType].Add(prop);
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Serializes an object to Xaml.
        /// </summary>
        /// <param name = "obj">The object to serialize.</param>
        /// <returns>A Xaml string representing the object.</returns>
        public virtual string Serialize(object obj)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = this.GetXmlWriter(sb);
            writer.WriteStartDocument();
            Dictionary<string, string> mappings = new Dictionary<string, string>();
            HashSet<object> cycleCheckObjects = new HashSet<object>(RefEquals);
            this.PreSerialize(obj, mappings, cycleCheckObjects);
            if (this.CanWriteObject(obj, cycleCheckObjects))
                this.VisitObject(obj, writer, mappings, cycleCheckObjects, true);
            writer.WriteEndDocument();
            writer.Close();
            string result = sb.ToString();
            this.PostSerialize(obj, ref result);
            return result;
        }

        /// <summary>
        ///   Checks to see whether an object can be serialized in Xaml.
        /// </summary>
        /// <param name = "obj">The object to be serialized.</param>
        /// <param name = "cycleCheckObjects">The set of objects currently on the stack (used to avoid cycles).</param>
        /// <returns>true if the object can be serialized, false otherwise.</returns>
        protected virtual bool CanWriteObject(object obj, ISet<object> cycleCheckObjects)
        {
            if (!this.IsUnique(obj, cycleCheckObjects))
                return false;
            Type objType = obj.GetType();
            if (objType.IsNotPublic)
                return false;

            // Try to get the TypeConverter
            TypeConverter tc = this.GetTypeConverter(objType);
            try
            {
                if (tc != null && tc.CanConvertTo(typeof(string)))
                {
                    tc.ConvertTo(null, EnUsCulture, obj, typeof(string));
                    return true;
                }
            }
            catch
            {
            }

            if (obj is Array)
                return false;
            if (objType.IsGenericType)
                return false;
            if (obj is IList && ((IList)obj).IsReadOnly)
                return false;
            if (new AssemblyName(obj.GetType().Assembly.FullName).Name.Equals("mscorlib"))
                return false;

            return objType.IsValueType || objType.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <summary>
        ///   Checks to see whether a property is writeable (and thus can be serialized).
        /// </summary>
        /// <param name = "p">The property to check.</param>
        /// <returns>true if the property can be written, false otherwise.</returns>
        protected bool CanWriteProperty(PropertyInfo p)
        {
            return p.CanWrite && p.GetSetMethod() != null && p.GetSetMethod().IsPublic;
        }

        protected bool CanSerializeProperty(PropertyInfo p)
        {
            var attributes = p.GetCustomAttributes(typeof(XamlSerializationVisibility), true);
            if (attributes.Count() != 1)
            {
                return true;
            }
            var att = attributes[0] as XamlSerializationVisibility;
            return att.Visibility == SerializationVisibility.Visible;
            
        }

        /// <summary>
        ///   Gets a string for an attached property used to determine the order in which attached properties will be serialized.
        /// </summary>
        /// <param name = "ap">The attached property being serialized.</param>
        /// <returns>A sortable string representation for the attached property.</returns>
        protected virtual string GetAttachedOrder(AttachedProperty ap)
        {
            return ap.Name;
        }

        /// <summary>
        ///   Gets all of the attached properties that could be applied to the specified type (or its base types or interfaces).
        /// </summary>
        /// <param name = "t">The type for which attached properties should be retrieved.</param>
        /// <returns>A collection of attached properties for the given type.</returns>
        protected IEnumerable<AttachedProperty> GetAttachedProperties(Type t)
        {
            if (this.State.AttachedPropertiesCache.ContainsKey(t))
                return this.State.AttachedPropertiesCache[t];
            IEnumerable<AttachedProperty> result = new AttachedProperty[0];
            if (this.AttachedProperties.ContainsKey(t))
                result = result.Concat(this.AttachedProperties[t]);
            result = t.GetInterfaces().Where(
                iFace => this.AttachedProperties.ContainsKey(iFace)).Aggregate(result,
                                                                               (current, iFace) =>
                                                                               current.Concat(this.AttachedProperties[t]));
            if (!(t.Equals(typeof(object))))
                result = result.Concat(this.GetAttachedProperties(t.BaseType));
            return this.State.AttachedPropertiesCache[t] = result;
        }

        /// <summary>
        ///   Gets the ContentProperty for the given object.
        /// </summary>
        /// <param name = "obj">The object being serialized.</param>
        /// <returns>The ContentProperty for the given object, or null if one is not specified.</returns>
        protected PropertyInfo GetContentProperty(object obj)
        {
            if (ContentPropertyCache.ContainsKey(obj.GetType()))
                return ContentPropertyCache[obj.GetType()];
            ContentPropertyAttribute cpa =
                obj.GetType().GetCustomAttributes(typeof(ContentPropertyAttribute), true)
                    .Cast<ContentPropertyAttribute>().LastOrDefault();
            if (cpa == null)
                return ContentPropertyCache[obj.GetType()] = null;
            PropertyInfo prop = obj.GetType().GetProperty(cpa.Name);
            return ContentPropertyCache[obj.GetType()] = prop;
        }

        /// <summary>
        ///   Gets the xml namespace for the given type.
        /// </summary>
        /// <param name = "t">The type whose namespace must be retrieved.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <returns>The xml namespace for the type.</returns>
        protected string GetNamespace(Type t, IDictionary<string, string> prefixMappings)
        {
            Assembly asm = t.Assembly;
            IEnumerable<XmlnsDefinitionAttribute> xmlnsDefs = XmlnsDefinitionCache.ContainsKey(asm)
                                                                  ? XmlnsDefinitionCache[asm]
                                                                  : XmlnsDefinitionCache[asm] =
                                                                    asm.GetCustomAttributes(
                                                                        typeof(XmlnsDefinitionAttribute), false).Cast
                                                                        <XmlnsDefinitionAttribute>();
            IEnumerable<XmlnsPrefixAttribute> prefixes = XmlnsPrefixCache.ContainsKey(asm)
                                                             ? XmlnsPrefixCache[asm]
                                                             : XmlnsPrefixCache[asm] =
                                                               asm.GetCustomAttributes(typeof(XmlnsPrefixAttribute),
                                                                                       false).Cast
                                                                   <XmlnsPrefixAttribute>();
            if (!this.State.LoadedAsms.Contains(asm))
            {
                foreach (var prefix in prefixes.Where(prefix => !prefixMappings.ContainsKey(prefix.XmlNamespace)))
                {
                    if (!prefix.Prefix.Equals("x") ||
                        prefix.XmlNamespace.Equals(XamlNamespace))
                        prefixMappings[prefix.XmlNamespace] = prefix.Prefix;
                    else
                        prefixMappings[prefix.XmlNamespace] = prefix.Prefix + "1";
                }
                this.State.LoadedAsms.Add(asm);
            }

            if (this.State.NsMappings.ContainsKey(t))
                return this.State.NsMappings[t];
            foreach (var xmlnsDef in
                xmlnsDefs.Where(xmlnsDef => xmlnsDef.ClrNamespace.Equals(t.Namespace)).OrderBy(att => att.XmlNamespace))
            {
                return this.State.NsMappings[t] = xmlnsDef.XmlNamespace;
            }
            return
                this.State.NsMappings[t] =
                string.Format("clr-namespace:{0};assembly={1}", t.Namespace, new AssemblyName(asm.FullName).Name);
        }

        /// <summary>
        ///   Gets an Xml prefix for the given namespace, taking into consideration any existing prefixes in the XmlWriter.
        /// </summary>
        /// <param name = "ns">The namespace for which a namespace must be assigned.</param>
        /// <param name = "prefixMappings">Existing prefix mappings.</param>
        /// <param name = "writer">The writer which will consume the prefix.</param>
        /// <returns>A prefix for the given namespace.</returns>
        protected virtual string GetPrefix(string ns, IDictionary<string, string> prefixMappings, XmlWriter writer)
        {
            if (ns == null)
                return null;
            string foundPrefix = writer.LookupPrefix(ns);
            if (foundPrefix != null)
                return foundPrefix;
            string preNs = ns;
            if (!prefixMappings.ContainsKey(preNs))
            {
                if (ns.StartsWith("clr-namespace"))
                {
                    ns = ns.Replace("clr-namespace:", "");
                    ns = ns.Replace(";assembly=", "_");
                }
                ns = Regex.Replace(ns, @"\W", "_");
                prefixMappings[preNs] = ns;
            }
            return prefixMappings[preNs];
        }

        /// <summary>
        /// Gets the property getter.
        /// </summary>
        /// <param name="inf">The PropertyInfo from which to get the getter.</param>
        /// <returns></returns>
        protected static Func<object, object> GetPropertyGetter(PropertyInfo inf)
        {
            if (PropertyGetterCache.ContainsKey(inf))
                return PropertyGetterCache[inf];
#if WINDOWS_PHONE
            return PropertyGetterCache[inf] = (obj) => inf.GetValue(obj, null);
#else
            var param = System.Linq.Expressions.Expression.Parameter(typeof(object));
            var cast = System.Linq.Expressions.Expression.Convert(param, inf.DeclaringType);
            var getValue = System.Linq.Expressions.Expression.Property(cast, inf);
            var finalCast = System.Linq.Expressions.Expression.Convert(getValue, typeof(object));
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object>>(finalCast, param);
            return PropertyGetterCache[inf] = lambda.Compile();
#endif
        }

        /// <summary>
        ///   Gets a string for a property used to determine the order in which properties will be serialized.
        /// </summary>
        /// <param name = "prop">The property being serialized.</param>
        /// <returns>A sortable string representation for the property.</returns>
        protected virtual string GetPropertyOrder(PropertyInfo prop)
        {
            return prop.Name;
        }

        /// <summary>
        ///   Gets the TypeConverter for the given property or attached property.
        /// </summary>
        /// <param name = "inf">The property or getter for the attached property.</param>
        /// <returns>The TypeConverter to be used for serialization if one exists, otherwise null.</returns>
        protected TypeConverter GetTypeConverter(MemberInfo inf)
        {
            if (inf is MethodInfo)
                return this.GetTypeConverter(inf, ((MethodInfo)inf).ReturnType);
            if (inf is PropertyInfo)
                return this.GetTypeConverter(inf, ((PropertyInfo)inf).PropertyType);
            return null;
        }

        /// <summary>
        ///   Gets an XmlWriter for the given StringBuilder, and can be used to customize formatting settings for the Xaml.
        /// </summary>
        /// <param name = "output">The StringBuilder into which the writer will emit Xaml.</param>
        /// <returns>An XmlWriter to be used for serialization.</returns>
        protected virtual XmlWriter GetXmlWriter(StringBuilder output)
        {
            return XmlWriter.Create(output,
                                    new XmlWriterSettings
                                    {
                                        Indent = true,
                                        NamespaceHandling = NamespaceHandling.OmitDuplicates,
                                        OmitXmlDeclaration = true,
                                        NewLineOnAttributes = false,
                                        CloseOutput = true,
                                    });
        }

        /// <summary>
        ///   Determines whether a type matches the dictionary interface that Xaml supports.
        /// </summary>
        /// <param name = "t">The type to check.</param>
        /// <returns>true if the type is a supported dictionary type, false otherwise.</returns>
        protected static bool IsDictionary(Type t)
        {
#if WINDOWS_PHONE
            return t.Equals(typeof(ResourceDictionary));
#else
            if (t.IsInterface)
                return false;
            var interfaces =
                t.GetInterfaces().Concat(new[] { t }).Where(
                    iface =>
                    iface.Equals(typeof(IDictionary)) ||
                    iface.IsGenericType && iface.GetGenericTypeDefinition().Equals(typeof(IDictionary<,>)));
            return interfaces.FirstOrDefault() != null;
#endif
        }

        /// <summary>
        ///   Determines whether the given property can be written inline (as an attribute) rather than using object-element syntax.
        /// </summary>
        /// <param name = "obj">The object on which the property is being set.</param>
        /// <param name = "propValue">The value of the property being set.</param>
        /// <param name = "inf">The identifier for the property being set (a PropertyInfo for a property, and the getter MethodInfo for an attached property).</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        /// <returns></returns>
        protected virtual bool IsInlinable(object obj,
                                           object propValue,
                                           MemberInfo inf,
                                           ISet<object> cycleCheckObjects)
        {
            TypeConverter tc = GetTypeConverter(inf);
            try
            {
                if (tc != null && tc.CanConvertTo(typeof(string)))
                {
                    tc.ConvertTo(null, EnUsCulture, propValue, typeof(string));
                    return true;
                }
            }
            catch
            {
            }

            if (propValue is string)
            {
                return true;
            }
            if (propValue == null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Determines whether an attached property can be serialized in attribute form rather than using object-element syntax.
        /// </summary>
        /// <param name = "obj">The object on which the attached property is set.</param>
        /// <param name = "propValue">The value of the attached property.</param>
        /// <param name = "meth">The getter for the attached property.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        /// <returns>true if the attached property can be written as an attribute, false otherwise.</returns>
        protected bool IsInlineAttachedProperty(object obj,
                                                object propValue,
                                                MethodInfo meth,
                                                ISet<object> cycleCheckObjects)
        {
            try
            {
                return this.IsInlinable(obj, propValue, meth, cycleCheckObjects);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }

        /// <summary>
        ///   Determines whether a property can be serialized in attribute form rather than using object-element syntax.
        /// </summary>
        /// <param name = "obj">The object on which the property is set.</param>
        /// <param name = "propValue">The value of the property.</param>
        /// <param name = "prop">The property to serialize.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        /// <returns>true if the property can be written as an attribute, false otherwise.</returns>
        protected bool IsInlineProperty(object obj,
                                        object propValue,
                                        PropertyInfo prop,
                                        ISet<object> cycleCheckObjects)
        {
            return this.CanWriteProperty(prop) && this.IsInlinable(obj, propValue, prop, cycleCheckObjects);
        }

        /// <summary>
        ///   Determines whether a type matches the collection interface that Xaml supports.
        /// </summary>
        /// <param name = "t">The type to check.</param>
        /// <returns>true if the type is a supported collection type, false otherwise.</returns>
        protected static bool IsList(Type t)
        {
            if (IsListCache.ContainsKey(t))
                return IsListCache[t];
            if (t.IsInterface && !t.Equals(typeof(IList)))
                return IsListCache[t] = false;
            var interfaces =
                t.GetInterfaces().Concat(new[] { t }).Where(
                    iface =>
                    iface.Equals(typeof(IList)) ||
                    iface.IsGenericType && iface.GetGenericTypeDefinition().Equals(typeof(IList<>)));
            return IsListCache[t] = interfaces.Count() > 0;
        }

        /// <summary>
        ///   Determines whether an object is unique given the items on the stack.
        /// </summary>
        /// <param name = "obj">The object to check.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        /// <returns>true if the object is unique, false otherwise.</returns>
        protected bool IsUnique(object obj, ISet<object> cycleCheckObjects)
        {
            return obj == null || obj.GetType().IsValueType || !cycleCheckObjects.Contains(obj);
        }

        /// <summary>
        ///   Called after serialization (and can be used for cleanup).
        /// </summary>
        /// <param name = "obj">The object being serialized.</param>
        /// <param name = "str">The serialized Xaml, which can be modified for cleanup during this method.</param>
        protected virtual void PostSerialize(object obj, ref string str)
        {
            lock (this._State)
                this._State.Remove(Thread.CurrentThread);
        }

        /// <summary>
        ///   Called before serialization (and can be used for initialization).
        /// </summary>
        /// <param name = "obj">The object being serialized.</param>
        /// <param name = "prefixMappings">The initial dictionary of prefixes, which can be primed for custom serializers.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack, which can be primed if certain objects must be avoided.</param>
        protected virtual void PreSerialize(object obj,
                                            Dictionary<string, string> prefixMappings,
                                            ISet<object> cycleCheckObjects)
        {
            lock (this._State)
                this._State[Thread.CurrentThread] = new SerializeState();
#if WINDOWS_PHONE
            prefixMappings["http://schemas.microsoft.com/client/2007"] = String.Empty;
#endif
        }

        /// <summary>
        ///   Determines whether a property should be serialized (based upon the DefaultAttribute and ShouldSerialize methods).
        /// </summary>
        /// <param name = "obj">The object being serialized.</param>
        /// <param name = "propValue">The value of the property being serialized.</param>
        /// <param name = "prop">The property being serialized.</param>
        /// <returns>true if the property should be serialized, false otherwise.</returns>
        protected virtual bool ShouldSerialize(object obj, object propValue, PropertyInfo prop)
        {
            var defaultValue = DefaultValueCache.ContainsKey(prop)
                                   ? DefaultValueCache[prop]
                                   : DefaultValueCache[prop] =
                                     prop.GetCustomAttributes(typeof(DefaultValueAttribute), false).Cast
                                         <DefaultValueAttribute>().FirstOrDefault();
            if (defaultValue != null && Equals(propValue, defaultValue.Value))
                return false;
            try
            {
                var shouldSerializeMethod = ShouldSerializeMethodCache.ContainsKey(prop)
                                                ? ShouldSerializeMethodCache[prop]
                                                : ShouldSerializeMethodCache[prop] =
                                                  obj.GetType().GetMethod(
                                                      string.Format("ShouldSerialize{0}", prop.Name),
                                                      BindingFlags.Instance | BindingFlags.Public |
                                                      BindingFlags.FlattenHierarchy,
                                                      null,
                                                      new Type[0],
                                                      null);
                if (shouldSerializeMethod != null)
                    return (bool)shouldSerializeMethod.Invoke(obj, new object[0]);
            }
            catch (Exception)
            {
            }
            return true;
        }

        /// <summary>
        ///   Called after all properties that can be written as attributes (rather than in object-element syntax) are written, but before an object-element content is written.  Use this virtual as an opportunity to inject additional attributes before the object is written.
        /// </summary>
        /// <param name = "obj">The object being serialized.</param>
        /// <param name = "writer">The writer being used for serialization.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        protected virtual void VisitAfterAttributes(object obj,
                                                    XmlWriter writer,
                                                    IDictionary<string, string> prefixMappings,
                                                    ISet<object> cycleCheckObjects)
        {
        }

        /// <summary>
        ///   Called immediately after the BeginElement for the object being serialized has been written.
        /// </summary>
        /// <param name = "obj">The object being serialized.</param>
        /// <param name = "writer">The writer being used for serialization.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        protected virtual void VisitAfterBeginElement(object obj,
                                                      XmlWriter writer,
                                                      IDictionary<string, string> prefixMappings,
                                                      ISet<object> cycleCheckObjects)
        {
        }

        /// <summary>
        ///   Called when an object's Content property was not set, allowing special types (e.g. Templates) whose content properties are not discoverable publicly, to be serialized.
        /// </summary>
        /// <param name = "obj">The object being serialized.</param>
        /// <param name = "writer">The writer being used for serialization.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        protected virtual void VisitAlternateContent(object obj,
                                                     XmlWriter writer,
                                                     IDictionary<string, string> prefixMappings,
                                                     ISet<object> cycleCheckObjects)
        {
        }

        /// <summary>
        ///   Serializes an attached property on an object.
        /// </summary>
        /// <param name = "obj">The object on which the attached property is set.</param>
        /// <param name = "propValue">The value of the attached property.</param>
        /// <param name = "propertyName">The name of the attached property.</param>
        /// <param name = "getter">The getter method for the attached property.</param>
        /// <param name = "setter">The setter method for the attached property.</param>
        /// <param name = "writer">The writer being used to serialize the object.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects currently on the stack (for cycle detection).</param>
        protected virtual void VisitAttachedProperty(object obj,
                                                     object propValue,
                                                     string propertyName,
                                                     MethodInfo getter,
                                                     MethodInfo setter,
                                                     XmlWriter writer,
                                                     IDictionary<string, string> prefixMappings,
                                                     ISet<object> cycleCheckObjects)
        {
            try
            {
                string ns = this.GetNamespace(getter.DeclaringType, prefixMappings);
                if (string.Empty.Equals(writer.LookupPrefix(ns)))
                {
                    ns = null;
                }
                if (setter != null)
                {
                    TypeConverter tc = GetTypeConverter(getter);
                    bool convertable = false;
                    string conversion = null;
                    try
                    {
                        if (tc != null && getter.ReturnType.IsInstanceOfType(propValue) &&
                            tc.CanConvertTo(typeof(string)))
                        {
                            conversion = (string)tc.ConvertTo(null, EnUsCulture, propValue, typeof(string));
                            convertable = true;
                        }
                        
                    }
                    catch (Exception)
                    {
                    }
                    string attValue = null;
                    if (convertable)
                        attValue = tc == BuiltIn && !propValue.GetType().Equals(typeof(string)) &&
                                   !propValue.GetType().Equals(typeof(Uri))
                                       ? conversion
                                       : CleanupString(conversion);
                    else if (propValue is string)
                        attValue = CleanupString(propValue as string);
                    else if (propValue == null)
                    {
                        if (writer.LookupPrefix(XamlNamespace) == null)
                            writer.WriteAttributeString("xmlns",
                                                        "x",
                                                        null,
                                                        XamlNamespace);
                        attValue = "{x:Null}";
                    }
                    if (attValue != null)
                    {
                        writer.WriteAttributeString(this.GetPrefix(ns, prefixMappings, writer),
                                                    string.Format("{0}.{1}", getter.DeclaringType.Name, propertyName),
                                                    ns,
                                                    attValue);
                    }
                    else if (this.CanWriteObject(propValue, cycleCheckObjects))
                    {
                        writer.WriteStartElement(this.GetPrefix(ns, prefixMappings, writer),
                                                 string.Format("{0}.{1}", getter.DeclaringType.Name, propertyName),
                                                 ns);
                        this.VisitObject(propValue, writer, prefixMappings, cycleCheckObjects);
                        writer.WriteEndElement();
                    }
                }
                else
                {
                    bool isDictionary = IsDictionary(getter.ReturnType);
                    bool isList = !isDictionary && IsList(getter.ReturnType);
                    if ((isDictionary || isList) && propValue is IEnumerable && !IsEmpty((IEnumerable)propValue))
                    {
                        writer.WriteStartElement(this.GetPrefix(ns, prefixMappings, writer),
                                                 string.Format("{0}.{1}", getter.DeclaringType.Name, propertyName),
                                                 ns);
                        if (isDictionary)
                        {
                            this.VisitDictionaryContents((IDictionary)propValue,
                                                         writer,
                                                         prefixMappings,
                                                         cycleCheckObjects);
                        }
                        else
                        {
                            this.VisitCollectionContents((IEnumerable)propValue,
                                                         writer,
                                                         prefixMappings,
                                                         cycleCheckObjects);
                        }
                        writer.WriteEndElement();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        /// <summary>
        ///   Called immediately before the EndElement for the object being serialized is called.  Can be used for cleanup.
        /// </summary>
        /// <param name = "obj">The object being serialized.</param>
        /// <param name = "writer">The writer being used for serialization.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        protected virtual void VisitBeforeEndElement(object obj,
                                                     XmlWriter writer,
                                                     IDictionary<string, string> prefixMappings,
                                                     ISet<object> cycleCheckObjects)
        {
        }

        /// <summary>
        ///   Serializes the contents of a collection.
        /// </summary>
        /// <param name = "collection">The collection being serialized.</param>
        /// <param name = "writer">The writer being used for serialization.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        protected virtual void VisitCollectionContents(IEnumerable collection,
                                                       XmlWriter writer,
                                                       IDictionary<string, string> prefixMappings,
                                                       ISet<object> cycleCheckObjects)
        {
            try
            {
                foreach (var item in collection)
                    if (this.CanWriteObject(item, cycleCheckObjects))
                        this.VisitObject(item, writer, prefixMappings, cycleCheckObjects);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        /// <summary>
        ///   Serializes the contents of a dictionary.
        /// </summary>
        /// <param name = "dict">The dictionary being serialized.</param>
        /// <param name = "writer">The writer being used for serialization.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        protected virtual void VisitDictionaryContents(IDictionary dict,
                                                       XmlWriter writer,
                                                       IDictionary<string, string> prefixMappings,
                                                       ISet<object> cycleCheckObjects)
        {
            try
            {
                foreach (var dictKey in dict.Keys)
                    if (this.CanWriteObject(dict[dictKey], cycleCheckObjects))
                        this.VisitObject(dict[dictKey], writer, prefixMappings, cycleCheckObjects, key: "" + dictKey);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        /// <summary>
        ///   Serializes a key for an object (usually as "x:Key").  The writer will be in attribute mode.
        /// </summary>
        /// <param name = "key">The key to serialize.</param>
        /// <param name = "writer">The writer being used for serialization.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        protected virtual void VisitKey(object key,
                                        XmlWriter writer,
                                        IDictionary<string, string> prefixMappings,
                                        ISet<object> cycleCheckObjects)
        {
            TypeConverter keyTc = this.GetTypeConverter(key.GetType());
            string keyString = keyTc != null ? (string)keyTc.ConvertTo(null, EnUsCulture, key, typeof(string)) : string.Format("{0}", key);
            writer.WriteAttributeString("x", "Key", XamlNamespace, keyString);
        }

        /// <summary>
        ///   Serializes an object into Xaml.
        /// </summary>
        /// <param name = "obj">The object to serialize.</param>
        /// <param name = "writer">The writer being used for serialization.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection)</param>
        /// <param name = "isRoot">A value indicating whether this object is the root of the document.</param>
        /// <param name = "key">A value that represents the key for this object, or null if the object has no key (and hence is not in a dictionary).</param>
        protected virtual void VisitObject(object obj,
                                           XmlWriter writer,
                                           IDictionary<string, string> prefixMappings,
                                           ISet<object> cycleCheckObjects,
                                           bool isRoot = false,
                                           object key = null)
        {
            try
            {
                Type objType = obj.GetType();
                if (objType.IsNotPublic)
                    return;
                string ns = this.GetNamespace(objType, prefixMappings);
                string prefix = this.GetPrefix(ns, prefixMappings, writer);
                if (prefix.Equals(string.Empty) && writer.LookupPrefix(ns) != null)
                    writer.WriteStartElement(objType.Name);
                else
                    writer.WriteStartElement(prefix, objType.Name, ns);
                if (isRoot)
                    this.VisitRootAttribute(obj, writer, prefixMappings, cycleCheckObjects);

                if (key != null)
                    this.VisitKey(key, writer, prefixMappings, cycleCheckObjects);

                this.VisitAfterBeginElement(obj, writer, prefixMappings, cycleCheckObjects);

                // Try to get the TypeConverter
                TypeConverter tc = this.GetTypeConverter(objType);
                try
                {
                    if (tc != null && tc.CanConvertTo(typeof(string)))
                    {
                        string toWrite = (string)tc.ConvertTo(null, EnUsCulture, obj, typeof(string));
                        if (NeedsSpacePreservation(toWrite))
                            writer.WriteAttributeString("xml", "space", null, "preserve");
                        writer.WriteValue(toWrite);
                        writer.WriteFullEndElement();
                        return;
                    }
                }
                catch
                {
                }

                cycleCheckObjects.Add(obj);
                PropertyInfo contentProp = this.GetContentProperty(obj);

                // If no TypeConverter was found, use OE syntax.
                var props =
                    (from p in
                         PropertiesCache.ContainsKey(objType)
                             ? PropertiesCache[objType]
                             : PropertiesCache[objType] =
                               objType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     where
                         !this.PropertiesToSkip.Contains(p) &&
                         (this.CanWriteProperty(p) || IsList(p.PropertyType) || IsDictionary(p.PropertyType)) &&
                         p.GetIndexParameters().Length == 0
                     let value = TryCall(() => GetPropertyGetter(p)(obj))
                     where value != NotSetObject && this.ShouldSerialize(obj, value, p)
                     orderby this.GetPropertyOrder(p)
                     select
                         new
                         {
                             Property = p,
                             Value = value,
                             IsInline = this.IsInlineProperty(obj, value, p, cycleCheckObjects)
                         }).ToArray();
                var attachedProps = (from ap in this.GetAttachedProperties(objType)
                                     let value = TryCall(() => ap.GetterFunc(obj))
                                     where value != NotSetObject
                                     orderby this.GetAttachedOrder(ap)
                                     select
                                         new
                                         {
                                             Property = ap,
                                             Value = value,
                                             IsInline =
                                     this.IsInlineAttachedProperty(obj, value, ap.Getter, cycleCheckObjects)
                                         }).ToArray();
                bool foundContent = false;
                foreach (var prop in props.Where(p => p.IsInline))
                {
                    
                        try
                        {
                            this.VisitProperty(obj,
                                               prop.Value,
                                               prop.Property,
                                               false,
                                               writer,
                                               prefixMappings,
                                               cycleCheckObjects);
                        }
                        catch
                        {
                        }
                    
                }
                foreach (var ap in attachedProps.Where(p => p.IsInline))
                {
                    try
                    {
                        this.VisitAttachedProperty(obj,
                                                   ap.Value,
                                                   ap.Property.Name,
                                                   ap.Property.Getter,
                                                   ap.Property.Setter,
                                                   writer,
                                                   prefixMappings,
                                                   cycleCheckObjects);
                    }
                    catch
                    {
                    }
                }
                this.VisitAfterAttributes(obj, writer, prefixMappings, cycleCheckObjects);
                foreach (var prop in props.Where(p => !p.IsInline))
                {
                    if (prop.Property == contentProp)
                        foundContent = true;
                    try
                    {
                        this.VisitProperty(obj,
                                           prop.Value,
                                           prop.Property,
                                           prop.Property == contentProp,
                                           writer,
                                           prefixMappings,
                                           cycleCheckObjects);
                    }
                    catch
                    {
                    }
                }
                foreach (var ap in attachedProps.Where(p => !p.IsInline))
                {
                    try
                    {
                        this.VisitAttachedProperty(obj,
                                                   ap.Value,
                                                   ap.Property.Name,
                                                   ap.Property.Getter,
                                                   ap.Property.Setter,
                                                   writer,
                                                   prefixMappings,
                                                   cycleCheckObjects);
                    }
                    catch
                    {
                    }
                }

                if (IsDictionary(obj.GetType()))
                {
                    foundContent = true;
                    this.VisitDictionaryContents((IDictionary)obj, writer, prefixMappings, cycleCheckObjects);
                }
                else if (IsList(obj.GetType()))
                {
                    foundContent = true;
                    this.VisitCollectionContents((IEnumerable)obj, writer, prefixMappings, cycleCheckObjects);
                }

                if (!foundContent)
                    this.VisitAlternateContent(obj, writer, prefixMappings, cycleCheckObjects);

                this.VisitBeforeEndElement(obj, writer, prefixMappings, cycleCheckObjects);
                cycleCheckObjects.Remove(obj);
                writer.WriteEndElement();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        /// <summary>
        ///   Serializes a property.
        /// </summary>
        /// <param name = "obj">The object on which the property is set.</param>
        /// <param name = "propValue">The value of the property.</param>
        /// <param name = "prop">The property being set.</param>
        /// <param name = "isContentProperty">A value indicating that the property is the ContentProperty for the object, and thus no property elements need to be generated.</param>
        /// <param name = "writer">The writer being used for serialization.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        protected virtual void VisitProperty(object obj,
                                             object propValue,
                                             PropertyInfo prop,
                                             bool isContentProperty,
                                             XmlWriter writer,
                                             IDictionary<string, string> prefixMappings,
                                             ISet<object> cycleCheckObjects)
        {
            try
            {
                //if I can serialize this property
                if (!CanSerializeProperty(prop))
                {
                    return;
                }

                string ns = this.GetNamespace(obj.GetType(), prefixMappings);
                if (this.CanWriteProperty(prop))
                {
                    TypeConverter tc = GetTypeConverter(prop);
                    bool convertable = false;
                    string conversion = null;
                    try
                    {
                        if (tc != null && prop.PropertyType.IsInstanceOfType(propValue) &&
                            tc.CanConvertTo(typeof(string)))
                        {
                            conversion = (string)tc.ConvertTo(null, EnUsCulture, propValue, typeof(string));
                            convertable = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    string attValue = null;
                    if (convertable)
                        attValue = tc == BuiltIn && !propValue.GetType().Equals(typeof(string)) &&
                                   !propValue.GetType().Equals(typeof(Uri))
                                       ? conversion
                                       : CleanupString(conversion);
                    else if (propValue is string)
                        attValue = CleanupString(propValue as string);
                    else if (propValue == null)
                    {
                        if (writer.LookupPrefix(XamlNamespace) == null)
                            writer.WriteAttributeString("xmlns",
                                                        "x",
                                                        null,
                                                        XamlNamespace);
                        attValue = "{x:Null}";
                    }
                    if (attValue != null)
                        writer.WriteAttributeString(prop.Name, attValue);
                    else if (this.CanWriteObject(propValue, cycleCheckObjects))
                    {
                        if (!isContentProperty)
                            writer.WriteStartElement(obj.GetType().Name + "." + prop.Name, ns);
                        this.VisitObject(propValue, writer, prefixMappings, cycleCheckObjects);
                        if (!isContentProperty)
                            writer.WriteEndElement();
                    }
                }

                //Checks whether the property is generic and visits the property if it is the case
                ProcessGenericProperty(obj, propValue, prop, isContentProperty, writer, prefixMappings, cycleCheckObjects, ns);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void ProcessGenericProperty(object obj, object propValue, PropertyInfo prop, bool isContentProperty, XmlWriter writer, IDictionary<string, string> prefixMappings, ISet<object> cycleCheckObjects, string ns)
        {
            bool isDictionary = IsDictionary(prop.PropertyType);
            bool isList = !isDictionary && IsList(prop.PropertyType);
            if ((isDictionary || isList) && propValue is IEnumerable && !IsEmpty((IEnumerable)propValue))
            {
                if (!isContentProperty)
                    writer.WriteStartElement(obj.GetType().Name + "." + prop.Name, ns);
                if (isDictionary)
                    this.VisitDictionaryContents((IDictionary)propValue,
                                                 writer,
                                                 prefixMappings,
                                                 cycleCheckObjects);
                else
                    this.VisitCollectionContents((IEnumerable)propValue,
                                                 writer,
                                                 prefixMappings,
                                                 cycleCheckObjects);
                if (!isContentProperty)
                    writer.WriteEndElement();
            }
        }

        /// <summary>
        ///   Called during attribute serialization on the root object.  Can be used for global namespace declaration.
        /// </summary>
        /// <param name = "obj">The root object.</param>
        /// <param name = "writer">The writer being used for serialization.</param>
        /// <param name = "prefixMappings">A mapping of xml namespaces to prefixes.</param>
        /// <param name = "cycleCheckObjects">The set of objects on the stack (for cycle detection).</param>
        protected virtual void VisitRootAttribute(object obj,
                                                  XmlWriter writer,
                                                  IDictionary<string, string> prefixMappings,
                                                  ISet<object> cycleCheckObjects)
        {
#if WINDOWS_PHONE
            writer.WriteAttributeString("xmlns", null, null, "http://schemas.microsoft.com/client/2007");
#endif
            writer.WriteAttributeString("xmlns", "x", null, XamlNamespace);
        }

        private static string CleanupString(string str)
        {
            if (str.Length > 0 && str[0] == '{')
                return "{}" + str;
            return str;
        }

        private TypeConverter GetTypeConverter(Type t)
        {
            if (this.State.FoundTCs.ContainsKey(t))
                return this.State.FoundTCs[t];
            IEnumerable<TypeConverterAttribute> tcAtts =
                t.GetCustomAttributes(typeof(TypeConverterAttribute), false).Cast<TypeConverterAttribute>();
            if (tcAtts.Count() > 0)
            {
                try
                {
                    TypeConverter conv =
                        Activator.CreateInstance(Type.GetType(tcAtts.First().ConverterTypeName)) as TypeConverter;
                    if (conv != null && conv.CanConvertTo(typeof(string)))
                        return this.State.FoundTCs[t] = conv;
                }
                catch
                {
                    return this.State.FoundTCs[t] = null;
                }
            }
            if (BuiltInTypeConverter.IsSupportedType(t))
                return this.State.FoundTCs[t] = BuiltIn;
            return this.State.FoundTCs[t] = this.TypeConverters.ContainsKey(t) ? this.TypeConverters[t] : null;
        }

        private TypeConverter GetTypeConverter(MemberInfo inf, Type returnType)
        {
            Tuple<Type, Type, string> tuple = new Tuple<Type, Type, string>(returnType, inf.DeclaringType, inf.Name);
            if (this.State.FoundPropTCs.ContainsKey(tuple))
                return this.State.FoundPropTCs[tuple];
            IEnumerable<TypeConverterAttribute> tcAtts =
                inf.GetCustomAttributes(typeof(TypeConverterAttribute), false).Cast<TypeConverterAttribute>();
            if (tcAtts.Count() == 0)
            {
                if (BuiltInTypeConverter.IsSupportedType(returnType))
                    return this.State.FoundPropTCs[tuple] = BuiltIn;
                return this.State.FoundPropTCs[tuple] = this.GetTypeConverter(returnType);
            }
            try
            {
                return
                    this.State.FoundPropTCs[tuple] =
                    Activator.CreateInstance(Type.GetType(tcAtts.First().ConverterTypeName)) as TypeConverter;
            }
            catch
            {
                return this.State.FoundPropTCs[tuple] = this.GetTypeConverter(returnType);
            }
        }

        private static bool IsEmpty(IEnumerable en)
        {
            if (en is ICollection)
                return ((ICollection)en).Count == 0;
            return !en.GetEnumerator().MoveNext();
        }

        private static bool NeedsSpacePreservation(string input)
        {
            Regex spaceChecker = new Regex(@"\s\s");
            return spaceChecker.IsMatch(input);
        }

        private void PrepareDictionary(Type t)
        {
            if (this.AttachedProperties.ContainsKey(t))
                return;
            this.AttachedProperties[t] = new List<AttachedProperty>();
        }

        private static object TryCall(Func<object> toCall)
        {
            try
            {
                return toCall();
            }
            catch
            {
                return NotSetObject;
            }
        }




        /// <summary>
        ///   Represents an attached property.
        /// </summary>
        protected class AttachedProperty
        {
            #region Properties (6)

            /// <summary>
            ///   The getter method for the attached property.
            /// </summary>
            public MethodInfo Getter { get; set; }

            /// <summary>
            ///   A reflection-emitted delegate for getting the value of the attached property (for performance enhancements).
            /// </summary>
            public Func<object, object> GetterFunc { get; private set; }

            /// <summary>
            ///   The name of the attached property.
            /// </summary>
            public string Name
            {
                get { return this.Getter.Name.Substring(3); }
            }

            /// <summary>
            ///   The setter method for the attached property (if any).
            /// </summary>
            public MethodInfo Setter { get; set; }

            /// <summary>
            ///   The type that the attached property targets.
            /// </summary>
            public Type TargetType
            {
                get { return this.Getter.GetParameters()[0].ParameterType; }
            }

            /// <summary>
            ///   The return value of the attached property.
            /// </summary>
            public Type ValueType
            {
                get { return this.Getter.ReturnType; }
            }

            #endregion Properties

            #region Methods (2)

            // Public Methods (2) 

            /// <summary>
            ///   Returns a <see cref = "T:System.String" /> that represents the current <see cref = "T:System.Object" />.
            /// </summary>
            /// <returns>
            ///   A <see cref = "T:System.String" /> that represents the current <see cref = "T:System.Object" />.
            /// </returns>
            public override string ToString()
            {
                return string.Format("{0} {1}.{2}({3})",
                                     this.ValueType,
                                     this.Getter.DeclaringType,
                                     this.Name,
                                     this.TargetType);
            }

            /// <summary>
            ///   Validates the attached property, checking to see that the getter and setter meet all of the requirements for a serializable attached property.
            /// </summary>
            /// <returns>true if the attached property is valid, otherwise false.</returns>
            public bool Validate()
            {
                if (this.Getter == null)
                    return false;
                if (this.Getter.DeclaringType.DeclaringType != null)
                    return false;
                if (!this.Getter.DeclaringType.IsPublic)
                    return false;
                if (this.Setter == null)
                {
                    // Check that the Getter is static
                    if (!this.Getter.IsStatic)
                        return false;

                    // Check that the Getter has an appropriate name
                    if (!this.Getter.Name.StartsWith("Get"))
                        return false;

                    // Check that the parameter count is correct
                    if (this.Getter.GetParameters().Length != 1)
                        return false;

                    // Check that the return type is a List or Dictionary
                    if (!(IsList(this.Getter.ReturnType) || IsDictionary(this.Getter.ReturnType)))
                        return false;
                }
                else
                {
                    // Check that both Getter and Setter are static methods
                    if (!this.Getter.IsStatic || !this.Setter.IsStatic)
                        return false;

                    // Check that both the Getter and Setter are on the same class
                    if (!this.Getter.DeclaringType.Equals(this.Setter.DeclaringType))
                        return false;

                    // Check that the method names match
                    if (!this.Getter.Name.StartsWith("Get") || !this.Setter.Name.StartsWith("Set") ||
                        !this.Getter.Name.Substring(3).Equals(this.Setter.Name.Substring(3)))
                        return false;

                    // Check that the parameter counts are correct
                    if (this.Getter.GetParameters().Length != 1 || this.Setter.GetParameters().Length != 2)
                        return false;

                    // Check that the target and value types match
                    if (
                        !this.Getter.GetParameters()[0].ParameterType.Equals(
                            this.Setter.GetParameters()[0].ParameterType))
                        return false;

                    if (!this.Getter.ReturnType.Equals(this.Setter.GetParameters()[1].ParameterType))
                        return false;
                }
#if WINDOWS_PHONE
                if (GetDependencyProperty(Getter) == null)
                    return false;
                this.GetterFunc = (obj) => this.Getter.Invoke(null, new object[] { obj });
#else
                var param = System.Linq.Expressions.Expression.Parameter(typeof(object));
                var cast = System.Linq.Expressions.Expression.Convert(param, this.TargetType);
                var call = System.Linq.Expressions.Expression.Call(null, this.Getter, cast);
                var finalCast = System.Linq.Expressions.Expression.Convert(call, typeof(object));
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object>>(finalCast, param);
                this.GetterFunc = lambda.Compile();
#endif
                return true;
            }

            #endregion Methods

#if WINDOWS_PHONE
            private DependencyProperty GetDependencyProperty(MethodInfo meth)
            {
                return this.GetDependencyProperty(meth.Name.Substring(3), meth.DeclaringType);
            }

            private DependencyProperty GetDependencyProperty(string propName, Type declaringType)
            {
                var tuple = new Tuple<Type, string>(declaringType, propName);
                FieldInfo dpField = declaringType.GetField(propName + "Property",
                                                           BindingFlags.Public | BindingFlags.Static |
                                                           BindingFlags.FlattenHierarchy);
                if (dpField == null)
                    return null;
                if (dpField.FieldType.Equals(typeof(DependencyProperty)) ||
                    dpField.FieldType.IsSubclassOf(typeof(DependencyProperty)))
                    return dpField.GetValue(null) as DependencyProperty;
                return null;
            }
#endif
        }
        private class BuiltInTypeConverter : TypeConverter
        {
            #region Fields (1)

            private static readonly Type[] SupportedTypes = new[] { typeof(int), typeof(double), typeof(bool), typeof(string), typeof(Uri),typeof(Decimal),typeof(DateTime) };

            #endregion Fields

            #region Methods (3)

            // Public Methods (3) 

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(string);
            }

            public override object ConvertTo(ITypeDescriptorContext context,
                                             CultureInfo culture,
                                             object value,
                                             Type destinationType)
            {
                Type fromType = value.GetType();
                if (SupportedTypes.Contains(fromType))
                    return string.Format(culture, "{0}", value);
                if (fromType.IsEnum)
                {
                    string result = string.Format(culture, "{0}", value);
                    result.Replace("|", ",");
                    return result;
                }
                return value;
            }

            public static bool IsSupportedType(Type t)
            {
                return t.IsEnum || SupportedTypes.Contains(t);
            }

            #endregion Methods
        }
        /// <summary>
        ///   Implements an IEqualityComparer based upon delegates.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        protected class DelegateEqualityComparer<T> : EqualityComparer<T>
        {
            #region Fields (2)

            private readonly Func<T, T, bool> _Equals;
            private readonly Func<T, int> _Hash;

            #endregion Fields

            #region Constructors (1)

            /// <summary>
            ///   Constructs a DelegateEqualityComparer.
            /// </summary>
            /// <param name = "equals">The delegate to use to check equality.</param>
            /// <param name = "hash">The delegate to use to retrieve the hashcode for the object.</param>
            public DelegateEqualityComparer(Func<T, T, bool> equals, Func<T, int> hash)
            {
                this._Equals = equals;
                this._Hash = hash;
            }

            #endregion Constructors

            #region Methods (2)

            // Public Methods (2) 

            /// <summary>
            ///   When overridden in a derived class, determines whether two objects of type T are equal.
            /// </summary>
            /// <returns>
            ///   true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <param name = "x">The first object to compare.</param>
            /// <param name = "y">The second object to compare.</param>
            public override bool Equals(T x, T y)
            {
                return this._Equals(x, y);
            }

            /// <summary>
            ///   When overridden in a derived class, serves as a hash function for the specified object for hashing algorithms and data structures, such as a hash table.
            /// </summary>
            /// <returns>
            ///   A hash code for the specified object.
            /// </returns>
            /// <param name = "obj">The object for which to get a hash code.</param>
            /// <exception cref = "T:System.ArgumentNullException">The type of <paramref name = "obj" /> is a reference type and <paramref name = "obj" /> is null.</exception>
            public override int GetHashCode(T obj)
            {
                return this._Hash(obj);
            }

            #endregion Methods
        }
        private class SerializeState
        {
            #region Constructors (1)

            public SerializeState()
            {
                this.LoadedAsms = new HashSet<Assembly>();
                this.NsMappings = new Dictionary<Type, string>();
                this.FoundPropTCs = new Dictionary<Tuple<Type, Type, string>, TypeConverter>();
                this.FoundTCs = new Dictionary<Type, TypeConverter>();
                this.AttachedPropertiesCache = new Dictionary<Type, IEnumerable<AttachedProperty>>();
            }

            #endregion Constructors

            #region Properties (5)

            public Dictionary<Type, IEnumerable<AttachedProperty>> AttachedPropertiesCache { get; private set; }

            public Dictionary<Tuple<Type, Type, string>, TypeConverter> FoundPropTCs { get; private set; }

            public Dictionary<Type, TypeConverter> FoundTCs { get; private set; }

            public HashSet<Assembly> LoadedAsms { get; private set; }

            public Dictionary<Type, string> NsMappings { get; private set; }

            #endregion Properties
        }
    }
}
