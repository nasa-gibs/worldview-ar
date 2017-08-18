using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.VR;

/// <summary>
/// Simple script to interact with the IPointerHandlerVR script. 
/// All uGUI objects that you want to be able to interact with need to be on World Canvases and have colliders
/// Either the uGUI elements or this object needs to have a collider that is marked as a trigger.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hand : MonoBehaviour
{
    public VRNode vrNode;

    private new Transform transform;

    private IPointerHandlerVR isTouchingVrButton;


    private void Awake()
    {
        transform = GetComponent<Transform>();
        InputTracking.Recenter();
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localPosition = InputTracking.GetLocalPosition(vrNode);
        transform.localRotation = InputTracking.GetLocalRotation(vrNode);

        if (Input.GetButtonDown("VRSubmit") && isTouchingVrButton != null)
        {
            isTouchingVrButton.PointerDown(transform.position);
            print("Pointer Down");
        }

        if (Input.GetButtonUp("VRSubmit") && isTouchingVrButton != null)
        {
            isTouchingVrButton.PointerUp(transform.position);
            print("Pointer Up");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        ///**********************************************************************************************************************************************************************************************************
        /// https://blogs.unity3d.com/2014/05/16/custom-operator-should-we-keep-it/
        /// The c++ object becomes null but part of the c# unity wrapper hangs on so this check is actually important even though it seems insane
        /// the link above explains it.
        if (isTouchingVrButton != null && isTouchingVrButton.Equals(null))
        {
            isTouchingVrButton = null;
        }
        /// *********************************************************************************************************************************************************************************************************

        if (isTouchingVrButton != null  && !((MonoBehaviour)isTouchingVrButton).gameObject.Equals(other.gameObject))
        {
            isTouchingVrButton.PointerExit(transform.position);
            isTouchingVrButton = null;
        }
        if (other.GetComponent<IPointerHandlerVR>() != null)
        {
            isTouchingVrButton = other.GetComponent<IPointerHandlerVR>();
            isTouchingVrButton.PointerEnter(transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ///**********************************************************************************************************************************************************************************************************
        /// https://blogs.unity3d.com/2014/05/16/custom-operator-should-we-keep-it/
        /// The c++ object becomes null but part of the c# unity wrapper hangs on so this check is actually important even though it seems insane
        /// the link above explains it.
        if (isTouchingVrButton != null && isTouchingVrButton.Equals(null))
        {
            isTouchingVrButton = null;
        }
        /// *********************************************************************************************************************************************************************************************************

        if (isTouchingVrButton != null && ((MonoBehaviour)isTouchingVrButton).gameObject.Equals(other.gameObject))
        {
            isTouchingVrButton.PointerExit(transform.position);
            isTouchingVrButton = null;
        }
    }
}
