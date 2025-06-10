using UnityEngine;

public class HandTrackingManager : MonoBehaviour
{
    //[SerializeField] private OVRHand leftHand, rightHand;
    //[SerializeField] private PalmUIController palmUI;

    //void Update()
    //{
    //    if (IsPalmFacingUser(rightHand))
    //    {
    //        palmUI.ShowPalmMenu(rightHand.transform.position,
    //                           rightHand.transform.rotation);
    //    }
    //    else
    //    {
    //        palmUI.HidePalmMenu();
    //    }
    //}

    //bool IsPalmFacingUser(OVRHand hand)
    //{
    //    Vector3 palmNormal = hand.transform.up;
    //    Vector3 toCamera = Camera.main.transform.position - hand.transform.position;
    //    return Vector3.Dot(palmNormal, toCamera.normalized) > 0.5f;
    //}
}