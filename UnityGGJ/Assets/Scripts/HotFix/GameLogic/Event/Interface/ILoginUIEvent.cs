using UnityGameFramework.Runtime;

namespace GameLogic
{
    [EventInterface(EEventGroup.GroupUI)]
    public interface ILoginUIEvent
    {
        public void OnRoleLogin();

        public void OnRoleLoginOut(int a1, bool b2);
    }
}