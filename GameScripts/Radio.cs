using FistVR;
using KOTH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



namespace SosigRadio
{
    public class Radio : FVRPhysicalObject
    {

        [Header("Radio Settings")]

        public float pointDistance = 50f;

        public float followUpdateDelay = 1f;

        public LayerMask sosigLayer;

        public LayerMask environmentLayer;

        [HideInInspector]
        public Quaternion upRotation;

        [HideInInspector]
        public Sosig hoveredSosig;

        [HideInInspector]
        public List<Sosig> selectedSosigs = new List<Sosig>();

        [HideInInspector]
        public List<Sosig> followingSosigs = new List<Sosig>();

        
        private bool radioActive;
        private float currentUpdateTime = 0f;
        private int currentFollowingSosigIndex = 0;


        public override void Awake()
        {
            base.Awake();

            upRotation = Quaternion.LookRotation(Vector3.up);
        }

        public override void UpdateInteraction(FVRViveHand hand)
        {
            base.UpdateInteraction(hand);

            if(hand.Input.TriggerFloat > 0.5f)
            {
                radioActive = true;
            }
            else
            {
                radioActive = false;
                selectedSosigs.Clear();
                hoveredSosig = null;
            }


            if (IsHandSelecting(hand))
            {
                if(hoveredSosig != null && !selectedSosigs.Contains(hoveredSosig) && hoveredSosig.E.IFFCode == GM.CurrentPlayerBody.GetPlayerIFF())
                {
                    selectedSosigs.Add(hoveredSosig);
                }
            }

            if (IsHandCommanding(hand))
            {
                Debug.Log("Command button pressed");

                CommandSelectedSosigs();
            }
        }


        private bool IsHandSelecting(FVRViveHand hand)
        {
            return hand.IsInStreamlinedMode && hand.Input.AXButtonPressed || !hand.IsInStreamlinedMode && hand.Input.TouchpadNorthPressed;
        }

        private bool IsHandCommanding(FVRViveHand hand)
        {
            return hand.IsInStreamlinedMode && hand.Input.BYButtonDown || !hand.IsInStreamlinedMode && hand.Input.TouchpadSouthDown;
        }


        public override void EndInteraction(FVRViveHand hand)
        {
            base.EndInteraction(hand);

            radioActive = false;
            selectedSosigs.Clear();
            hoveredSosig = null;
        }

        public override void FVRUpdate()
        {
            base.FVRUpdate();

            if (radioActive)
            {
                UpdateRadioPointer();
                UpdateSelectedSosigs();
            }

            UpdateFollowingSosigs();
        }


        private void UpdateSelectedSosigs()
        {
            for (int i = 0; i < selectedSosigs.Count; i++)
            {
                //Remove dead sosigs
                if (selectedSosigs[i] == null || selectedSosigs[i].Links.Count < 4 || selectedSosigs[i].Links[3] == null)
                {
                    selectedSosigs.RemoveAt(i);
                    i--;
                    continue;
                }

                if (selectedSosigs.Count == 0) return;

                DrawSosigSelection(selectedSosigs[i]);
            }
        }


        private void UpdateFollowingSosigs()
        {
            currentUpdateTime -= Time.deltaTime;
            if (currentUpdateTime < 0)
            {
                currentUpdateTime = followUpdateDelay;

                //Loop until you find a valid sosig to update, or have removed all null sosigs
                while (followingSosigs.Count > 0)
                {
                    currentFollowingSosigIndex += 1;
                    if (currentFollowingSosigIndex >= followingSosigs.Count) currentFollowingSosigIndex = 0;

                    if (followingSosigs[currentFollowingSosigIndex] == null)
                    {
                        followingSosigs.RemoveAt(currentFollowingSosigIndex);
                    }

                    else
                    {
                        //Have the sosig follow the point behind the players head
                        Vector3 followPosition = GM.CurrentPlayerBody.Head.position - GM.CurrentPlayerBody.Head.forward;

                        if(Vector3.Distance(followPosition, followingSosigs[currentFollowingSosigIndex].m_assaultPoint) > 1)
                        {
                            followingSosigs[currentFollowingSosigIndex].CommandAssaultPoint(followPosition);
                        }

                        return;
                    }
                }
            }
        }


        private void UpdateRadioPointer()
        {
            RaycastHit hit;
            if(Physics.Raycast(m_hand.OtherHand.transform.position, m_hand.OtherHand.PointingTransform.forward, out hit, pointDistance, sosigLayer))
            {
                SosigLink link = hit.collider.GetComponentInParent<SosigLink>();

                Debug.Log("Hit agent body! " + link);

                if(link != null)
                {
                    if(link.S.E.IFFCode != GM.CurrentPlayerBody.GetPlayerIFF())
                    {
                        DrawRadioPointer(Color.red);
                    }
                    else if (selectedSosigs.Contains(link.S))
                    {
                        DrawRadioPointer(Color.green);
                    }
                    else
                    {
                        DrawRadioPointer(Color.yellow);
                    }

                    hoveredSosig = link.S;
                    return;
                }
            }

            hoveredSosig = null;
            DrawRadioPointer(Color.white);
        }

        private void DrawRadioPointer(Color color)
        {
            Popcron.Gizmos.Line(m_hand.OtherHand.transform.position, m_hand.OtherHand.transform.position + m_hand.OtherHand.PointingTransform.forward * pointDistance, color);
        }

        private void DrawSosigSelection(Sosig sosig)
        {
            Popcron.Gizmos.Circle(sosig.Links[3].transform.position, 0.2f, upRotation, Color.green);

            if (followingSosigs.Contains(sosig))
            {
                Popcron.Gizmos.Line(sosig.Links[3].transform.position, GM.CurrentPlayerBody.transform.position, Color.blue);
            }

            else if(sosig.FallbackOrder == Sosig.SosigOrder.Assault)
            {
                Popcron.Gizmos.Line(sosig.Links[3].transform.position, sosig.m_assaultPoint, Color.blue);
            }
        }

        private void CommandSelectedSosigs()
        {
            RaycastHit hit;
            if (Physics.Raycast(m_hand.OtherHand.transform.position, m_hand.OtherHand.PointingTransform.forward, out hit, pointDistance, environmentLayer))
            {
                //If commanded at the players feet, they should follow the player
                if (Vector3.Distance(GM.CurrentPlayerBody.transform.position, hit.point) < 1f){

                    foreach (Sosig sosig in selectedSosigs)
                    {
                        if (sosig == null || followingSosigs.Contains(sosig)) continue;

                        followingSosigs.Add(sosig);
                        sosig.CommandAssaultPoint(hit.point);
                    }
                }

                else
                {
                    Debug.Log("Commanding sosigs to assault point: " + hit.point.ToString());

                    foreach (Sosig sosig in selectedSosigs)
                    {
                        if (sosig == null) continue;

                        if (followingSosigs.Contains(sosig)) followingSosigs.Remove(sosig);

                        sosig.CommandAssaultPoint(hit.point);
                    }
                }
                
            }
        }
    }

}


