using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PodcastUtilities.AndroidLogic.ViewModel
{
    public class ViewModelFactory : Java.Lang.Object, ViewModelProvider.IFactory
    {
        public struct ClassMap
        {
            public Java.Lang.Class javaClassType;
            public Type netType;
        }

        private IIocContainer IocContainer;
        private ILogger Logger;

        private Dictionary<string, ClassMap> ModelMap = new Dictionary<string, ClassMap>(10);

        public ViewModelFactory(
            IIocContainer container,
            ILogger logger
            )
        {
            IocContainer = container;
            Logger = logger;
        }

        public void AddMap(ClassMap item)
        {
            string name = item.javaClassType.CanonicalName ?? throw new InvalidOperationException($"Cannot get name of class {item.ToString}");
            Logger.Debug(() => $"ViewModelFactory:AddMap {name}");
            ModelMap.Add(name, item);
        }

        public void AddMap(Type type)
        {
            AddMap(new ClassMap() { javaClassType = Java.Lang.Class.FromType(type), netType = type });
        }

        public Java.Lang.Object Create(Java.Lang.Class classType)
        {
            //Type fred = typeof(DownloadViewModel);
            Logger.Debug(() => $"ViewModelFactory:Create {classType.CanonicalName}");

            // instead of a generic call like this
            // var obj = IocContainer.Resolve<DownloadViewModel>();
            // we do this - because we do not know the type (DownloadViewModel in this example) at compile time
            MethodInfo method = typeof(IIocContainer).GetMethod(nameof(IIocContainer.Resolve))!;
            string name = classType.CanonicalName ?? throw new InvalidOperationException($"Cannot get name of class {classType.Name}");
            MethodInfo generic = method.MakeGenericMethod(ModelMap[name].netType);
            var obj = generic.Invoke(IocContainer, null);

            Logger.Debug(() => $"ViewModelFactory:Create null == {obj == null}");
            return (Java.Lang.Object)obj!;
        }
    }
}