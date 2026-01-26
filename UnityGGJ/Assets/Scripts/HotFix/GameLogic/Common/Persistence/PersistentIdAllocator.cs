using System;
using System.Threading;

namespace GameLogic
{
    /// <summary>
    /// PersistentId 分配器（持久化 ID 分配器）
    ///
    /// 用途：
    /// - 为需要序列化 / 跨时间保持一致性的实体分配唯一 ID
    /// - ID 单调递增，保证在同一个世界/存档内不发生冲突
    /// - 支持 Save / Load 后继续分配
    ///
    /// 设计约定：
    /// - ID 0 作为无效值（Invalid）
    /// - 正常分配从 1 开始
    /// - 线程安全（使用 Interlocked）
    /// </summary>
    public sealed class PersistentIdAllocator
    {
        /// <summary>
        /// 无效 ID（保留）
        /// </summary>
        public const ulong InvalidId = 0UL;

        // _nextId 表示“下一个将被分配的 ID”
        // 不变量：_nextId >= 1
        // 使用 long 是为了配合 Interlocked API
        private long _nextId;

        /// <summary>
        /// 创建一个 PersistentIdAllocator
        /// </summary>
        /// <param name="startFrom">
        /// 起始 ID（默认从 1 开始）
        /// 注意：0 被保留为 InvalidId
        /// </param>
        public PersistentIdAllocator(ulong startFrom = 1UL)
        {
            if (startFrom == InvalidId)
                throw new ArgumentException("startFrom 不能为 0，0 被保留为 InvalidId。", nameof(startFrom));

            if (startFrom > long.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(startFrom), "startFrom 超出了分配器支持的范围。");

            _nextId = (long)startFrom;
        }

        /// <summary>
        /// 分配一个新的、唯一的 PersistentId
        /// </summary>
        /// <returns>新分配的 PersistentId</returns>
        public ulong Allocate()
        {
            // Interlocked.Increment 返回的是递增后的值
            // 因此这里先 +1，再 -1，得到“当前分配的 ID”
            long id = Interlocked.Increment(ref _nextId) - 1;

            if (id <= 0)
                throw new InvalidOperationException("分配器生成了非法 ID，请检查初始化逻辑。");

            return (ulong)id;
        }

        /// <summary>
        /// 获取当前的 nextId（即下一个将被分配的 ID）
        /// 保存存档时应持久化这个值
        /// </summary>
        public ulong GetNextId()
        {
            long value = Volatile.Read(ref _nextId);
            if (value <= 0)
                return 1UL;

            return (ulong)value;
        }

        /// <summary>
        /// 从存档中恢复 nextId
        /// 恢复后，后续 Allocate() 不会与已有 ID 发生冲突
        /// </summary>
        /// <param name="nextIdFromSave">存档中保存的 nextId</param>
        public void RestoreNextId(ulong nextIdFromSave)
        {
            if (nextIdFromSave == InvalidId)
                nextIdFromSave = 1UL;

            if (nextIdFromSave > long.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(nextIdFromSave), "nextIdFromSave 超出了分配器支持的范围。");

            Interlocked.Exchange(ref _nextId, (long)nextIdFromSave);
        }

        /// <summary>
        /// 确保当前 nextId 至少不小于指定值
        /// 常用于：
        /// - 导入外部数据
        /// - 修复旧存档
        /// - nextId 丢失或不可信时的兜底处理
        /// </summary>
        /// <param name="minNextId">
        /// 最小允许的 nextId（通常是 maxExistingId + 1）
        /// </param>
        public void EnsureAtLeast(ulong minNextId)
        {
            if (minNextId == InvalidId)
                minNextId = 1UL;

            if (minNextId > long.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(minNextId), "minNextId 超出了分配器支持的范围。");

            while (true)
            {
                long current = Volatile.Read(ref _nextId);

                // 已满足要求
                if ((ulong)current >= minNextId)
                    return;

                // 尝试把 _nextId 提升到 minNextId
                long original = Interlocked.CompareExchange(
                    ref _nextId,
                    (long)minNextId,
                    current
                );

                if (original == current)
                    return;
            }
        }
    }
}
