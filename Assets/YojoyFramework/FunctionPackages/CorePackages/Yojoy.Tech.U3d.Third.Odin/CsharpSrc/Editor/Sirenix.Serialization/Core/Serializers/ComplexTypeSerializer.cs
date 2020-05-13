#pragma warning disable
//-----------------------------------------------------------------------\n// <copyright file="ComplexTypeSerializer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
    using System;
    using Utilities;

    /// <summary>
    /// Serializer for all complex types; IE, types which are not primitives as determined by the <see cref="FormatterUtilities.IsPrimitiveType(Type)" /> method.
    /// </summary>
    /// <typeparam name="T">The type which the <see cref="ComplexTypeSerializer{T}" /> can serialize and deserialize.</typeparam>
    /// <seealso cref="Serializer{T}" />
    public sealed class ComplexTypeSerializer<T> : Serializer<T>
    {
        private static readonly bool ComplexTypeMayBeBoxedValueType = typeof(T).IsInterface || typeof(T) == typeof(object) || typeof(T) == typeof(ValueType) || typeof(T) == typeof(Enum);
        private static readonly bool ComplexTypeIsAbstract = typeof(T).IsAbstract || typeof(T).IsInterface;
        private static readonly bool ComplexTypeIsNullable = typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>);
        private static readonly bool ComplexTypeIsValueType = typeof(T).IsValueType;

        private static readonly bool AllowDeserializeInvalidDataForT = typeof(T).IsDefined(typeof(AllowDeserializeInvalidDataAttribute), true);

        /// <summary>
        /// Reads a value of type <see cref="T" />.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns>
        /// The value which has been read.
        /// </returns>
        public override T ReadValue(IDataReader reader)
        {
            var context = reader.Context;

            if (context.Config.SerializationPolicy.AllowNonSerializableTypes == false && typeof(T).IsSerializable == false)
            {
                context.Config.DebugContext.LogError("The type " + typeof(T).Name + " is not marked as serializable.");
                return default(T);
            }

            bool exitNode = true;

            string name;
            var entry = reader.PeekEntry(out name);

            if (ComplexTypeIsValueType)
            {
                if (entry == EntryType.Null)
                {
                    context.Config.DebugContext.LogWarning("Expecting complex struct of type " + typeof(T).GetNiceFullName() + " but got null value.");
                    reader.ReadNull();
                    return default(T);
                }
                else if (entry != EntryType.StartOfNode)
                {
                    context.Config.DebugContext.LogWarning("Unexpected entry '" + name + "' of type " + entry.ToString() + ", when " + EntryType.StartOfNode + " was expected. A value has likely been lost.");
                    reader.SkipEntry();
                    return default(T);
                }

                try
                {
                    Type expectedType = typeof(T);
                    Type serializedType;

                    if (reader.EnterNode(out serializedType))
                    {
                        if (serializedType != expectedType)
                        {
                            if (serializedType != null)
                            {
                                context.Config.DebugContext.LogWarning("Expected complex struct value " + expectedType.Name + " but the serialized value is of type " + serializedType.Name + ".");

                                if (serializedType.IsCastableTo(expectedType))
                                {
                                    object value = FormatterLocator.GetFormatter(serializedType, context.Config.SerializationPolicy).Deserialize(reader);

                                    bool serializedTypeIsNullable = serializedType.IsGenericType && serializedType.GetGenericTypeDefinition() == typeof(Nullable<>);
                                    bool allowCastMethod = !ComplexTypeIsNullable && !serializedTypeIsNullable;

                                    var castMethod = allowCastMethod ? serializedType.GetCastMethodDelegate(expectedType) : null;

                                    if (castMethod != null)
                                    {
                                        return (T)castMethod(value);
                                    }
                                    else
                                    {
                                        return (T)value;
                                    }
                                }
                                else if (AllowDeserializeInvalidDataForT || reader.Context.Config.AllowDeserializeInvalidData)
                                {
                                    context.Config.DebugContext.LogWarning("Can't cast serialized type " + serializedType.Name + " into expected type " + expectedType.Name + ". Attempting to deserialize with possibly invalid data. Value may be lost or corrupted for node '" + name + "'.");
                                    return FormatterLocator.GetFormatter<T>(context.Config.SerializationPolicy).Deserialize(reader);
                                }
                                else
                                {
                                    context.Config.DebugContext.LogWarning("Can't cast serialized type " + serializedType.Name + " into expected type " + expectedType.Name + ". Value lost for node '" + name + "'.");
                                    return default(T);
                                }
                            }
                            else if (AllowDeserializeInvalidDataForT || reader.Context.Config.AllowDeserializeInvalidData)
                            {
                                context.Config.DebugContext.LogWarning("Expected complex struct value " + expectedType.Name + " but the serialized type could not be resolved. Attempting to deserialize with possibly invalid data. Value may be lost or corrupted for node '" + name + "'.");
                                return FormatterLocator.GetFormatter<T>(context.Config.SerializationPolicy).Deserialize(reader);
                            }
                            else
                            {
                                context.Config.DebugContext.LogWarning("Expected complex struct value " + expectedType.Name + " but the serialized type could not be resolved. Value lost for node '" + name + "'.");
                                return default(T);
                            }
                        }
                        else
                        {
                            return FormatterLocator.GetFormatter<T>(context.Config.SerializationPolicy).Deserialize(reader);
                        }
                    }
                    else
                    {
                        context.Config.DebugContext.LogError("Failed to enter node '" + name + "'.");
                        return default(T);
                    }
                }
                catch (SerializationAbortException ex)
                {
                    exitNode = false;
                    throw ex;
                }
                catch (Exception ex)
                {
                    context.Config.DebugContext.LogException(ex);
                    return default(T);
                }
                finally
                {
                    if (exitNode)
                    {
                        reader.ExitNode();
                    }
                }
            }
            else
            {
                switch (entry)
                {
                    case EntryType.Null:
                        {
                            reader.ReadNull();
                            return default(T);
                        }

                    case EntryType.ExternalReferenceByIndex:
                        {
                            int index;
                            reader.ReadExternalReference(out index);

                            object value = context.GetExternalObject(index);

                            try
                            {
                                return (T)value;
                            }
                            catch (InvalidCastException)
                            {
                                context.Config.DebugContext.LogWarning("Can't cast external reference type " + value.GetType().Name + " into expected type " + typeof(T).Name + ". Value lost for node '" + name + "'.");
                                return default(T);
                            }
                        }

                    case EntryType.ExternalReferenceByGuid:
                        {
                            Guid guid;
                            reader.ReadExternalReference(out guid);

                            object value = context.GetExternalObject(guid);

                            try
                            {
                                return (T)value;
                            }
                            catch (InvalidCastException)
                            {
                                context.Config.DebugContext.LogWarning("Can't cast external reference type " + value.GetType().Name + " into expected type " + typeof(T).Name + ". Value lost for node '" + name + "'.");
                                return default(T);
                            }
                        }

                    case EntryType.ExternalReferenceByString:
                        {
                            string id;
                            reader.ReadExternalReference(out id);

                            object value = context.GetExternalObject(id);

                            try
                            {
                                return (T)value;
                            }
                            catch (InvalidCastException)
                            {
                                context.Config.DebugContext.LogWarning("Can't cast external reference type " + value.GetType().Name + " into expected type " + typeof(T).Name + ". Value lost for node '" + name + "'.");
                                return default(T);
                            }
                        }

                    case EntryType.InternalReference:
                        {
                            int id;
                            reader.ReadInternalReference(out id);

                            object value = context.GetInternalReference(id);

                            try
                            {
                                return (T)value;
                            }
                            catch (InvalidCastException)
                            {
                                context.Config.DebugContext.LogWarning("Can't cast internal reference type " + value.GetType().Name + " into expected type " + typeof(T).Name + ". Value lost for node '" + name + "'.");
                                return default(T);
                            }
                        }

                    case EntryType.StartOfNode:
                        {
                            try
                            {
                                Type expectedType = typeof(T);
                                Type serializedType;
                                int id;

                                if (reader.EnterNode(out serializedType))
                                {
                                    id = reader.CurrentNodeId;

                                    T result;

                                    if (serializedType != null && expectedType != serializedType) // We have type metadata different from the expected type
                                    {
                                        bool success = false;
                                        var isPrimitive = FormatterUtilities.IsPrimitiveType(serializedType);

                                        bool assignableCast;

                                        if (ComplexTypeMayBeBoxedValueType && isPrimitive)
                                        {
                                            // It's a boxed primitive type, so simply read that straight and register success
                                            var serializer = Serializer.Get(serializedType);
                                            result = (T)serializer.ReadValueWeak(reader);
                                            success = true;
                                        }
                                        else if ((assignableCast = expectedType.IsAssignableFrom(serializedType)) || serializedType.HasCastDefined(expectedType, false))
                                        {
                                            try
                                            {
                                                object value;

                                                if (isPrimitive)
                                                {
                                                    var serializer = Serializer.Get(serializedType);
                                                    value = serializer.ReadValueWeak(reader);
                                                }
                                                else
                                                {
                                                    var alternateFormatter = FormatterLocator.GetFormatter(serializedType, context.Config.SerializationPolicy);
                                                    value = alternateFormatter.Deserialize(reader);
                                                }

                                                if (assignableCast)
                                                {
                                                    result = (T)value;
                                                }
                                                else
                                                {
                                                    var castMethod = serializedType.GetCastMethodDelegate(expectedType);

                                                    if (castMethod != null)
                                                    {
                                                        result = (T)castMethod(value);
                                                    }
                                                    else
                                                    {
                                                        // Let's just give it a go anyways
                                                        result = (T)value;
                                                    }
                                                }

                                                success = true;
                                            }
                                            catch (SerializationAbortException ex)
                                            {
                                                exitNode = false;
                                                throw ex;
                                            }
                                            catch (InvalidCastException)
                                            {
                                                success = false;
                                                result = default(T);
                                            }
                                        }
                                        else if (!ComplexTypeIsAbstract && (AllowDeserializeInvalidDataForT || reader.Context.Config.AllowDeserializeInvalidData))
                                        {
                                            // We will try to deserialize an instance of T with the invalid data.
                                            context.Config.DebugContext.LogWarning("Can't cast serialized type " + serializedType.Name + " into expected type " + expectedType.Name + ". Attempting to deserialize with invalid data. Value may be lost or corrupted for node '" + name + "'.");
                                            result = FormatterLocator.GetFormatter<T>(context.Config.SerializationPolicy).Deserialize(reader);
                                            success = true;
                                        }
                                        else
                                        {
                                            // We couldn't cast or use the type, but we still have to deserialize it and register
                                            // the reference so the reference isn't lost if it is referred to further down
                                            // the data stream.

                                            var alternateFormatter = FormatterLocator.GetFormatter(serializedType, context.Config.SerializationPolicy);
                                            object value = alternateFormatter.Deserialize(reader);

                                            if (id >= 0)
                                            {
                                                context.RegisterInternalReference(id, value);
                                            }

                                            result = default(T);
                                        }

                                        if (!success)
                                        {
                                            // We can't use this
                                            context.Config.DebugContext.LogWarning("Can't cast serialized type " + serializedType.Name + " into expected type " + expectedType.Name + ". Value lost for node '" + name + "'.");
                                            result = default(T);
                                        }
                                    }
                                    else if (ComplexTypeIsAbstract)
                                    {
                                        result = default(T);
                                    }
                                    else
                                    {
                                        result = FormatterLocator.GetFormatter<T>(context.Config.SerializationPolicy).Deserialize(reader);
                                    }

                                    if (id >= 0)
                                    {
                                        context.RegisterInternalReference(id, result);
                                    }

                                    return result;
                                }
                                else
                                {
                                    context.Config.DebugContext.LogError("Failed to enter node '" + name + "'.");
                                    return default(T);
                                }
                            }
                            catch (SerializationAbortException ex)
                            {
                                exitNode = false;
                                throw ex;
                            }
                            catch (Exception ex)
                            {
                                context.Config.DebugContext.LogException(ex);
                                return default(T);
                            }
                            finally
                            {
                                if (exitNode)
                                {
                                    reader.ExitNode();
                                }
                            }
                        }

                    //
                    // The below cases are for when we expect an object, but have
                    // serialized a straight primitive type. In such cases, we can
                    // often box the primitive type as an object.
                    //
                    // Sadly, the exact primitive type might be lost in case of
                    // integer and floating points numbers, as we don't know what
                    // type to expect.
                    //
                    // To be safe, we read and box the most precise type available.
                    //

                    case EntryType.Boolean:
                        {
                            if (!ComplexTypeMayBeBoxedValueType)
                            {
                                goto default;
                            }

                            bool value;
                            reader.ReadBoolean(out value);
                            return (T)(object)value;
                        }

                    case EntryType.FloatingPoint:
                        {
                            if (!ComplexTypeMayBeBoxedValueType)
                            {
                                goto default;
                            }

                            double value;
                            reader.ReadDouble(out value);
                            return (T)(object)value;
                        }

                    case EntryType.Integer:
                        {
                            if (!ComplexTypeMayBeBoxedValueType)
                            {
                                goto default;
                            }

                            long value;
                            reader.ReadInt64(out value);
                            return (T)(object)value;
                        }

                    case EntryType.String:
                        {
                            if (!ComplexTypeMayBeBoxedValueType)
                            {
                                goto default;
                            }

                            string value;
                            reader.ReadString(out value);
                            return (T)(object)value;
                        }

                    case EntryType.Guid:
                        {
                            if (!ComplexTypeMayBeBoxedValueType)
                            {
                                goto default;
                            }

                            Guid value;
                            reader.ReadGuid(out value);
                            return (T)(object)value;
                        }

                    default:

                        // Lost value somehow
                        context.Config.DebugContext.LogWarning("Unexpected entry of type " + entry.ToString() + ", when a reference or node start was expected. A value has been lost.");
                        reader.SkipEntry();
                        return default(T);
                }
            }
        }

        /// <summary>
        /// Writes a value of type <see cref="T" />.
        /// </summary>
        /// <param name="name">The name of the value to write.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="writer">The writer to use.</param>
        public override void WriteValue(string name, T value, IDataWriter writer)
        {
            var context = writer.Context;

            if (context.Config.SerializationPolicy.AllowNonSerializableTypes == false && typeof(T).IsSerializable == false)
            {
                context.Config.DebugContext.LogError("The type " + typeof(T).Name + " is not marked as serializable.");
                return;
            }

            FireOnSerializedType();

            if (ComplexTypeIsValueType)
            {
                bool endNode = true;

                try
                {
                    writer.BeginStructNode(name, typeof(T));
                    FormatterLocator.GetFormatter<T>(context.Config.SerializationPolicy).Serialize(value, writer);
                }
                catch (SerializationAbortException ex)
                {
                    endNode = false;
                    throw ex;
                }
                finally
                {
                    if (endNode)
                    {
                        writer.EndNode(name);
                    }
                }
            }
            else
            {
                int id;
                int index;
                string strId;
                Guid guid;

                bool endNode = true;

                if (object.ReferenceEquals(value, null))
                {
                    writer.WriteNull(name);
                }
                else if (context.TryRegisterExternalReference(value, out index))
                {
                    writer.WriteExternalReference(name, index);
                }
                else if (context.TryRegisterExternalReference(value, out guid))
                {
                    writer.WriteExternalReference(name, guid);
                }
                else if (context.TryRegisterExternalReference(value, out strId))
                {
                    writer.WriteExternalReference(name, strId);
                }
                else if (context.TryRegisterInternalReference(value, out id))
                {
                    // Get type of actual stored object
                    //
                    // Don't have it as a strongly typed T value, since people can "override" (shadow)
                    // GetType() on derived classes with the "new" operator. By referencing the type
                    // as a System.Object, we ensure the correct GetType() method is always called.
                    //
                    // (Yes, this has actually happened, and this was done to fix it.)

                    Type type = (value as object).GetType();

                    if (ComplexTypeMayBeBoxedValueType && FormatterUtilities.IsPrimitiveType(type)) // It's a boxed primitive type
                    {
                        try
                        {
                            writer.BeginReferenceNode(name, type, id);

                            var serializer = Serializer.Get(type);
                            serializer.WriteValueWeak(value, writer);
                        }
                        catch (SerializationAbortException ex)
                        {
                            endNode = false;
                            throw ex;
                        }
                        finally
                        {
                            if (endNode)
                            {
                                writer.EndNode(name);
                            }
                        }
                    }
                    else
                    {
                        var formatter = FormatterLocator.GetFormatter(type, context.Config.SerializationPolicy);

                        try
                        {
                            writer.BeginReferenceNode(name, type, id);
                            formatter.Serialize(value, writer);
                        }
                        catch (SerializationAbortException ex)
                        {
                            endNode = false;
                            throw ex;
                        }
                        finally
                        {
                            if (endNode)
                            {
                                writer.EndNode(name);
                            }
                        }
                    }
                }
                else
                {
                    writer.WriteInternalReference(name, id);
                }
            }
        }
    }
}
#pragma warning enable