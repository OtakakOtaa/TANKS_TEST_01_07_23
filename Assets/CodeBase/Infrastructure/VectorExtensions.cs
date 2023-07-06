using UnityEngine;

namespace CodeBase.Infrastructure
{
    public static class VectorExtensions
    {
        public static bool VectorInArrayRange(this Vector3Int target, Vector3 size)
            => !(target.x >= size.x || target.x < 0 || target.y >= size.y || target.y < 0);
        
        public static bool VectorInArrayRange(this Vector2 target, Vector3 size)
            => !(target.x >= size.x || target.x < 0 || target.y >= size.y || target.y < 0);
    }
}