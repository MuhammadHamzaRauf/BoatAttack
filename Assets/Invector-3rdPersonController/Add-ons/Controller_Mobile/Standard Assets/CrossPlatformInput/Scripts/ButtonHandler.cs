using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class ButtonHandler : MonoBehaviour
    {

        public string Name;
        public InvectorJoystick IJS;

        void OnEnable()
        {

        }

        public void SetDownState()
        {
            CrossPlatformInputManager.SetButtonDown(Name);
            if (IJS != null)
                IJS.UpdateVirtualAxes(new Vector3(0, 100, 0));
        }


        public void SetUpState()
        {
            CrossPlatformInputManager.SetButtonUp(Name);
            if (IJS != null)
                IJS.UpdateVirtualAxes(Vector3.zero);
        }


        public void SetAxisPositiveState()
        {
            CrossPlatformInputManager.SetAxisPositive(Name);
        }


        public void SetAxisNeutralState()
        {
            CrossPlatformInputManager.SetAxisZero(Name);
        }


        public void SetAxisNegativeState()
        {
            CrossPlatformInputManager.SetAxisNegative(Name);
        }

        //public void Update()
        //{

        //}
    }
}
