using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Assets.MainAssets.Scripts.Jobs
{
    [BurstCompile]
    public struct AccelerationJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> FoodPositions;

        [ReadOnly]
        public NativeArray<Vector3> Positions; // as input

        [ReadOnly]
        public NativeArray<Vector3> Velocities; // as input
    
        public NativeArray<Vector3> Accelerations; // as output

        public float DestinationThreshold;

        public Vector3 Weights;

        public Vector3 TargetPosition;

        private int Count => Positions.Length - 1;

        public void Execute(int index)
        {
            Vector3 commonSpread = Vector3.zero;
            Vector3 commonVelocity = Vector3.zero;
            Vector3 commonPosition = Vector3.zero;

            for(int i = 0; i < Count; i++)
            {
                if (i == index)
                    continue;

                var currentPos = Positions[index];
                var targetPos = Positions[i];

                var positionsDifference = currentPos - targetPos;

                if (positionsDifference.magnitude > DestinationThreshold)
                {
                    continue;
                }

                commonSpread += positionsDifference.normalized;
                commonVelocity += Velocities[i];
                commonPosition += targetPos;
            }

            var averageSpread = commonSpread / Count;
            var averageVelocity = commonVelocity/ Count;
            var averagePosition = commonPosition/ Count - Positions[index];

            if (FoodPositions.Any())
            {
                var foodPos = FindClosestTargetPosition(Positions[index]);
                var direction = foodPos - Positions[index];
                averageVelocity += direction; // Somth like that can make fishe go to their target...
            }


            Accelerations[index] += averageSpread * Weights.x + averageVelocity * Weights.y + averagePosition * Weights.z;
        }

        private Vector3 FindClosestTargetPosition(Vector3 fishPos)
        {
            float closestDistance = float.MaxValue;
            int closestFoodPosIndex = -1;

            for(int i = 0; i < FoodPositions.Length; i++)
            {
                if (Vector3.Distance(fishPos, FoodPositions[i]) < closestDistance)
                {
                    closestDistance = Vector3.Distance(fishPos, FoodPositions[i]);
                    closestFoodPosIndex = i;
                }
            }

            if (closestFoodPosIndex == -1) return Vector3.zero;

            return FoodPositions[closestFoodPosIndex];
        }
    }
}
