using System;

namespace EOSUnityPlugin.EpicOnlineServices
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class MonoPInvokeCallbackAttribute : Attribute
    {
        public MonoPInvokeCallbackAttribute(Type type)
        {
        }
    }
}
