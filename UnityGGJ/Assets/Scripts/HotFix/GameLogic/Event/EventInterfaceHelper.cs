using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventInterfaceImpAttribute : BaseAttribute
    {
        private EEventGroup _eGroup;
        public EEventGroup EventGroup => _eGroup;

        public EventInterfaceImpAttribute(EEventGroup group)
        {
            _eGroup = group;
        }
    }

    public class EventInterfaceHelper
    {
        public static void Init()
        {
            Register(GameEvent.EventMgr, EEventGroup.GroupLogic);
            Register(GameEvent.EventMgr, EEventGroup.GroupUI);
        }
        
        private static void Register(EventMgr mgr, EEventGroup eGroup)
        {
            var dispatcher = mgr.Dispatcher;

            HashSet<Type> types = TypesManager.Instance.GetTypes(typeof(EventInterfaceImpAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(EventInterfaceImpAttribute), false);
                if (attrs.Length == 0)
                    continue;
                EventInterfaceImpAttribute httpHandlerAttribute = (EventInterfaceImpAttribute)attrs[0];
                if (httpHandlerAttribute.EventGroup != eGroup)
                    continue;
                object obj = Activator.CreateInstance(type, dispatcher);
                mgr.RegWrapInterface(obj.GetType().GetInterfaces()[0]?.FullName, obj);
            }
        }
    }
}