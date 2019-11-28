using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using UnityEngine.Assertions;

namespace FootStone.Kitchen
{
	public static class CharacterControllerUtilities
	{
		// A collector which stores every hit up to the length of the provided native array.
		// To filter out self hits, it stores the rigid body index of the body representing
		// the character controller. Unfortunately, it needs to do this in TransformNewHits
		// since during AddHit rigid body index is not exposed.
		// https://github.com/Unity-Technologies/Unity.Physics/issues/256
		public struct MaxHitsCollector<T> : ICollector<T> where T : struct, IQueryResult
		{
			private int m_NumHits;
			private int m_selfRBIndex;
			public bool EarlyOutOnFirstHit => false;
			public float MaxFraction { get; }
			public int NumHits => m_NumHits;

			public NativeArray<T> AllHits;

			public MaxHitsCollector(int rbIndex, float maxFraction, ref NativeArray<T> allHits)
			{
				MaxFraction = maxFraction;
				AllHits = allHits;
				m_NumHits = 0;
				m_selfRBIndex = rbIndex;
			}

			#region IQueryResult implementation

			public bool AddHit(T hit)
			{
				Assert.IsTrue(hit.Fraction < MaxFraction);
				Assert.IsTrue(m_NumHits < AllHits.Length);
				AllHits[m_NumHits] = hit;
				m_NumHits++;
				return true;
			}

			public void TransformNewHits(int oldNumHits, float oldFraction, Unity.Physics.Math.MTransform transform, uint numSubKeyBits, uint subKey)
			{
				for (int i = oldNumHits; i < m_NumHits; i++)
				{
					T hit = AllHits[i];
					hit.Transform(transform, numSubKeyBits, subKey);
					AllHits[i] = hit;
				}
			}

			public void TransformNewHits(int oldNumHits, float oldFraction, Unity.Physics.Math.MTransform transform, int rigidBodyIndex)
			{
				if (rigidBodyIndex == m_selfRBIndex)
				{
					m_NumHits = oldNumHits;
					return;
				}

				for (int i = oldNumHits; i < m_NumHits; i++)
				{
					T hit = AllHits[i];
					hit.Transform(transform, rigidBodyIndex);
					AllHits[i] = hit;
				}
			}

			#endregion
		}

		public static unsafe void CheckSupport(PhysicsWorld world, int selfRigidBodyIndex, float skinWidth, NativeList<DistanceHit> distanceHits, ref NativeArray<SurfaceConstraintInfo> constraints, out int numConstraints)
		{
			// Iterate over distance hits and create constraints from them
			numConstraints = 0;
			for (int i = 0; i < distanceHits.Length; i++)
			{
				DistanceHit hit = distanceHits[i];
				if (hit.RigidBodyIndex != selfRigidBodyIndex)
				{
					CreateConstraint(world, hit.RigidBodyIndex, hit.ColliderKey, hit.Position, hit.SurfaceNormal, hit.Distance,
						skinWidth, ref constraints, ref numConstraints);
				}
			}
		}

		private static void CreateConstraint(PhysicsWorld world, int hitRigidBodyIndex,
			ColliderKey hitColliderKey, float3 hitPosition, float3 hitSurfaceNormal, float hitDistance,
			float skinWidth, ref NativeArray<SurfaceConstraintInfo> constraints, ref int numConstraints)
		{
			CreateConstraintFromHit(world, hitRigidBodyIndex, hitColliderKey, hitPosition,
				hitSurfaceNormal, hitDistance, skinWidth, out SurfaceConstraintInfo constraint);

			// Add original constraint to the list
			constraints[numConstraints++] = constraint;
		}

		private static void CreateConstraintFromHit(PhysicsWorld world, int rigidBodyIndex, ColliderKey colliderKey,
		float3 hitPosition, float3 normal, float distance, float skinWidth, out SurfaceConstraintInfo constraint)
		{
			bool bodyIsDynamic = 0 <= rigidBodyIndex && rigidBodyIndex < world.NumDynamicBodies;
			constraint = new SurfaceConstraintInfo()
			{
				Plane = new Unity.Physics.Plane
				{
					Normal = normal,
					Distance = distance - skinWidth,
				},
				RigidBodyIndex = rigidBodyIndex,
				ColliderKey = colliderKey,
				HitPosition = hitPosition,
				Velocity = bodyIsDynamic ?
					world.GetLinearVelocity(rigidBodyIndex, hitPosition) : float3.zero,
				Priority = bodyIsDynamic ? 1 : 0
			};
		}
	}
}
