using Studio;
using System.Collections.Generic;
using UnityEngine;

namespace IllusionVR.Koikatu.CharaStudio
{
    internal class ObjMoveHelper
    {
        public Vector3 moveAlongBasePos;

        public Quaternion moveAlongBaseRot;

        public void SetBasePos(Vector3 basePos)
        {
            moveAlongBasePos = basePos;
        }

        public ObjectCtrlInfo GetFirstObject()
        {
            var instance = Singleton<Studio.Studio>.Instance;
            if(instance != null)
            {
                ObjectCtrlInfo[] selectObjectCtrl = instance.treeNodeCtrl.selectObjectCtrl;
                if(selectObjectCtrl != null && selectObjectCtrl.Length != 0)
                {
                    return selectObjectCtrl[0];
                }
            }
            return null;
        }

        public void MoveAllCharaAndItemsHere(Vector3 newPos, bool keepY = true)
        {
            var instance = Singleton<Studio.Studio>.Instance;
            if(instance == null)
            {
                return;
            }
            Vector3 vector = newPos - moveAlongBasePos;
            if(keepY)
            {
                vector.y = 0f;
            }
            new Dictionary<Transform, Transform>();
            List<GuideCommand.EqualsInfo> list = new List<GuideCommand.EqualsInfo>();
            ObjectCtrlInfo[] selectObjectCtrl = instance.treeNodeCtrl.selectObjectCtrl;
            for(int i = 0; i < selectObjectCtrl.Length; i++)
            {
                GuideObject guideObject = selectObjectCtrl[i].guideObject;
                if(guideObject != null)
                {
                    Vector3 localPosition = guideObject.transformTarget.localPosition;
                    guideObject.transformTarget.position += vector;
                    guideObject.changeAmount.pos = guideObject.transformTarget.localPosition;
                    if(guideObject.enablePos)
                    {
                        GuideCommand.EqualsInfo item = new GuideCommand.EqualsInfo
                        {
                            dicKey = guideObject.dicKey,
                            oldValue = localPosition,
                            newValue = guideObject.changeAmount.pos
                        };
                        list.Add(item);
                    }
                }
                Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.MoveEqualsCommand(list.ToArray()));
            }
        }

        public void MoveObject(ObjectCtrlInfo oci, Vector3 newPos, bool keepY)
        {
            if(keepY)
            {
                newPos.y = oci.guideObject.transformTarget.position.y;
            }
            GuideObject guideObject = oci.guideObject;
            if(guideObject != null)
            {
                Vector3 localPosition = guideObject.transformTarget.localPosition;
                guideObject.transformTarget.position = newPos;
                guideObject.changeAmount.pos = guideObject.transformTarget.localPosition;
                if(guideObject.enablePos)
                {
                    GuideCommand.EqualsInfo equalsInfo = new GuideCommand.EqualsInfo
                    {
                        dicKey = guideObject.dicKey,
                        oldValue = localPosition,
                        newValue = guideObject.changeAmount.pos
                    };
                    Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.MoveEqualsCommand(new GuideCommand.EqualsInfo[]
                    {
                        equalsInfo
                    }));
                }
            }
        }
    }
}
