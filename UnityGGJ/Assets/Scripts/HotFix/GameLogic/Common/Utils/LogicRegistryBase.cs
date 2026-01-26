using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace GameLogic
{
    public class LogicRegistryBase<TLogic> where TLogic : class
    {
        private readonly Dictionary<string, TLogic> _logics = new Dictionary<string, TLogic>(StringComparer.Ordinal);
        public virtual UniTask InitAsync()
        {
            var typesPair = TypesManager.Instance.GetTypes();
            foreach (var typePair in typesPair)
            {
                Type type = typePair.Value;
                if (type != null && !type.IsInterface && typeof(TLogic).IsAssignableFrom(type))
                    _logics[type.FullName] = Activator.CreateInstance(type) as TLogic;
            }
            return UniTask.CompletedTask;
        }

        public virtual bool TryGetLogic(string logicId, out TLogic logic)
        {
            logic = null;
            if (string.IsNullOrEmpty(logicId))
                return false;
            _logics.TryGetValue(logicId, out logic);
            return true;
        }
    }
}
