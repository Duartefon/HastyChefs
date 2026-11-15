using UnityEngine;

public static class MathUtilities
{

    /// <summary>
    /// Returns the shortest delta rotation needed to rotate from 'from' to 'to'.
    /// </summary>
    public static Quaternion ShortestRotation(Quaternion to, Quaternion from)
    {
        // Ensure both quaternions lie in the same hemisphere so the delta doesn't take the long way around
        if (Quaternion.Dot(to, from) < 0f)
        {
            to = new Quaternion(-to.x, -to.y, -to.z, -to.w);
        }

        return to * Quaternion.Inverse(from);
    }
    
}
