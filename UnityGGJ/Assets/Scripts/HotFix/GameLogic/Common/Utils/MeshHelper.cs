using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class MeshHelper
    {
        public static Mesh CreatMesh(Vector3[] verticeList)
        {
            Mesh mesh = new Mesh();
            int length = verticeList.Length;
            Vector3[] vertices = new Vector3[length];
            Vector2[] uvs = new Vector2[length];
            int triangleIndex = 0;
            int[] triangles = new int[length * 2 * 3];

            for (int i = 0; i < length; i++)
            {
                vertices[i] = verticeList[i];
                uvs[i] = Vector2.zero;
            }

            for (int i = 0; i < length; i++)
            {
                if (i + 2 >= length)
                    break;
                triangles[triangleIndex] = i + 0;
                triangles[triangleIndex + 1] = i + 2;
                triangles[triangleIndex + 2] = i + 1;
                triangleIndex += 3;

                if (i + 3 >= length)
                    break;
                triangles[triangleIndex] = i + 1;
                triangles[triangleIndex + 1] = i + 2;
                triangles[triangleIndex + 2] = i + 3;
                triangleIndex += 3;
            }

            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            mesh.UploadMeshData(false);
            return mesh;
        }

        public static Mesh CreateCylinder(float radius = 0.5f, float height = 1f, int radialSegments = 24)
        {
            Mesh mesh = new Mesh();
            mesh.name = "ProceduralCylinder";

            int vertexCount = (radialSegments + 1) * 4 + 2;
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];

            int vertIndex = 0;
            float halfHeight = height * 0.5f;

            for (int y = 0; y <= 1; y++)
            {
                float yPos = y == 0 ? -halfHeight : halfHeight;
                float v = y;

                for (int x = 0; x <= radialSegments; x++)
                {
                    float angle = (x / (float)radialSegments) * Mathf.PI * 2f;
                    float xPos = Mathf.Cos(angle) * radius;
                    float zPos = Mathf.Sin(angle) * radius;
                    float u = x / (float)radialSegments;

                    vertices[vertIndex] = new Vector3(xPos, yPos, zPos);
                    normals[vertIndex] = new Vector3(xPos, 0, zPos).normalized;
                    uvs[vertIndex] = new Vector2(u, v);
                    vertIndex++;
                }
            }

            List<int> triangleList = new List<int>();

            for (int x = 0; x < radialSegments; x++)
            {
                int current = x;
                int next = current + radialSegments + 1;

                triangleList.Add(current);
                triangleList.Add(next);
                triangleList.Add(current + 1);

                triangleList.Add(current + 1);
                triangleList.Add(next);
                triangleList.Add(next + 1);
            }

            int bottomCenterIndex = vertIndex;
            vertices[vertIndex] = new Vector3(0, -halfHeight, 0);
            normals[vertIndex] = Vector3.down;
            uvs[vertIndex] = new Vector2(0.5f, 0.5f);
            vertIndex++;

            for (int x = 0; x <= radialSegments; x++)
            {
                float angle = (x / (float)radialSegments) * Mathf.PI * 2f;
                float xPos = Mathf.Cos(angle) * radius;
                float zPos = Mathf.Sin(angle) * radius;

                vertices[vertIndex] = new Vector3(xPos, -halfHeight, zPos);
                normals[vertIndex] = Vector3.down;
                uvs[vertIndex] = new Vector2(xPos / radius * 0.5f + 0.5f, zPos / radius * 0.5f + 0.5f);
                vertIndex++;
            }

            for (int x = 0; x < radialSegments; x++)
            {
                triangleList.Add(bottomCenterIndex);
                triangleList.Add(bottomCenterIndex + x + 1);
                triangleList.Add(bottomCenterIndex + x + 2);
            }

            int topCenterIndex = vertIndex;
            vertices[vertIndex] = new Vector3(0, halfHeight, 0);
            normals[vertIndex] = Vector3.up;
            uvs[vertIndex] = new Vector2(0.5f, 0.5f);
            vertIndex++;

            for (int x = 0; x <= radialSegments; x++)
            {
                float angle = (x / (float)radialSegments) * Mathf.PI * 2f;
                float xPos = Mathf.Cos(angle) * radius;
                float zPos = Mathf.Sin(angle) * radius;

                vertices[vertIndex] = new Vector3(xPos, halfHeight, zPos);
                normals[vertIndex] = Vector3.up;
                uvs[vertIndex] = new Vector2(xPos / radius * 0.5f + 0.5f, zPos / radius * 0.5f + 0.5f);
                vertIndex++;
            }

            for (int x = 0; x < radialSegments; x++)
            {
                triangleList.Add(topCenterIndex);
                triangleList.Add(topCenterIndex + x + 2);
                triangleList.Add(topCenterIndex + x + 1);
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangleList.ToArray();

            mesh.RecalculateBounds();

            return mesh;
        }

        public static void BuildRingMesh(Mesh mesh, float outerRadius, float innerRadius, int segments)
        {
            if (mesh == null)
                return;

            int clampedSegments = Mathf.Clamp(segments, 3, 256);
            float clampedOuter = Mathf.Max(0.01f, outerRadius);
            float clampedInner = Mathf.Clamp(innerRadius, 0.01f, clampedOuter - 0.01f);

            Vector3[] vertices = new Vector3[clampedSegments * 2];
            Vector3[] normals = new Vector3[vertices.Length];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[clampedSegments * 6];

            for (int i = 0; i < clampedSegments; i++)
            {
                float t = i / (float)clampedSegments;
                float angle = t * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Vector3 outer = new Vector3(cos * clampedOuter, 0f, sin * clampedOuter);
                Vector3 inner = new Vector3(cos * clampedInner, 0f, sin * clampedInner);
                int vertIndex = i * 2;

                vertices[vertIndex] = outer;
                vertices[vertIndex + 1] = inner;
                normals[vertIndex] = Vector3.up;
                normals[vertIndex + 1] = Vector3.up;
                uvs[vertIndex] = new Vector2(t, 1f);
                uvs[vertIndex + 1] = new Vector2(t, 0f);

                int next = ((i + 1) % clampedSegments) * 2;
                int triIndex = i * 6;

                triangles[triIndex] = vertIndex;
                triangles[triIndex + 1] = next;
                triangles[triIndex + 2] = vertIndex + 1;

                triangles[triIndex + 3] = vertIndex + 1;
                triangles[triIndex + 4] = next;
                triangles[triIndex + 5] = next + 1;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
        }

        public static void BuildArrowMesh(Mesh mesh, float length, float tailWidth, float headWidth, float headLengthRatio)
        {
            if (mesh == null)
                return;

            float clampedLength = Mathf.Max(0.1f, length);
            float ratio = Mathf.Clamp(headLengthRatio, 0.1f, 0.9f);
            float shaftLength = Mathf.Max(0.01f, clampedLength * (1f - ratio));
            float headBaseZ = shaftLength;

            float halfTail = Mathf.Max(0.01f, tailWidth * 0.5f);
            float halfHead = Mathf.Max(halfTail, headWidth * 0.5f);

            Vector3[] vertices =
            {
                new Vector3(-halfTail, 0f, 0f),
                new Vector3(-halfTail, 0f, headBaseZ),
                new Vector3(-halfHead, 0f, headBaseZ),
                new Vector3(0f, 0f, clampedLength),
                new Vector3(halfHead, 0f, headBaseZ),
                new Vector3(halfTail, 0f, headBaseZ),
                new Vector3(halfTail, 0f, 0f)
            };

            Vector3[] normals = new Vector3[vertices.Length];
            Vector2[] uvs = new Vector2[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                normals[i] = Vector3.up;
                uvs[i] = new Vector2((vertices[i].x / (halfHead * 2f)) + 0.5f, vertices[i].z / clampedLength);
            }

            int triangleCount = vertices.Length - 2;
            int[] triangles = new int[triangleCount * 3];
            for (int i = 0; i < triangleCount; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
        }
    }
}


