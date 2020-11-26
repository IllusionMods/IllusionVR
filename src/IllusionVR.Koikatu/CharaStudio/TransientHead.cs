using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Helpers;

namespace IllusionVR.Koikatu.CharaStudio
{
    public class TransientHead : ProtectedBehaviour
    {
        private List<Renderer> rendererList = new List<Renderer>();

        private bool hidden;

        private Transform root;

        private Renderer[] m_tongues;

        private ChaControl avatar;

        private Transform headTransform;

        private Transform eyesTransform;

        public Transform Eyes => eyesTransform;

        public bool Visible
        {
            get => !hidden;
            set
            {
                if(value)
                {
                    Console.WriteLine("SHOW");
                }
                else
                {
                    Console.WriteLine("HIDE");
                }
                SetVisibility(value);
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            avatar = GetComponent<ChaControl>();
            Reinitialize();
        }

        public void Reinitialize()
        {
            headTransform = GetHead(avatar);
            eyesTransform = GetEyes(avatar);
            root = avatar.objRoot.transform;
            m_tongues = (from renderer in root.GetComponentsInChildren<SkinnedMeshRenderer>()
                         where renderer.name.ToLower().StartsWith("cm_o_tang") || renderer.name == "cf_o_tang"
                         select renderer into tongue
                         where tongue.enabled
                         select tongue).ToArray<SkinnedMeshRenderer>();
        }

        public static Transform GetHead(ChaControl human)
        {
            return human.objHead.GetComponentsInParent<Transform>().First((Transform t) => t.name.StartsWith("c") && t.name.ToLower().Contains("j_head"));
        }

        public static Transform GetEyes(ChaControl human)
        {
            Transform transform = human.objHeadBone.transform.Descendants().FirstOrDefault((Transform t) => t.name.StartsWith("c") && t.name.ToLower().EndsWith("j_faceup_tz"));
            if(!transform)
            {
                VRLog.Debug("Creating eyes", new object[0]);
                transform = new GameObject("cf_j_faceup_tz").transform;
                transform.SetParent(GetHead(human), false);
                transform.transform.localPosition = new Vector3(0f, 0.07f, 0.05f);
            }
            else
            {
                VRLog.Debug("found eyes", new object[0]);
            }
            return transform;
        }

        private void SetVisibility(bool visible)
        {
            if(visible)
            {
                if(hidden)
                {
                    foreach(Renderer renderer4 in rendererList)
                    {
                        if(renderer4)
                        {
                            renderer4.enabled = true;
                        }
                    }
                    foreach(Renderer renderer2 in m_tongues)
                    {
                        if(renderer2)
                        {
                            renderer2.enabled = true;
                        }
                    }
                }
            }
            else if(!hidden)
            {
                m_tongues = (from renderer in root.GetComponentsInChildren<SkinnedMeshRenderer>()
                             where renderer.name.StartsWith("cm_o_tang") || renderer.name == "cf_o_tang"
                             select renderer into tongue
                             where tongue.enabled
                             select tongue).ToArray<SkinnedMeshRenderer>();
                rendererList.Clear();
                foreach(Renderer renderer3 in from renderer in headTransform.GetComponentsInChildren<Renderer>()
                                              where renderer.enabled
                                              select renderer)
                {
                    rendererList.Add(renderer3);
                    renderer3.enabled = false;
                }
                Renderer[] tongues = m_tongues;
                for(int i = 0; i < tongues.Length; i++)
                {
                    tongues[i].enabled = false;
                }
            }
            hidden = !visible;
        }
    }
}
