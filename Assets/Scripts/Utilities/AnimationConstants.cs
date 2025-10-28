using UnityEngine;

namespace Utilities
{
    public static class AnimationConstants
    {
        public static readonly int SPEED = Animator.StringToHash("Speed");
        public static readonly int IS_GATHERING = Animator.StringToHash("IsGathering");
        public static readonly int IS_BUILDING = Animator.StringToHash("IsBuilding");
        public static readonly int ATTACK = Animator.StringToHash("Attack");
    }
}