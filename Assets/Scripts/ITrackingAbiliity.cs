using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMI
{
    public interface ITrackingAbility
    {
        public void Init();
        public void ProcessAbility();
        public void SetDebugElementsActive(bool active);
    }
}