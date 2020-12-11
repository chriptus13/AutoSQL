using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using SqlReflect.Attributes;

namespace SqlReflect {
    public class EmitDataMapper {
        /// Dictionary which maps a Model Type into the respective DynamicDataMapper Type
        private static Dictionary<Type, Type> map = new Dictionary<Type, Type>();

        public static DynamicDataMapper Build(Type klass, string connStr, bool withCache) {
            int numberOfComplexAttributes = GetNumberOfComplexAttributes(klass.GetProperties(), out PropertyInfo[] cProperties);

            List<DynamicDataMapper> dataMappers = new List<DynamicDataMapper>();
            foreach(PropertyInfo p in cProperties) dataMappers.Add(Build(p.PropertyType, connStr, withCache));

            if(map.TryGetValue(klass, out Type res))
                if(numberOfComplexAttributes == 0) return (DynamicDataMapper) Activator.CreateInstance(res, new Object[] { connStr, withCache });
                else return (DynamicDataMapper) Activator.CreateInstance(res, new Object[] { connStr, withCache, dataMappers.ToArray() });

            // Set AssemblyName
            AssemblyName an = new AssemblyName(klass.Name + "DynamicDataMapperEmited");
            // Get Builder for the Assembly
            AssemblyBuilder ab = AppDomain.CurrentDomain
                .DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);

            // Create new Module
            ModuleBuilder mb = ab.DefineDynamicModule(an.Name, an.Name + ".dll");

            // Create new Type in the module
            TypeBuilder tb = mb.DefineType(klass.Name + "DynamicDataMapper",
                TypeAttributes.Public, typeof(DynamicDataMapper));

            // Constructor
            GenerateConstructor(tb, klass, cProperties, out FieldInfo[] fields);

            // Method Insert
            GenerateInsertMethod(tb, klass);

            // Method Update
            GenerateUpdateMethod(tb, klass);

            // Method Delete
            GenerateDeleteMethod(tb, klass);

            // Method Load
            GenerateLoadMethod(tb, klass, fields);

            // Type Creating
            res = tb.CreateType();
            // Save Dynamic Assembly in Disk
            ab.Save(an.Name + ".dll");

            // Add new entry to map
            map.Add(klass, res);

            if(numberOfComplexAttributes == 0) return (DynamicDataMapper) Activator.CreateInstance(res, new Object[] { connStr, withCache });
            else return (DynamicDataMapper) Activator.CreateInstance(res, new Object[] { connStr, withCache, dataMappers.ToArray() });
        }

        private static int GetNumberOfComplexAttributes(PropertyInfo[] propertiesInfo, out PropertyInfo[] propertiesComplex) {
            List<PropertyInfo> aux = new List<PropertyInfo>();

            foreach(PropertyInfo p in propertiesInfo)
                if(!p.PropertyType.Namespace.StartsWith("System")) aux.Add(p);

            propertiesComplex = aux.ToArray();
            return propertiesComplex.Length;
        }

        /// Generates Constructor
        private static void GenerateConstructor(TypeBuilder typeBuilder, Type klass, PropertyInfo[] cProperties, out FieldInfo[] fields) {
            Type[] args;
            if(cProperties.Length != 0) args = new Type[] { typeof(String), typeof(bool), typeof(DynamicDataMapper[]) };
            else args = new Type[] { typeof(String), typeof(bool) };
            ConstructorBuilder cb = typeBuilder
                .DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, args);
            ILGenerator ctrGen = cb.GetILGenerator();

            ctrGen.Emit(OpCodes.Ldarg_0);
            ctrGen.Emit(OpCodes.Ldtoken, klass);
            ctrGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            ctrGen.Emit(OpCodes.Ldarg_1);
            ctrGen.Emit(OpCodes.Ldarg_2);
            ctrGen.Emit(OpCodes.Call, typeof(DynamicDataMapper).GetConstructor(new Type[] { typeof(Type), typeof(String), typeof(bool) }));
            fields = new FieldInfo[cProperties.Length];
            for(int i = 0; i < cProperties.Length; ++i) {
                string fldName = cProperties[i].PropertyType.Name + "DataMapper";
                FieldBuilder fb = typeBuilder.DefineField(fldName, typeof(AbstractDataMapper), FieldAttributes.Private | FieldAttributes.InitOnly);
                fields[i] = fb;
                ctrGen.Emit(OpCodes.Ldarg_0);
                ctrGen.Emit(OpCodes.Ldarg_3);
                ctrGen.Emit(OpCodes.Ldc_I4, i);
                ctrGen.Emit(OpCodes.Ldelem_Ref);
                ctrGen.Emit(OpCodes.Stfld, fb);
            }
            ctrGen.Emit(OpCodes.Ret);
        }

        /// Generates Insert Method
        private static void GenerateInsertMethod(TypeBuilder typeBuilder, Type klass) {
            MethodBuilder methb = typeBuilder.DefineMethod("SqlInsert",
                MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.HasThis, typeof(String), new Type[] { typeof(Object) });
            ILGenerator methGen = methb.GetILGenerator();

            methGen.DeclareLocal(klass);
            methGen.DeclareLocal(typeof(StringBuilder));

            methGen.Emit(OpCodes.Ldarg_1);
            if(klass.IsValueType) methGen.Emit(OpCodes.Unbox_Any, klass);
            else methGen.Emit(OpCodes.Castclass, klass);
            methGen.Emit(OpCodes.Stloc_0);
            methGen.Emit(OpCodes.Newobj, typeof(StringBuilder).GetConstructor(new Type[] { }));
            methGen.Emit(OpCodes.Stloc_1);
            methGen.Emit(OpCodes.Ldloc_1);

            PropertyInfo pk = null;
            foreach(PropertyInfo p in klass.GetProperties())
                if(p.IsDefined(typeof(PKAttribute))) {
                    pk = p;
                    break;
                }

            int i = 0, j = 2;
            bool isSystemClass;
            foreach(PropertyInfo p in klass.GetProperties()) {
                if(p == pk && ((PKAttribute) p.GetCustomAttribute(typeof(PKAttribute))).AutoIncrement) continue;
                isSystemClass = p.PropertyType.Namespace.StartsWith("System");
                if(i == 0) ++i;
                else {
                    methGen.Emit(OpCodes.Ldstr, ", ");
                    methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(String) }));
                }

                if(klass.IsValueType) {
                    methGen.Emit(OpCodes.Ldloca_S, (byte) 0);
                    methGen.Emit(OpCodes.Call, p.GetMethod);
                } else {
                    methGen.Emit(OpCodes.Ldloc_0);
                    methGen.Emit(OpCodes.Callvirt, p.GetMethod);
                }

                PropertyInfo ppk = p.PropertyType.GetProperty(p.PropertyType.Name + "ID");
                if(!isSystemClass) {
                    if(p.PropertyType.IsValueType) {
                        methGen.DeclareLocal(p.PropertyType);
                        methGen.Emit(OpCodes.Stloc_S, (byte) j);
                        methGen.Emit(OpCodes.Ldloca_S, (byte) j);
                        methGen.Emit(OpCodes.Call, ppk.GetMethod);
                        j++;
                    } else methGen.Emit(OpCodes.Callvirt, ppk.GetMethod);
                    if(ppk.PropertyType.IsPrimitive) methGen.Emit(OpCodes.Box, ppk.PropertyType);
                } else if(p.PropertyType.IsPrimitive) methGen.Emit(OpCodes.Box, p.PropertyType);

                methGen.Emit(OpCodes.Call, typeof(DynamicDataMapper).GetMethod("ResolveNull"));
                methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(String) }));
            }
            methGen.Emit(OpCodes.Pop);
            methGen.Emit(OpCodes.Ldarg_0);
            methGen.Emit(OpCodes.Ldfld, typeof(DynamicDataMapper).GetField("insertStmt"));
            methGen.Emit(OpCodes.Ldloc_1);
            methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("ToString", new Type[] { }));
            methGen.Emit(OpCodes.Call, typeof(String).GetMethod("Format", new Type[] { typeof(String), typeof(Object) }));
            methGen.Emit(OpCodes.Ret);
        }

        /// Generates Update Method
        private static void GenerateUpdateMethod(TypeBuilder typeBuilder, Type klass) {
            MethodBuilder methb = typeBuilder.DefineMethod("SqlUpdate",
                MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.HasThis, typeof(String), new Type[] { typeof(Object) });
            ILGenerator methGen = methb.GetILGenerator();

            methGen.DeclareLocal(klass);
            methGen.DeclareLocal(typeof(StringBuilder));
            methGen.DeclareLocal(typeof(StringBuilder));

            methGen.Emit(OpCodes.Ldarg_1);
            if(klass.IsValueType) methGen.Emit(OpCodes.Unbox_Any, klass);
            else methGen.Emit(OpCodes.Castclass, klass);
            methGen.Emit(OpCodes.Stloc_0);
            methGen.Emit(OpCodes.Newobj, typeof(StringBuilder).GetConstructor(new Type[] { }));
            methGen.Emit(OpCodes.Stloc_1);
            methGen.Emit(OpCodes.Ldloc_1);

            PropertyInfo pk = null;
            foreach(PropertyInfo p in klass.GetProperties())
                if(p.IsDefined(typeof(PKAttribute))) {
                    pk = p;
                    break;
                }

            int i = 0, j = 3;
            bool isSystemClass;
            foreach(PropertyInfo p in klass.GetProperties()) {
                if(p == pk) continue;
                isSystemClass = p.PropertyType.Namespace.StartsWith("System");
                if(i == 0) {
                    methGen.Emit(OpCodes.Ldstr, p.Name + (isSystemClass ? "" : "ID") + "=");
                    methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(String) }));
                    ++i;
                } else {
                    methGen.Emit(OpCodes.Ldstr, ", " + p.Name + (isSystemClass ? "" : "ID") + "=");
                    methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(String) }));
                }

                if(klass.IsValueType) {
                    methGen.Emit(OpCodes.Ldloca_S, (byte) 0);
                    methGen.Emit(OpCodes.Call, p.GetMethod);
                } else {
                    methGen.Emit(OpCodes.Ldloc_0);
                    methGen.Emit(OpCodes.Callvirt, p.GetMethod);
                }

                PropertyInfo ppk = p.PropertyType.GetProperty(p.PropertyType.Name + "ID");
                if(!isSystemClass) {
                    if(p.PropertyType.IsValueType) {
                        methGen.DeclareLocal(p.PropertyType);
                        methGen.Emit(OpCodes.Stloc_S, (byte) j);
                        methGen.Emit(OpCodes.Ldloca_S, (byte) j);
                        methGen.Emit(OpCodes.Call, ppk.GetMethod);
                        j++;
                    } else methGen.Emit(OpCodes.Callvirt, ppk.GetMethod);
                    if(ppk.PropertyType.IsPrimitive) methGen.Emit(OpCodes.Box, ppk.PropertyType);
                } else if(p.PropertyType.IsPrimitive) methGen.Emit(OpCodes.Box, p.PropertyType);

                methGen.Emit(OpCodes.Call, typeof(DynamicDataMapper).GetMethod("ResolveNull"));
                methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(String) }));
            }
            methGen.Emit(OpCodes.Pop);

            methGen.Emit(OpCodes.Newobj, typeof(StringBuilder).GetConstructor(new Type[] { }));
            methGen.Emit(OpCodes.Stloc_2);
            methGen.Emit(OpCodes.Ldloc_2);
            methGen.Emit(OpCodes.Ldc_I4_S, (byte) '\'');
            methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(char) }));
            if(klass.IsValueType) {
                methGen.Emit(OpCodes.Ldloca_S, (byte) 0);
                methGen.Emit(OpCodes.Call, pk.GetMethod);
            } else {
                methGen.Emit(OpCodes.Ldloc_0);
                methGen.Emit(OpCodes.Callvirt, pk.GetMethod);
            }
            methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[] { pk.PropertyType }));
            methGen.Emit(OpCodes.Ldc_I4_S, (byte) '\'');
            methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[] { typeof(char) }));
            methGen.Emit(OpCodes.Pop);

            methGen.Emit(OpCodes.Ldarg_0);
            methGen.Emit(OpCodes.Ldfld, typeof(DynamicDataMapper).GetField("updateStmt"));
            methGen.Emit(OpCodes.Ldloc_1);
            methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("ToString", new Type[] { }));
            methGen.Emit(OpCodes.Ldloc_2);
            methGen.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("ToString", new Type[] { }));
            methGen.Emit(OpCodes.Call, typeof(String).GetMethod("Format", new Type[] { typeof(String), typeof(Object), typeof(Object) }));
            methGen.Emit(OpCodes.Ret);
        }

        /// Generates Delete Method in the specified TypeBuilder and for the Type klass
        private static void GenerateDeleteMethod(TypeBuilder typeBuilder, Type klass) {
            MethodBuilder methb = typeBuilder.DefineMethod("SqlDelete",
                MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.HasThis, typeof(String), new Type[] { typeof(Object) });
            ILGenerator methGen = methb.GetILGenerator();

            methGen.DeclareLocal(klass);

            methGen.Emit(OpCodes.Ldarg_1);
            if(klass.IsValueType) methGen.Emit(OpCodes.Unbox_Any, klass);
            else methGen.Emit(OpCodes.Castclass, klass);
            methGen.Emit(OpCodes.Stloc_0);
            methGen.Emit(OpCodes.Ldarg_0);
            methGen.Emit(OpCodes.Ldfld, typeof(DynamicDataMapper).GetField("deleteStmt"));
            methGen.Emit(OpCodes.Ldstr, "'");

            PropertyInfo pk = null;
            foreach(PropertyInfo p in klass.GetProperties())
                if(p.IsDefined(typeof(PKAttribute))) {
                    pk = p;
                    break;
                }
            if(pk == null) throw new InvalidOperationException(klass.Name + " should have a property with a PK attribute");
            if(klass.IsValueType) {
                methGen.Emit(OpCodes.Ldloca_S, (byte) 0);
                methGen.Emit(OpCodes.Call, pk.GetMethod);
            } else {
                methGen.Emit(OpCodes.Ldloc_0);
                methGen.Emit(OpCodes.Callvirt, pk.GetMethod);
            }
            methGen.Emit(OpCodes.Box, pk.PropertyType);

            methGen.Emit(OpCodes.Ldstr, "'");
            methGen.Emit(OpCodes.Call, typeof(String).GetMethod("Concat",
                new Type[] { typeof(Object), typeof(Object), typeof(Object), typeof(Object) }));
            methGen.Emit(OpCodes.Ret);
        }

        /// Generates Load Method in the specified TypeBuilder and for the Type klass
        private static void GenerateLoadMethod(TypeBuilder typeBuilder, Type klass, FieldInfo[] fields) {
            MethodBuilder methb = typeBuilder.DefineMethod("Load",
                MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.HasThis, typeof(Object), new Type[] { typeof(IDataReader) });
            ILGenerator methGen = methb.GetILGenerator();

            methGen.DeclareLocal(klass);
            if(klass.IsValueType) {
                methGen.Emit(OpCodes.Ldloca_S, (byte) 0);
                methGen.Emit(OpCodes.Initobj, klass);
            } else {
                methGen.Emit(OpCodes.Newobj, klass.GetConstructor(new Type[] { }));
                methGen.Emit(OpCodes.Stloc_0);
            }

            bool isSystemClass;
            int i = 0;
            foreach(PropertyInfo p in klass.GetProperties()) {
                isSystemClass = p.PropertyType.Namespace.StartsWith("System");
                if(klass.IsValueType) methGen.Emit(OpCodes.Ldloca_S, (byte) 0);
                else methGen.Emit(OpCodes.Ldloc_0);

                if(!isSystemClass) {
                    methGen.Emit(OpCodes.Ldarg_0);
                    methGen.Emit(OpCodes.Ldfld, fields[i++]);
                } else if(!p.PropertyType.IsValueType) methGen.Emit(OpCodes.Ldarg_0);

                methGen.Emit(OpCodes.Ldarg_1);

                methGen.Emit(OpCodes.Ldstr, p.Name + (isSystemClass ? "" : "ID"));
                methGen.Emit(OpCodes.Callvirt, typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(String) }));

                if(!isSystemClass) methGen.Emit(OpCodes.Callvirt, typeBuilder.BaseType.GetMethod("GetById"));
                else if(!p.PropertyType.IsValueType) methGen.Emit(OpCodes.Callvirt, typeof(DynamicDataMapper).GetMethod("Caster"));

                if(p.PropertyType.IsValueType) methGen.Emit(OpCodes.Unbox_Any, p.PropertyType);
                else methGen.Emit(OpCodes.Castclass, p.PropertyType);

                if(klass.IsValueType) methGen.Emit(OpCodes.Call, p.SetMethod);
                else methGen.Emit(OpCodes.Callvirt, p.SetMethod);
            }
            methGen.Emit(OpCodes.Ldloc_0);
            if(klass.IsValueType) methGen.Emit(OpCodes.Box, klass);
            methGen.Emit(OpCodes.Ret);
        }
    }
}