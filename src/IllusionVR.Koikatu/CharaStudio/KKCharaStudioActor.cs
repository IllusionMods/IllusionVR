using UnityEngine;
using VRGIN.Core;
using VRGIN.Helpers;

namespace KKCharaStudioVR
{
    public class KKCharaStudioActor : DefaultActorBehaviour<ChaControl>
    {
        private LookTargetController _TargetController;

        public TransientHead Head { get; private set; }

        protected override void Initialize(ChaControl actor)
        {
            base.Initialize(actor);
            Head = actor.gameObject.AddComponent<TransientHead>();
        }

        public override Transform Eyes => Head.Eyes;

        public override bool HasHead
        {
            get => Head.Visible;
            set => Head.Visible = value;
        }

        public bool IsFemale => Actor.sex == 1;

        protected override void OnStart()
        {
            base.OnStart();
            _TargetController = LookTargetController.AttachTo(this, gameObject);
        }

        protected override void OnLevel(int level)
        {
            base.OnLevel(level);
        }

        private void InitializeDynamicBoneColliders()
        {
            DynamicBone[] array = FindObjectsOfType<DynamicBone>();
            for(int i = 0; i < array.Length; i++)
            {
                array[i].m_UpdateRate = 90f;
            }
            DynamicBone_Ver01[] array2 = FindObjectsOfType<DynamicBone_Ver01>();
            for(int i = 0; i < array2.Length; i++)
            {
                array2[i].m_UpdateRate = 90f;
            }
            DynamicBone_Ver02[] array3 = FindObjectsOfType<DynamicBone_Ver02>();
            for(int i = 0; i < array3.Length; i++)
            {
                array3[i].UpdateRate = 90f;
            }
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
            EyeLookController eyeLookCtrl = Actor.eyeLookCtrl;
            NeckLookControllerVer2 neckLookCtrl = Actor.neckLookCtrl;
            Transform transform = Camera.main.transform;
            if(transform)
            {
                if(eyeLookCtrl && eyeLookCtrl.target == transform)
                {
                    eyeLookCtrl.target = _TargetController.Target;
                }
                if(neckLookCtrl && neckLookCtrl.target == transform)
                {
                    neckLookCtrl.target = _TargetController.Target;
                }
            }
        }

        internal void OnVRModeChanged(bool newMode)
        {
            if(_TargetController != null && !newMode)
            {
                EyeLookController eyeLookCtrl = Actor.eyeLookCtrl;
                NeckLookControllerVer2 neckLookCtrl = Actor.neckLookCtrl;
                Transform transform = Camera.main.transform;
                if(transform)
                {
                    if(eyeLookCtrl && eyeLookCtrl.target == _TargetController.Target)
                    {
                        eyeLookCtrl.target = transform;
                    }
                    if(neckLookCtrl && neckLookCtrl.target == _TargetController.Target)
                    {
                        neckLookCtrl.target = transform;
                    }
                }
            }
        }
    }
}
