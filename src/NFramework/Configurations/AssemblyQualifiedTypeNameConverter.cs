using System;
using System.ComponentModel;
using System.Configuration;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Configurations {
    /// <summary>
    /// Represents a configuration converter that converts a string to <see cref="Type"/> based on a fully qualified name.
    /// </summary>
    public class AssemblyQualifiedTypeNameConverter : ConfigurationConverterBase {
        /// <summary>
        /// Returns the assembly qualified name for the passed in Type.
        /// </summary>
        /// <param name="context">The container representing this System.ComponentModel.TypeDescriptor.</param>
        /// <param name="culture">Culture info for assembly</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destinationType">Type to convert to.</param>
        /// <returns>Assembly Qualified Name as a string</returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value,
                                         Type destinationType) {
            if(value != null) {
                var typeValue = value as Type;

                Guard.Assert(() => typeValue != null);

                if(typeValue != null)
                    return (typeValue).AssemblyQualifiedName;
            }
            return null;
        }

        /// <summary>
        /// Returns a type based on the assembly qualified name passed in as data.
        /// </summary>
        /// <param name="context">The container representing this System.ComponentModel.TypeDescriptor.</param>
        /// <param name="culture">Culture info for assembly.</param>
        /// <param name="value">Data to convert.</param>
        /// <returns>Type of the data</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
            var stringValue = (string)value;

            if(stringValue.IsNotWhiteSpace()) {
                var result = Type.GetType(stringValue, false);
                result.ShouldNotBeNull("result");

                return result;
            }
            return null;
        }
    }
}