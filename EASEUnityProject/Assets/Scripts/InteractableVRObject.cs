//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Demonstrates how to create a simple interactable object
//
//=============================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(VelocityEstimator))]
    public class InteractableVRObject : MonoBehaviour
    {
        public bool grabbable;
        public bool throwable;
        public string displayedName;
        public string commandOnTrigger;
        public string commandOnDetached;
        bool startToTrackMe;
        public bool notRestoringGravity;

        private Vector3 oldPosition;
        private Quaternion oldRotation;
        
        [EnumFlags]
        [Tooltip("The flags used to attach this object to the hand.")]
        public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand;

        [Tooltip("Name of the attachment transform under in the hand's hierarchy which the object should should snap to.")]
        public string attachmentPoint;

        [Tooltip("How fast must this object be moving to attach due to a trigger hold instead of a trigger press?")]
        public float catchSpeedThreshold = 0.0f;

        [Tooltip("When detaching the object, should it return to its original parent?")]
        public bool restoreOriginalParent = false;

        public bool attachEaseIn = false;
        public AnimationCurve snapAttachEaseInCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        public float snapAttachEaseInTime = 0.15f;
        public string[] attachEaseInAttachmentNames;

        private VelocityEstimator velocityEstimator;
        private bool attached = false;
        private float attachTime;
        private Vector3 attachPosition;
        private Quaternion attachRotation;
        private Transform attachEaseInTransform;

        public UnityEvent onPickUp;
        public UnityEvent onDetachFromHand;

        public bool snapAttachEaseInCompleted = false;

        public bool canBeUsedMultipleTimesForGoalCompletion;
        public bool usedForGoalCompletion;

        //private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers);

        //-------------------------------------------------
        void Awake()
        {
            if(throwable)
            {
                velocityEstimator = GetComponent<VelocityEstimator>();

                if (attachEaseIn)
                {
                    attachmentFlags &= ~Hand.AttachmentFlags.SnapOnAttach;
                }

                Rigidbody rb = GetComponent<Rigidbody>();
                rb.maxAngularVelocity = 50.0f;
            }
        }

        //-------------------------------------------------
        // Called when a Hand starts hovering over this object
        //-------------------------------------------------
        private void OnHandHoverBegin(Hand hand)
        {
            if(displayedName.Length > 0)
                hand.gameObject.GetComponentInChildren<HandDisplay>().handDisplay(displayedName);

            if(throwable)
            {
                bool showHint = false;

                // "Catch" the throwable by holding down the interaction button instead of pressing it.
                // Only do this if the throwable is moving faster than the prescribed threshold speed,
                // and if it isn't attached to another hand
                if (!attached)
                {
                    if (hand.GetStandardInteractionButton())
                    {
                        Rigidbody rb = GetComponent<Rigidbody>();
                        if (rb.velocity.magnitude >= catchSpeedThreshold)
                        {
                            foreach(Hand.AttachedObject ao in hand.AttachedObjects)
                            {
                                if (ao.attachedObject.GetComponent<InteractableVRObject>() != null)
                                {
                                    Debug.Log("also attached: " + ao.attachedObject.GetComponent<InteractableVRObject>().displayedName);

                                    if (!ao.attachedObject.GetComponent<InteractableVRObject>().displayedName.Equals(displayedName))
                                        return;
                                }
                            }
                            hand.AttachObject(gameObject, attachmentFlags, attachmentPoint);
                            showHint = false;
                        }
                    }
                }

                if (showHint)
                {
                    ControllerButtonHints.ShowButtonHint(hand, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
                }
            }
        }


        //-------------------------------------------------
        // Called when a Hand stops hovering over this object
        //-------------------------------------------------
        private void OnHandHoverEnd(Hand hand)
        {
            if (displayedName.Length > 0)
                hand.gameObject.GetComponentInChildren<HandDisplay>().handDisplay("");

            if(throwable)
            {
                ControllerButtonHints.HideButtonHint(hand, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
            }
        }


        //-------------------------------------------------
        // Called every Update() while a Hand is hovering over this object
        //-------------------------------------------------
        private void HandHoverUpdate(Hand hand)
        {
            if(throwable)
            {
                //Trigger got pressed
                if (hand.GetStandardInteractionButtonDown())
                {
                    hand.AttachObject(gameObject, attachmentFlags, attachmentPoint);
                    ControllerButtonHints.HideButtonHint(hand, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
                    if (GetComponent<hasBeenAttached>() != null)
                        GetComponent<hasBeenAttached>().beenAttached = true;
                }
            }

            if (hand.GetStandardInteractionButtonDown() || ((hand.controller != null) && hand.controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip)))
            {
                if(grabbable)
                {
                    if (hand.currentAttachedObject != gameObject)
                    {
                        hand.HoverLock(GetComponent<Interactable>());

                        // Attach this object to the hand
                        hand.AttachObject(gameObject, attachmentFlags);
                        GetComponentInChildren<Rigidbody>().isKinematic = true;
                    }
                    else
                    {
                        // Detach this object from the hand
                        hand.DetachObject(gameObject);
                        GetComponentInChildren<Rigidbody>().isKinematic = false;

                        // Call this to undo HoverLock
                        hand.HoverUnlock(GetComponent<Interactable>());
                    }
                }
            }

            if (commandOnTrigger.Length > 0 && hand.GetStandardInteractionButtonUp())
            {
                GameObject.Find("LevelLogic").SendMessage(commandOnTrigger);
                if(GameObject.Find("TrackingLogic"))
                    GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().trackEvent(TrackingEvent.TrackingEventType.USAGE, gameObject);
            }

        }

        public Hand handAttachedTo;
        //-------------------------------------------------
        // Called when this GameObject becomes attached to the hand
        //-------------------------------------------------
        private void OnAttachedToHand(Hand hand)
        {
            attachTime = Time.time;
            handAttachedTo = hand;

            if (throwable)
            {
                attached = true;
                startToTrackMe = true; // for tracking, never set false again

                if(onPickUp != null)
                    onPickUp.Invoke();

                hand.HoverLock(null);

                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
                //rb.useGravity = false;
                rb.interpolation = RigidbodyInterpolation.None;
                if(GameObject.Find("TrackingLogic"))
                    GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().trackEvent(TrackingEvent.TrackingEventType.PICKUP, gameObject);

                if (hand.controller == null)
                {
                    velocityEstimator.BeginEstimatingVelocity();
                }

                attachTime = Time.time;
                attachPosition = transform.position;
                attachRotation = transform.rotation;

                if (attachEaseIn)
                {
                    attachEaseInTransform = hand.transform;
                    if (!Util.IsNullOrEmpty(attachEaseInAttachmentNames))
                    {
                        float smallestAngle = float.MaxValue;
                        for (int i = 0; i < attachEaseInAttachmentNames.Length; i++)
                        {
                            Transform t = hand.GetAttachmentTransform(attachEaseInAttachmentNames[i]);
                            float angle = Quaternion.Angle(t.rotation, attachRotation);
                            if (angle < smallestAngle)
                            {
                                attachEaseInTransform = t;
                                smallestAngle = angle;
                            }
                        }
                    }
                }

                snapAttachEaseInCompleted = false;
            }
        }


        //-------------------------------------------------
        // Called when this GameObject is detached from the hand
        //-------------------------------------------------
        private void OnDetachedFromHand(Hand hand)
        {
            handAttachedTo = null;

            if (throwable)
            {
                attached = false;

                if(onDetachFromHand != null)
                    onDetachFromHand.Invoke();

                hand.HoverUnlock(null);

                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                if(!notRestoringGravity)
                    rb.useGravity = true;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                if(GameObject.Find("TrackingLogic"))
                    GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().trackEvent(TrackingEvent.TrackingEventType.PUTDOWN, gameObject);

                Vector3 position = Vector3.zero;
                Vector3 velocity = Vector3.zero;
                Vector3 angularVelocity = Vector3.zero;
                if (hand.controller == null)
                {
                    velocityEstimator.FinishEstimatingVelocity();
                    velocity = velocityEstimator.GetVelocityEstimate();
                    angularVelocity = velocityEstimator.GetAngularVelocityEstimate();
                    position = velocityEstimator.transform.position;
                }
                else
                {
                    velocity = Player.instance.trackingOriginTransform.TransformVector(hand.controller.velocity);
                    angularVelocity = Player.instance.trackingOriginTransform.TransformVector(hand.controller.angularVelocity);
                    position = hand.transform.position;
                }

                Vector3 r = transform.TransformPoint(rb.centerOfMass) - position;
                rb.velocity = velocity + Vector3.Cross(angularVelocity, r);
                rb.angularVelocity = angularVelocity;

                // Make the object travel at the release velocity for the amount
                // of time it will take until the next fixed update, at which
                // point Unity physics will take over
                float timeUntilFixedUpdate = (Time.fixedDeltaTime + Time.fixedTime) - Time.time;
                transform.position += timeUntilFixedUpdate * velocity;
                float angle = Mathf.Rad2Deg * angularVelocity.magnitude;
                Vector3 axis = angularVelocity.normalized;
                transform.rotation *= Quaternion.AngleAxis(angle * timeUntilFixedUpdate, axis);
            }
            
            if (commandOnDetached != null && commandOnDetached.Length > 0)
            {
                GameObject.Find("LevelLogic").SendMessage(commandOnDetached);
            }

            float f = 0f;
            foreach(multiAttached ma in GetComponentsInChildren<multiAttached>())
            {
                f += 0.1f;
                ma.detachMeAfter(f);
            }
        }


        //-------------------------------------------------
        // Called every Update() while this GameObject is attached to the hand
        //-------------------------------------------------
        private void HandAttachedUpdate(Hand hand)
        {
            if(throwable)
            {
                //Trigger got released
                if (!hand.GetStandardInteractionButton())
                {
                    // Detach ourselves late in the frame.
                    // This is so that any vehicles the player is attached to
                    // have a chance to finish updating themselves.
                    // If we detach now, our position could be behind what it
                    // will be at the end of the frame, and the object may appear
                    // to teleport behind the hand when the player releases it.
                    StartCoroutine(LateDetach(hand));
                }

                if (attachEaseIn)
                {
                    float t = Util.RemapNumberClamped(Time.time, attachTime, attachTime + snapAttachEaseInTime, 0.0f, 1.0f);
                    if (t < 1.0f)
                    {
                        t = snapAttachEaseInCurve.Evaluate(t);
                        transform.position = Vector3.Lerp(attachPosition, attachEaseInTransform.position, t);
                        transform.rotation = Quaternion.Lerp(attachRotation, attachEaseInTransform.rotation, t);
                    }
                    else if (!snapAttachEaseInCompleted)
                    {
                        gameObject.SendMessage("OnThrowableAttachEaseInCompleted", hand, SendMessageOptions.DontRequireReceiver);
                        snapAttachEaseInCompleted = true;
                    }
                }
            }
        }

        //-------------------------------------------------
        private IEnumerator LateDetach(Hand hand)
        {
            yield return new WaitForEndOfFrame();
            if (throwable)
            {
                hand.DetachObject(gameObject, restoreOriginalParent);
            }
        }


        //-------------------------------------------------
        private void OnHandFocusAcquired(Hand hand)
        {
            if (throwable)
            {
                gameObject.SetActive(true);
                if(velocityEstimator)
                    velocityEstimator.BeginEstimatingVelocity();
            }
        }


        //-------------------------------------------------
        private void OnHandFocusLost(Hand hand)
        {
            if (throwable)
            {
                gameObject.SetActive(false);
                if (velocityEstimator)
                    velocityEstimator.FinishEstimatingVelocity();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!startToTrackMe)
                return;

            if(attached)
            {
                if(GameObject.Find("TrackingLogic"))
                    GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().trackEvent(TrackingEvent.TrackingEventType.COLLISION_WHILE_HELD, gameObject, collision.transform.gameObject);
            }
            else
                if (GameObject.Find("TrackingLogic"))
                    GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().trackEvent(TrackingEvent.TrackingEventType.COLLISION, gameObject, collision.transform.gameObject);
            if (collision.transform.GetComponent<InteractableVRObject>())
                collision.transform.GetComponent<InteractableVRObject>().startToTrackMe = true;


            if(handAttachedTo)
            {
                // multi-collecting
                foreach (Hand.AttachedObject ao in handAttachedTo.AttachedObjects)
                {
                    if (ao.attachedObject.GetComponent<InteractableVRObject>() && collision.gameObject.GetComponent<InteractableVRObject>() && 
                        ao.attachedObject.GetComponent<InteractableVRObject>().displayedName == collision.gameObject.GetComponent<InteractableVRObject>().displayedName )
                    {
                        //handAttachedTo.AttachObject(collision.gameObject, Hand.AttachmentFlags.ParentToHand);
                        collision.transform.parent = transform;
                        if(collision.gameObject.GetComponent<Rigidbody>())
                        {
                            collision.gameObject.GetComponent<Rigidbody>().useGravity = false;
                            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        }
                        foreach (Collider c in collision.gameObject.GetComponentsInChildren<Collider>())
                            c.enabled = false;
                        collision.gameObject.AddComponent<multiAttached>();
                        break;
                    }
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!startToTrackMe)
                return;
            if (GameObject.Find("TrackingLogic"))
                GameObject.Find("TrackingLogic").GetComponent<TrackingLogic>().trackEvent(TrackingEvent.TrackingEventType.COLLISION_EXIT, gameObject, collision.transform.gameObject);
        }
    }
}
