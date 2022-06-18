using FistVR;
using Sodalite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes
{

	public class WristMapController : MonoBehaviour
	{
		public float mapRadius = 0.05f;
		public Color mapColor = Color.white;
		public float mapScale = .001f;
		public WristMapDisplayMode displayMode = WristMapDisplayMode.RightHand;

		[HideInInspector]
		public List<WristMapTarget> targets = new List<WristMapTarget>();

		private Quaternion mapRotation;

		private Vector3 drawPosition;

		void Awake()
		{
			mapRotation = Quaternion.LookRotation(Vector3.up);
		}

		void Update()
		{
			if (GM.CurrentPlayerBody == null || displayMode == WristMapDisplayMode.Disabled) return;

			UpdateDrawPosition();
			DrawWristMap();
		}

		public void IncrementMapState(bool reverse)
        {
            if (reverse)
            {
				displayMode = displayMode.Previous();
            }

            else
            {
				displayMode = displayMode.Next();
            }
        }


		private void UpdateDrawPosition()
		{
			if(displayMode == WristMapDisplayMode.LeftHand)
            {
				drawPosition = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
			}
            else
            {
				drawPosition = GM.CurrentPlayerBody.RightHand.position + GM.CurrentPlayerBody.RightHand.forward * -0.2f;
			}
		}


		private void DrawWristMap()
		{
			Popcron.Gizmos.Circle(drawPosition, mapRadius, mapRotation, mapColor);

			foreach (WristMapTarget target in targets)
			{
				DrawWristTarget(target);
			}
		}


		private void DrawWristTarget(WristMapTarget target)
		{
			Vector3 offset = (target.transform.position - drawPosition) * mapScale;

			//If the flattened offset is larger than our map, set it to land on the edge
			offset.y = 0;
			if (offset.magnitude > mapRadius - (mapRadius * target.scale))
			{
				offset = offset.normalized * mapRadius - offset.normalized * (mapRadius * target.scale);
			}

			Popcron.Gizmos.Circle(drawPosition + offset, (mapRadius * target.scale), mapRotation, target.color);
		}

		


	}


	//This neat little extension brought to you by this stack overflow thread: https://stackoverflow.com/questions/642542/how-to-get-next-or-previous-enum-value-in-c-sharp
	public static class Enums
	{
		public static T Next<T>(this T v) where T : struct
		{
			return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).SkipWhile(e => !v.Equals(e)).Skip(1).First();
		}

		public static T Previous<T>(this T v) where T : struct
		{
			return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).Reverse().SkipWhile(e => !v.Equals(e)).Skip(1).First();
		}
	}


	public class WristMapTarget
	{

		public Color color;
		public float scale;
		public Transform transform;

		public WristMapTarget(Transform target, Color color, float radius)
		{
			this.transform = target;
			this.color = color;
			this.scale = radius;
		}

	}

	public enum WristMapDisplayMode
	{
		RightHand,
		LeftHand,
		Disabled
	}
}


