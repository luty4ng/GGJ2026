using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public struct Circle
    {
        public Vector3 Center;
        public float Radius;
        public Circle(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }

    public struct CircleRing
    {
        public Vector3 Center;
        public float MaxRadius;
        public float MinRadius;
        public CircleRing(Vector3 center, float minRadius, float maxRadius)
        {
            Center = center;
            MaxRadius = maxRadius;
            MinRadius = minRadius;
        }
    }
    public static class Geometry
    {
        public static void Ellipse(Vector3 focus, Vector3 center, float sampleCount, float minorRadius, ref List<Vector3> points)
        {
            float angle = 0;
            float c = Vector3.Distance(focus, center);
            float b = minorRadius;
            Vector3 relativeVec = focus - center;
            float relativeSin = relativeVec.magnitude == 0 ? 0 : relativeVec.z / relativeVec.magnitude;
            float relativeCos = relativeVec.magnitude == 0 ? 1 : relativeVec.x / relativeVec.magnitude;
            float a = Mathf.Sqrt(c * c + b * b);
            float eccentricity = c / a;
            points.Clear();
            while (angle < 360)
            {
                float r = b / Mathf.Sqrt(1 - Mathf.Pow(eccentricity, 2) * Mathf.Pow(Mathf.Cos(Mathf.Deg2Rad * angle), 2));
                float x = r * Mathf.Cos(Mathf.Deg2Rad * angle);
                float z = r * Mathf.Sin(Mathf.Deg2Rad * angle);
                float finalX = x * relativeCos - z * relativeSin;
                float finalZ = x * relativeSin + z * relativeCos;

                finalX += focus.x;
                finalZ += focus.z;

                points.Add(new Vector3(finalX, 0, finalZ));
                angle += 360f / sampleCount;
            }
        }

        public static void Circle(Vector3 center, float radius, float sampleCount, List<Vector3> points)
        {
            points.Clear();
            for (int i = 0; i < sampleCount; i++)
            {
                float angle = i * 360f / sampleCount;
                float x = center.x + radius * Mathf.Cos(Mathf.Deg2Rad * angle);
                float z = center.z + radius * Mathf.Sin(Mathf.Deg2Rad * angle);
                points.Add(new Vector3(x, 0, z));
            }
        }

        public static float DistanceToCircle(Vector3 center, float radius, Vector3 targetPos)
        {
            return Vector3.Distance(center, targetPos) - radius;
        }

        public static Vector3 IntersectToCircle(Vector3 center, float radius, Vector3 targetPos)
        {
            Vector3 dir = targetPos - center;
            dir.Normalize();
            return center + dir * radius;
        }

        public static Vector3 GetRandomPosOnCircle(Circle landOnCircle)
        {
            float angle = UnityEngine.Random.Range(0, 360);
            float x = landOnCircle.Center.x + landOnCircle.Radius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float z = landOnCircle.Center.z + landOnCircle.Radius * Mathf.Sin(Mathf.Deg2Rad * angle);
            return new Vector3(x, 0, z);
        }

        public static Vector3 GetRandomPosOnCircle(Circle landOnCircle, CircleRing includeRing)
        {
            // 先在 ring（MinRadius~MaxRadius）范围内生成点，再把该点沿方向唯一映射到 landOnCircle 圆周上。
            const float k_landOnEpsilon = 0.1f;
            float minRadius = Mathf.Max(0f, includeRing.MinRadius);
            float maxRadius = Mathf.Max(0f, includeRing.MaxRadius);
            if (minRadius > maxRadius)
            {
                (minRadius, maxRadius) = (maxRadius, minRadius);
            }

            float angle = Random.Range(0f, 360f);
            float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            float sin = Mathf.Sin(Mathf.Deg2Rad * angle);

            // 1) 在 ring 内取点（XZ 平面，y=0）。
            float radiusInRing = Random.Range(minRadius, maxRadius);
            Vector3 pointInRing = new Vector3(
                includeRing.Center.x + radiusInRing * cos,
                0f,
                includeRing.Center.z + radiusInRing * sin);

            // 如果生成点已经非常接近 land 圆周，则直接返回，避免额外计算。
            Vector3 landCenter = new Vector3(landOnCircle.Center.x, 0f, landOnCircle.Center.z);
            float distToLandCenter = Vector3.Distance(landCenter, pointInRing);
            if (Mathf.Abs(distToLandCenter - landOnCircle.Radius) < k_landOnEpsilon)
            {
                return pointInRing;
            }

            // 2) 将点沿 landOnCircle.Center -> pointInRing 的方向投影到 landOnCircle 圆周，得到唯一结果。
            Vector3 dir = pointInRing - landCenter;
            dir.y = 0f;
            dir.Normalize();

            return new Vector3(
                landOnCircle.Center.x + dir.x * landOnCircle.Radius,
                0f,
                landOnCircle.Center.z + dir.z * landOnCircle.Radius);
        }

        public static Vector3 TangentDirOnCircle(Vector3 center, Vector3 targetPos)
        {
            Vector3 dir = targetPos - center;
            dir.Normalize();
            return new Vector3(-dir.z, 0, dir.x);
        }
    }
}