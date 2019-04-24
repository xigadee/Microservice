using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This is the base description for the class.
    /// </summary>
    public abstract class DescriptionBase
    {
        /// <summary>
        /// This method sets the first attribute from the list specified.
        /// </summary>
        /// <param name="attrs">The attribute list.</param>
        /// <param name="defaultValue">The optional default value.</param>
        protected void SetValues(IEnumerable<Attribute> attrs, string defaultValue = null)
        {
            var attr = attrs.FirstOrDefault((a) => a is DescriptionAttribute || a is LocalizedDescriptionAttribute) as DescriptionAttribute;
            SetValues(attr, defaultValue);
        }
        /// <summary>
        /// This method sets the specific values from the attribute.
        /// </summary>
        /// <param name="attr">The optional attribute.</param>
        /// <param name="defaultValue">The optional default value.</param>
        protected void SetValues(DescriptionAttribute attr, string defaultValue = null)
        {
            Description = attr?.Description ?? defaultValue;
            if (attr != null && attr is LocalizedDescriptionAttribute)
            {
                var attrs = attr as LocalizedDescriptionAttribute;
                DescriptionLanguage = attrs.Language;
                DescriptionTranslationId = attrs.TranslationId;
            }
        }
        /// <summary>
        /// The description, as set by the description attribute, or the EnumName if this is not defined.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// The translation id code. This can be anything that you wish to specify.
        /// </summary>
        public string DescriptionTranslationId { get; private set; }
        /// <summary>
        /// The optional code description language, i.e. en-us, fr-fr etc.
        /// </summary>
        public string DescriptionLanguage { get; private set; }
    }

    /// <summary>
    /// This class contains a list of items derived from an enumeration definition.
    /// </summary>
    [DebuggerDisplay("{Name}/{UnderlyingType}=>'{Description},{DescriptionTranslationId},{DescriptionLanguage}' BitWise={IsBitwise} Valid={UTCValidUntil}")]
    public class EnumDefinition: DescriptionBase
    {
        #region Constructor
        /// <summary>
        /// This is the base constructor, where you pass in the enum to get the value.
        /// </summary>
        /// <param name="eType">The enumeration type.</param>
        /// <param name="validity">This is the length of time that the data can be cached.</param>
        public EnumDefinition(Type eType, TimeSpan? validity = null)
        {
            if (!eType.IsEnum)
                throw new ArgumentOutOfRangeException($"{eType.Name} is incorrect. Please check the IsEnum property is set.");

            IsBitwise = eType.IsDefined(typeof(FlagsAttribute));
            Name = eType.Name;
            UnderlyingType = Enum.GetUnderlyingType(eType).Name;

            Items = eType
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(fieldInfo => new Item(eType, fieldInfo))
                .ToList();

            if (validity.HasValue)
                UTCValidUntil = DateTime.UtcNow.Add(validity.Value);

            SetValues(eType.GetCustomAttributes());
        }
        #endregion

        /// <summary>
        /// Identifies whether this enumeration can be combined.
        /// </summary>
        public bool IsBitwise { get; }
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The underlying type, int, string etc.
        /// </summary>
        public string UnderlyingType { get; }
        /// <summary>
        /// This is the validity time that the data can be cached for.
        /// </summary>
        public DateTime? UTCValidUntil { get; }
        /// <summary>
        /// This list of value.
        /// </summary>
        public IEnumerable<Item> Items { get; }
        /// <summary>
        /// This is the specific enumeration value.
        /// </summary>
        [DebuggerDisplay("{Id}  {EnumName}  {Value} '{Description},{DescriptionTranslationId},{DescriptionLanguage}'")]
        public class Item: DescriptionBase
        {
            internal Item(Type enumType, FieldInfo fieldInfo)
            {
                EnumName = fieldInfo.Name;
                Value = Enum.Parse(enumType, EnumName);
                Id = Convert.ChangeType(Value, Enum.GetUnderlyingType(enumType)).ToString();

                SetValues(fieldInfo.GetCustomAttributes(), EnumName);
            }
            /// <summary>
            /// The field name.
            /// </summary>
            public string EnumName { get; }
            /// <summary>
            /// The underlying type.
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// The specific value.
            /// </summary>
            public object Value { get; }
        }    
    }

    /// <summary>
    /// This extended attribute is used to add a specific translation Id to the description.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="description">The standard attribute description.</param>
        /// <param name="transId">The translation code id. This can be used to match the description to a specific list on a translation mapping.</param>
        /// <param name="lang">The language of the attribute description.</param>
        public LocalizedDescriptionAttribute(string description, string transId = null, string lang = null) : base(description)
        {
            TranslationId = transId;
            Language = lang;
        }
        /// <summary>
        /// The translation id code. This can be anything that you wish to specify.
        /// </summary>
        public string TranslationId { get; }
        /// <summary>
        /// The optional code description language, i.e. en-us, fr-fr etc.
        /// </summary>
        public string Language { get; }
    }
}
