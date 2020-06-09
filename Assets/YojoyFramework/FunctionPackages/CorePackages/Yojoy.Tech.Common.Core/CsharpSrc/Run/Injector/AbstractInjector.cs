#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/9 11:49:15
//Email:         854327817@qq.com

#endregion

//��������ʵ������֮���ӳ��
using InstanceTypeMap = System.Collections.Generic.Dictionary<
System.Type, System.Type>;
//���ͺ͵���ʵ��֮���ӳ��
using SingleStorage = System.Collections.Generic.Dictionary<
    System.Type, object>;
//���ͺ͹���ί��֮���ӳ��
using BuildFuncStorage = System.Collections.Generic.Dictionary<
    System.Type, System.Func<object>>;
//���͵��ֶ���Ϣ
using FieldInfoStorage = System.Collections.Generic.Dictionary
    <System.Type, System.Collections.Generic.List<System.Reflection.FieldInfo>>;
//���͵�������Ϣ
using PropertyInfoStorage = System.Collections.Generic.Dictionary
    <System.Type, System.Collections.Generic.List<System.Reflection.PropertyInfo>>;
using System.Reflection;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Yojoy.Tech.Common.Core.Run
{
    /// <summary>
    /// ע�����ڿ����׶�ʹ�÷������ע������
    /// �����׶�ʹ��ί�����ע������
    /// </summary>
    public abstract class AbstractInjector : IInjector
    {
        /// <summary>
        /// ������������
        /// </summary>
        private const BindingFlags BindingFlags =
           System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static;

        private const string HiddenConstructorId = "HiddenConstructor";

        private readonly object[] constructorArray = new object[1];

        protected readonly DelayInitializationProperty<InstanceTypeMap>
            instanceTypeDelay = CreateDelayInitializationProperty(() => new InstanceTypeMap());
        protected readonly DelayInitializationProperty<SingleStorage>
            singleStorageDelay = CreateDelayInitializationProperty(() => new SingleStorage());
        protected readonly DelayInitializationProperty<BuildFuncStorage>
            buildFuncsDelay = CreateDelayInitializationProperty(() => new BuildFuncStorage());
        protected readonly DelayInitializationProperty<FieldInfoStorage>
            fieldInfoStorageDelay = CreateDelayInitializationProperty(() => new FieldInfoStorage());
        protected readonly DelayInitializationProperty<PropertyInfoStorage>
            propertyInfoStorageDelay = CreateDelayInitializationProperty(() => new PropertyInfoStorage());



        private void InvokeHiddenConstructor(object instance)
        {
            var methods = instance.GetType().GetMethods(BindingFlags).ToList();
            var hiddenConstructor = methods.Find(m => m.Name == HiddenConstructorId);
            hiddenConstructor?.Invoke(instance, constructorArray);
        }
        protected bool IsSingle(Type type) => type.GetSingleAttribute<SingleAttribute>() != null;

        protected bool HasInjectorAttribute(MemberInfo memberInfo)
        {
            var result = memberInfo.GetCustomAttributes(typeof(InjectAttribute), true).Any();
            return result;
        }

        public TTargetType Get<TTargetType>(bool useReflection) where TTargetType : class
        {
            throw new System.NotImplementedException();
        }

        protected abstract List<Type> AllTypes { get; set; }

        private InstanceTypeMap defaultInjectMap;

        public InstanceTypeMap DefautInjectMap
        {
            get
            {
                if (defaultInjectMap != null)
                {
                    return defaultInjectMap;
                }
                defaultInjectMap = new InstanceTypeMap();

                foreach (var type in AllTypes)
                {
                    var defaultInjectAttribute = type.GetSingleAttribute<DefaultInjectAttribute>();
                    if (defaultInjectAttribute != null)
                    {
                        defaultInjectMap.Add(defaultInjectAttribute.TargetType, type);
                    }
                }
                return defaultInjectMap;
            }
        }
        protected virtual Type GetInstanceType(Type targetType)
        {
            if (instanceTypeDelay.Value.ContainsKey(targetType))
            {
                return instanceTypeDelay.Value[targetType];
            }
            if (defaultInjectMap.ContainsKey(targetType))
            {
                return defaultInjectMap[targetType];
            }
            var isAbstract = IsAbstract(targetType);

            if (isAbstract)
            {
                throw new Exception("Abstract types and interface" +
                                       "types cannot create instance!");
            }
            return targetType;

            bool IsAbstract(Type type)
            {
                var result = type.IsAbstract || type.IsInterface;
                return result;
            }
        }
    }
}
