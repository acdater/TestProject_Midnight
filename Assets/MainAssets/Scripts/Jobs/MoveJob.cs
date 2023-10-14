using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Assets.MainAssets.Scripts.Jobs
{
    [BurstCompile]
    public struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Positions; // as output
        public NativeArray<Vector3> Velocities; // as input
        public NativeArray<Vector3> Accelerations; // as input
        public float MaxVelocity;

        public float DeltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            var velocity = Velocities[index] + Accelerations[index] * DeltaTime;
            var direction = velocity.normalized; // not needed if there is no rotaion.
            velocity = direction * Mathf.Clamp(velocity.magnitude, 1, MaxVelocity);
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += velocity * DeltaTime;

            Positions[index] = transform.position;

            //Save velocity to have actual data for next iteration
            Velocities[index] = velocity;
            Accelerations[index] = Vector3.zero;
        }
    }
}
