using System.Collections.Generic;
using UnityEngine;
using VRGIN.Core;

namespace VRGIN.Controls.Tools
{

    /// <summary>
    /// A tool that can be used with a Vive controller.
    /// </summary>
    public abstract class Tool : ProtectedBehaviour
    {

        protected SteamVR_TrackedObject Tracking;
        protected Controller Owner;
        protected Controller Neighbor;


        public abstract Texture2D Image
        {
            get;
        }

        public GameObject Icon
        {
            get; set;
        }

        protected override void OnStart()
        {
            base.OnStart();

            Tracking = GetComponent<SteamVR_TrackedObject>();
            Owner = GetComponent<Controller>();
            Neighbor = VR.Mode.Left == Owner ? VR.Mode.Right : VR.Mode.Left;
            VRLog.Debug(Neighbor ? "Got my neighbor!" : "No neighbor");
        }

        protected abstract void OnDestroy();


        /// <summary>
        /// Gets whether or not the attached controlller is tracking.
        /// </summary>
        protected bool IsTracking => Tracking && Tracking.isValid;

        /// <summary>
        /// Gets the attached controller input object.
        /// </summary>
        protected SteamVR_Controller.Device Controller => SteamVR_Controller.Input((int)Tracking.index);

        /// <summary>
        /// Gets the attached controller input object.
        /// </summary>
        protected Controller OtherController => Neighbor;


        protected virtual void OnEnable()
        {
            VRLog.Debug("On Enable: {0}", GetType().Name);
            if(Icon)
            {
                Icon.SetActive(true);
            }
            else
            {
                VRLog.Debug("But no icon...");

            }
        }

        protected virtual void OnDisable()
        {
            VRLog.Debug("On Disable: {0}", GetType().Name);
            if(Icon)
            {
                Icon.SetActive(false);
            }
            else
            {
                VRLog.Debug("But no icon...");
            }
        }

        public virtual List<HelpText> GetHelpTexts()
        {
            return new List<HelpText>();
        }

        protected Transform FindAttachPosition(params string[] names)
        {
            return Owner.FindAttachPosition(names);
        }

    }
}
