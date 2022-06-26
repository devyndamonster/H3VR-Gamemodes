using FistVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamemodes
{
    public class CapturablePoint : MonoBehaviour
    {
        public List<MeshRenderer> pointRenderers = new List<MeshRenderer>();
        public float captureRate = 0.1f;

        [HideInInspector]
        public List<SosigTeamMember> capturingSosigs = new List<SosigTeamMember>();

        [HideInInspector]
        public List<int> captureCount = new List<int>();

        [HideInInspector]
        public int currentTeam = -1;

        [HideInInspector]
        public bool isPlayerInPoint;

        protected float timeTillCaptureUpdate = 0;
        protected float captureUpdateFrequency = 0;
        protected float captureProgress = 0;
        protected List<Team> teams = new List<Team> ();

        public virtual void ResetPoint(List<Team> teams, float updateFrequency)
        {
            for (int i = 0; i < teams.Count; i++)
            {
                captureCount.Add(0);
            }
            this.teams = teams;

            captureUpdateFrequency = updateFrequency;
            
        }

        protected virtual void Update()
        {
            if (teams.Count <= 0) return;

            timeTillCaptureUpdate -= Time.deltaTime;

            if (timeTillCaptureUpdate <= 0)
            {
                timeTillCaptureUpdate = captureUpdateFrequency;
                CaptureUpdate();
            }
        }

        protected void ResetCaptureCount()
        {
            for (int i = 0; i < captureCount.Count; i++)
            {
                captureCount[i] = 0;
            }
        }

        public virtual void ForceCapturePointForTeam(int team)
        {
            GivePointToTeam(team);
            captureProgress = 1;
            UpdatePointRenderers();
            PointCaptured();
        }

        public virtual void ForceNeutralizePoint()
        {
            captureProgress = 0;
            currentTeam = -1;
            PointNeutralized();
            SetRendererColor(Color.gray);
            UpdatePointRenderers();
        }

        protected virtual int UpdateCaptureCount()
        {
            ResetCaptureCount();

            if (IsPlayerCapturing()) captureCount[0] += 1;

            int maxIndex = 0;
            for (int i = 0; i < capturingSosigs.Count; i++)
            {
                SosigTeamMember sosig = capturingSosigs[i];

                if (sosig == null)
                {
                    capturingSosigs.RemoveAt(i);
                    i -= 1;
                    continue;
                }

                if (sosig.sosig.E.IFFCode < 0 || sosig.sosig.E.IFFCode >= captureCount.Count)
                {
                    continue;
                }

                captureCount[sosig.sosig.E.IFFCode] += 1;

                if (captureCount[sosig.sosig.E.IFFCode] > captureCount[maxIndex]) maxIndex = sosig.sosig.E.IFFCode;
            }

            return maxIndex;
        }


        protected virtual void CaptureUpdate()
        {
            int dominantTeam = UpdateCaptureCount();

            if (captureCount[dominantTeam] > 0)
            {
                float previousCaptureProgress = captureProgress;
                UpdateCaptureProgress(dominantTeam);

                if (WasPointNeutralized(previousCaptureProgress))
                {
                    ForceNeutralizePoint();
                }

                else if(WasPointCaptured(previousCaptureProgress))
                {
                    PointCaptured();
                }

                else if(IsPointNeutral() && !WasPointNeutralized(previousCaptureProgress))
                {
                    GivePointToTeam(dominantTeam);
                }

                UpdatePointRenderers();
            }
        }

        protected virtual void UpdatePointRenderers()
        {
            foreach (MeshRenderer renderer in pointRenderers)
            {
                renderer.material.SetFloat("_CaptureProgress", captureProgress);
            }
        }

        protected virtual void GivePointToTeam(int team)
        {
            currentTeam = team;
            SetRendererColor(teams[team].teamColor);
        }

        protected virtual void SetRendererColor(Color color)
        {
            foreach (MeshRenderer renderer in pointRenderers)
            {
                renderer.material.SetColor("_TeamColor", color);
            }
        }

        protected virtual void UpdateCaptureProgress(int dominantTeam)
        {
            if (!DoesTeamControlPoint(dominantTeam))
            {
                captureProgress = Mathf.Max(captureProgress - captureRate, 0);
            }
            else
            {
                captureProgress = Mathf.Min(captureProgress + captureRate, 1);
            }
        }

        protected virtual void PointCaptured() { }

        protected virtual void PointNeutralized() { }

        public bool IsPlayerCapturing()
        {
            return isPlayerInPoint && PlayerTracker.currentPlayerState == PlayerState.playing;
        }

        public bool IsPointNeutral()
        {
            return captureProgress <= 0;
        }

        public bool DoesTeamControlPoint(int team)
        {
            return currentTeam == team;
        }

        public bool IsPointFullCaptured()
        {
            return captureProgress >= 1;
        }

        public bool WasPointNeutralized(float previousCaptureProgress)
        {
            return previousCaptureProgress > 0 && IsPointNeutral();
        }

        public bool WasPointCaptured(float previousCaptureProgress)
        {
            return previousCaptureProgress < 1 && captureProgress >= 1;
        }

        private void OnTriggerEnter(Collider other)
        {
            SosigTeamMember sosig = other.GetComponent<SosigTeamMember>();
            if (sosig != null)
            {
                SosigEnteredPoint(sosig);
                return;
            }

            FVRPlayerHitbox player = other.GetComponent<FVRPlayerHitbox>();
            if (player != null)
            {
                PlayerEnteredPoint();
                return;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            SosigTeamMember sosig = other.GetComponent<SosigTeamMember>();
            if (sosig != null)
            {
                SosigExitedPoint(sosig);
                return;
            }

            FVRPlayerHitbox player = other.GetComponent<FVRPlayerHitbox>();
            if (player != null)
            {
                PlayerExitedPoint();
                return;
            }
        }

        protected virtual void SosigEnteredPoint(SosigTeamMember sosig) 
        {
            if (!capturingSosigs.Contains(sosig))
            {
                capturingSosigs.Add(sosig);
            }
        }

        protected virtual void SosigExitedPoint(SosigTeamMember sosig) 
        {
            capturingSosigs.RemoveAll(o => o == null || o.Equals(sosig));
        }

        protected virtual void PlayerEnteredPoint() 
        {
            isPlayerInPoint = true;
        }

        protected virtual void PlayerExitedPoint()
        {
            isPlayerInPoint = false;
        }
    }
}

