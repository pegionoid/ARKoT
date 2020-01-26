using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
public class AugmentedImageSample : MonoBehaviour
{

    private List<AugmentedImage> augmentedImageList = new List<AugmentedImage>();
    [SerializeField] private GameObject ObjPrefab;
    private GameObject arObj = null;

    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return; //トラッキングできてないのでreturn
        }

        //ARCoreがトラッキングしているものを取得する。
        Session.GetTrackables<AugmentedImage>(augmentedImageList, TrackableQueryFilter.Updated);

        foreach (AugmentedImage image in augmentedImageList)
        {
            if (image.TrackingState == TrackingState.Tracking)
            {
                if (arObj == null)
                {
                    //トラッキング中かつarObjがNullなのでオブジェクトを生成する
                    Anchor anchor = image.CreateAnchor(image.CenterPose);
                    arObj = Instantiate(ObjPrefab, anchor.transform);

                }
                else
                {
                    //トラッキング中かつarObjが既に存在するので位置・回転の更新を行う。
                    arObj.transform.position = image.CenterPose.position;
                    arObj.transform.rotation = image.CenterPose.rotation;
                }
            }

        }
    }
}