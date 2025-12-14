using System;

namespace Lucky.Utilities
{
    public class LerpThenFollowValue<T>
    {
        private Action lerpAction;
        private Action followeAction;
        private Func<bool> closeCondition;

        private bool follow = true;

        public LerpThenFollowValue(Action lerpAction, Action followeAction, Func<bool> closeCondition)
        {
            this.lerpAction = lerpAction;
            this.followeAction = followeAction;
            this.closeCondition = closeCondition;
        }

        public void Update()
        {
            if (closeCondition())
                follow = true;

            if (follow)
                followeAction();
            else
                lerpAction();
        }

        public void FollowMode() => follow = true;
        public void LerpMode() => follow = false;
    }
}