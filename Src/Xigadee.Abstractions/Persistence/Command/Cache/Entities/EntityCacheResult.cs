namespace Xigadee
{
    public class EntityCacheResult<E>
    {
        public bool Success { get; set; }

        public bool Exists { get; set; }

        public E Entity { get; set; }
    }
}
