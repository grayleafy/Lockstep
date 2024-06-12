using FixMath.NET;
using System;

namespace BEPUutilities
{
    ///<summary>
    /// Contains ray hit data.
    ///</summary>
    [Serializable]
    public struct RayHit
    {
        ///<summary>
        /// Location of the ray hit.
        ///</summary>
        public Vector3 Location;
        ///<summary>
        /// Normal of the ray hit.
        ///</summary>
        public Vector3 Normal;
        ///<summary>
        /// T parameter of the ray hit.  
        /// The ray hit location is equal to the ray origin added to the ray direction multiplied by T.
        ///</summary>
        public Fix64 T;
    }
}
