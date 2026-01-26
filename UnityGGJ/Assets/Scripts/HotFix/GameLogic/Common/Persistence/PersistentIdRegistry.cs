using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 维护 EntityId 与 PersistentId 的映射关系。
    /// - 不要求改 Entity 框架
    /// - Runtime 可以只持有 EntityId，需要时查表拿 PersistentId
    /// </summary>
    public sealed class PersistentIdRegistry
    {
        private readonly Dictionary<int, ulong> _pidByEid = new();
        private readonly Dictionary<ulong, int> _eidByPid = new();

        public bool TryGetPersistentId(int entityId, out ulong persistentId)
            => _pidByEid.TryGetValue(entityId, out persistentId);

        public ulong GetPersistentId(int entityId)
        {
            if (!TryGetPersistentId(entityId, out ulong pid))
            {
                Debug.LogError($"EntityId {entityId} not found in PidRegistry");
                return 0;
            }
            return pid;
        }

        public bool TryGetEntityId(ulong persistentId, out int entityId)
            => _eidByPid.TryGetValue(persistentId, out entityId);

        public int GetEntityId(ulong persistentId)
        {
            if (!TryGetEntityId(persistentId, out int entityId))
            {
                Debug.LogError($"PersistentId {persistentId} not found in PidRegistry");
                return 0;
            }
            return entityId;
        }

        /// <summary>
        /// 新建实体绑定：通常由 allocator 自动生成 pid 后调用。
        /// </summary>
        public void BindNew(int entityId, ulong persistentId)
        {
            Debug.Log("绑定新实体: entityId=" + entityId + ", persistentId=" + persistentId);
            BindInternal(entityId, persistentId, allowOverwrite: false);
        }

        /// <summary>
        /// 读档绑定：pid 来自存档数据。
        /// </summary>
        public void BindFromSave(int entityId, ulong persistentId)
        {
            BindInternal(entityId, persistentId, allowOverwrite: false);
        }

        private void BindInternal(int entityId, ulong persistentId, bool allowOverwrite)
        {
            if (entityId <= 0) throw new ArgumentOutOfRangeException(nameof(entityId));
            if (persistentId == 0) throw new ArgumentOutOfRangeException(nameof(persistentId), "0 为无效 pid");

            if (!allowOverwrite)
            {
                if (_pidByEid.ContainsKey(entityId))
                    throw new InvalidOperationException($"entityId={entityId} 已绑定 pid，重复 Bind。");

                if (_eidByPid.ContainsKey(persistentId))
                    throw new InvalidOperationException($"persistentId={persistentId} 已绑定到其他 entity，发生冲突。");
            }

            _pidByEid[entityId] = persistentId;
            _eidByPid[persistentId] = entityId;
        }

        public void Unbind(int entityId)
        {
            if (_pidByEid.TryGetValue(entityId, out var pid))
            {
                _pidByEid.Remove(entityId);
                _eidByPid.Remove(pid);
            }
        }

        public void Clear()
        {
            _pidByEid.Clear();
            _eidByPid.Clear();
        }
    }
}
