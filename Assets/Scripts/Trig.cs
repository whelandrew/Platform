using UnityEngine;
namespace Trig
{
    public class Angles
    {
        public static Vector3 GetVectorFromAngle(float angle)
        {
            float angledRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angledRad), Mathf.Sin(angledRad));
        }

        public static float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }
    }
}
