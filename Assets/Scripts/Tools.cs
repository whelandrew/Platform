using UnityEngine;
namespace Tools
{
    public class AI
    {
        public static bool FacingLeft(Vector3 curPos, Vector3 target)
        {
            if(curPos.x <= target.x)
            {
                return false;
            }

            if(curPos.x > target.x)
            {
                return true;
            }

            return false;
        }
    }
}
