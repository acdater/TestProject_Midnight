using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Assets.MainAssets.Scripts.Jobs
{
    [BurstCompile]
    public struct BoundsJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> Positions;

        public NativeArray<Vector3> Accelerations;
        public Vector3 AreaSize;
        public float Threshold;
        public float Multiplier;


        public void Execute(int index)
        {
            var currentPos = Positions[index];
            var size = AreaSize * 0.5f;

            var rightDelta = -size.x - currentPos.x;
            var leftDelta = size.x - currentPos.x;
            var downDelta = size.y - currentPos.y; 
            var upDelta = -size.y - currentPos.y; 
            var backDelta = size.z - currentPos.z; 
            var forwardDelta = -size.z - currentPos.z; 

            Accelerations[index] += Compensate(leftDelta, Vector3.left)
                + Compensate(rightDelta, Vector3.right)
                + Compensate(upDelta, Vector3.up)
                + Compensate(downDelta, Vector3.down)
                + Compensate(forwardDelta, Vector3.forward)
                + Compensate(backDelta, Vector3.back);
        }

        private Vector3 Compensate(float delta, Vector3 direction)
        {
            delta = Mathf.Abs(delta);

            if (delta > Threshold)
            {
                return Vector3.zero;
            }

            return direction * (1 - delta / Threshold) * Multiplier;
        }
    }
}
