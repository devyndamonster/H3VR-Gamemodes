using Sodalite.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.Conquest
{
	public class ConquestPoint : CapturablePoint
	{
        public int StartingTeam = -1;

        public Transform AttackPointContainer;
        public Transform DefendPointContainer;
        public Transform SpawnPointContainer;
        public string PointName = "A";
        public List<ConquestPoint> Neighbors = new List<ConquestPoint>();

        [HideInInspector]
        public Dictionary<ConquestPoint, bool> AttackNeighborPoints = new Dictionary<ConquestPoint, bool>();
        [HideInInspector]
        public bool DefendThisPoint;
        [HideInInspector]
		public List<Transform> AttackPoints = new List<Transform>();
        [HideInInspector]
        public List<Transform> DefendPoints = new List<Transform>();
        [HideInInspector]
        public List<SpawnArea> SpawnAreas = new List<SpawnArea>();
        

        protected virtual void Awake()
        {
            AttackPoints = AttackPointContainer.Cast<Transform>().ToList();
            DefendPoints = DefendPointContainer.Cast<Transform>().ToList();
            SpawnAreas = SpawnPointContainer.GetComponentsInChildren<SpawnArea>().ToList();
            Neighbors.ForEach(o => AttackNeighborPoints[o] = true);
        }

        public override void ResetPoint(List<Team> teams, float updateFrequency)
        {
            base.ResetPoint(teams, updateFrequency);

            if(StartingTeam >= 0)
            {
                ForceCapturePointForTeam(StartingTeam);
            }

            else
            {
                ForceNeutralizePoint();
            }
        }

        public void ToggleDefendStrategy()
        {
            DefendThisPoint = !DefendThisPoint;

            if (!DefendThisPoint)
            {
                foreach (ConquestSosig sosig in capturingSosigs)
                {
                    ConquestPoint nextPoint = GetNeighboringPointToAttack();
                    sosig.OrderToAssault(nextPoint);
                }
            }
        }

        protected override void PointCaptured()
        {
            base.PointCaptured();

            foreach(ConquestSosig sosig in capturingSosigs)
            {
                if (DefendThisPoint)
                {
                    sosig.OrderToDefend(this);
                }
                else
                {
                    ConquestPoint nextPoint = GetNeighboringPointToAttack();
                    sosig.OrderToAssault(nextPoint);
                }
            }
        }

        protected override void PointNeutralized()
        {
            base.PointNeutralized();

            foreach (ConquestSosig sosig in capturingSosigs)
            {
                sosig.OrderToDefend(this);
            }
        }

        protected override void SosigEnteredPoint(SosigTeamMember sosig)
        {
            base.SosigEnteredPoint(sosig);

            if (sosig is ConquestSosig && DoesTeamControlPoint(sosig.sosig.GetIFF()) && IsPointFullCaptured())
            {
                ((ConquestSosig)sosig).OrderToAssault(GetNeighboringPointToAttack());
            }
        }

        protected override void SosigExitedPoint(SosigTeamMember sosig)
        {
            base.SosigExitedPoint(sosig);

            if (sosig is ConquestSosig && (!DoesTeamControlPoint(sosig.sosig.GetIFF()) || !IsPointFullCaptured()))
            {
                ((ConquestSosig)sosig).OrderToAssault(this);
            }
        }

        protected override void GivePointToTeam(int team)
        {
            base.GivePointToTeam(team);

            foreach(SpawnArea area in SpawnAreas)
            {
                area.SetTeam(teams[team], team);
            }
        }

        public ConquestPoint GetNeighboringPointToAttack()
        {
            List<ConquestPoint> possiblePoints = new List<ConquestPoint>();

            //Prioritize points that aren't captured and marked for attack
            possiblePoints.AddRange(Neighbors.Where(o => !o.DoesTeamControlPoint(currentTeam) && AttackNeighborPoints[o]));
            if (possiblePoints.Count > 0) return possiblePoints.GetRandom();

            possiblePoints.AddRange(Neighbors.Where(o => AttackNeighborPoints[o]));
            if (possiblePoints.Count > 0) return possiblePoints.GetRandom();

            return Neighbors.GetRandom();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.5f);

            foreach(ConquestPoint neighbor in Neighbors)
            {
                DrawArrow(transform.position, neighbor.transform.position, Color.red, 5);
            }
            
            Gizmos.color = Color.magenta;
            foreach (Transform point in DefendPointContainer)
            {
                Gizmos.DrawSphere(point.position, 0.25f);
            }

            Gizmos.color = Color.cyan;
            foreach (Transform point in AttackPointContainer)
            {
                Gizmos.DrawSphere(point.position, 0.25f);
            }
        }

        public static void DrawArrow(Vector3 start, Vector3 end, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Vector3 direction = end - start;

            Gizmos.color = color;
            Gizmos.DrawRay(start, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(start + (direction / 1.2f), right * arrowHeadLength);
            Gizmos.DrawRay(start + (direction / 1.2f), left * arrowHeadLength);
        }
#endif

    }
}

