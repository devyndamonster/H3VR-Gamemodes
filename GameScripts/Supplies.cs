using FistVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Gamemodes
{
    public delegate void InteractionEvent();

    public class Supplies : FVRPhysicalObject
    {

        public event InteractionEvent ShowBuildableAreas;
        public event InteractionEvent HideBuildableAreas;

        public override void BeginInteraction(FVRViveHand hand)
        {
            base.BeginInteraction(hand);
            ShowBuildableAreas.Invoke();
            CancelInvoke();
        }

        public override void EndInteraction(FVRViveHand hand)
        {
            base.EndInteraction(hand);

            //If the other hand is currently holding supplies, don't hide areas
            if (hand.OtherHand.CurrentInteractable != null && hand.OtherHand.CurrentInteractable is Supplies) return;

            Invoke("DelayedHideBuildableAreas", 2);
        }

        public void DelayedHideBuildableAreas()
        {
            HideBuildableAreas.Invoke();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            FVRViveHand leftHand = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();
            FVRViveHand rightHand = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();

            //If the player is holding supplies in either hand, don't hide the areas
            if (leftHand.CurrentInteractable is Supplies) return;
            if (rightHand.CurrentInteractable is Supplies) return;

            HideBuildableAreas.Invoke();
        }

    }
}



