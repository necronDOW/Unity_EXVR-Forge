using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MeshCutter
{
	public class MeshCut
    {
		public class MeshCutSide
        {
			public List<Vector3> vertices = new List<Vector3>();
			public List<Vector3> normals = new List<Vector3>();
			public List<int> triangles = new List<int>();
			public List<int> indices = new List<int>();
            
			public void ClearAll()
            {
				vertices.Clear();
				normals.Clear();
				triangles.Clear();
				indices.Clear();
			}

            public void AddBlanks(int count)
            {
                vertices.AddRange(new Vector3[count]);
                normals.AddRange(new Vector3[count]);
                triangles.AddRange(new int[count]);
                indices.AddRange(new int[count]);
            }

            public void AddTriangle(int p1, int p2, int p3)
            {
                AddTriangle(p1, p2, p3, vertices.Count);
            }

			public void AddTriangle(int p1, int p2, int p3, int base_index)
            {
				// triangle index order goes 1,2,3,4....
                
                indices.Add(base_index);
                indices.Add(base_index+1);
                indices.Add(base_index+2);

				triangles.Add(base_index);
				triangles.Add(base_index+1);
				triangles.Add(base_index+2);

				vertices.Add(victim_verts[p1]);
				vertices.Add(victim_verts[p2]);
				vertices.Add(victim_verts[p3]);

				normals.Add(victim_norms[p1]);
				normals.Add(victim_norms[p2]);
				normals.Add(victim_norms[p3]);
			}

            public void AddTriangle(Vector3[] points3, Vector3[] normals3, Vector3 faceNormal)
            {
                AddTriangle(points3, normals3, faceNormal, vertices.Count);
            }

			public void AddTriangle(Vector3[] points3, Vector3[] normals3, Vector3 faceNormal, int base_index)
            {
				Vector3 calculated_normal = Vector3.Cross((points3[1] - points3[0]).normalized, (points3[2] - points3[0]).normalized);

				int p1 = 0;
				int p2 = 1;
				int p3 = 2;

				if (Vector3.Dot(calculated_normal, faceNormal) < 0) {
					p1 = 2;
					p2 = 1;
					p3 = 0;
				}

                indices.Add(base_index);
                indices.Add(base_index+1);
                indices.Add(base_index+2);

				triangles.Add(base_index);
				triangles.Add(base_index+1);
				triangles.Add(base_index+2);

				vertices.Add(points3[p1]);
				vertices.Add(points3[p2]);
				vertices.Add(points3[p3]);

				normals.Add(normals3[p1]);
				normals.Add(normals3[p2]);
				normals.Add(normals3[p3]);
			}

            public void AddTriangleAtIndex(Vector3[] points3, Vector3[] normals3, Vector3 faceNormal, int base_index)
            {
                Vector3 calculated_normal = Vector3.Cross((points3[1] - points3[0]).normalized, (points3[2] - points3[0]).normalized);

                int p1 = 0;
                int p2 = 1;
                int p3 = 2;

                if (Vector3.Dot(calculated_normal, faceNormal) < 0)
                {
                    p1 = 2;
                    p2 = 1;
                    p3 = 0;
                }

                indices[base_index] = base_index;
                indices[base_index+1] = base_index+1;
                indices[base_index+2] = base_index+2;

                triangles[base_index] = base_index;
                triangles[base_index+1] = base_index+1;
                triangles[base_index+2] = base_index+2;

                vertices[base_index] = points3[p1];
                vertices[base_index+1] = points3[p2];
                vertices[base_index+2] = points3[p3];

                normals[base_index] = normals3[p1];
                normals[base_index+1] = normals3[p2];
                normals[base_index+2] = normals3[p3];
            }
		}

		private static MeshCutSide left_side = new MeshCutSide();
		private static MeshCutSide right_side = new MeshCutSide();

		private static Plane blade;
		private static Mesh victim_mesh;
        
		private static List<Vector3> new_vertices = new List<Vector3>();
        private static Vector3[] victim_verts;
        private static Vector3[] victim_norms;

        static float timer = 0.0f;
		/// <summary>
		/// Cut the specified victim, blade_plane and capMaterial.
		/// </summary>
		/// <param name="victim">Victim.</param>
		/// <param name="blade_plane">Blade plane.</param>
		/// <param name="capMaterial">Cap material.</param>
		public static GameObject[] Cut(GameObject victim, Vector3 anchorPoint, Vector3 normalDirection, MeshInfo mInfo = null)
        {
            Vector3 invAnchorPoint = victim.transform.InverseTransformPoint(anchorPoint);

            // set the blade relative to victim
            blade = new Plane(victim.transform.InverseTransformDirection(-normalDirection), invAnchorPoint);

            // get the victims mesh
            victim_mesh = victim.GetComponent<MeshFilter>().sharedMesh;
            victim_verts = victim_mesh.vertices;
            victim_norms = victim_mesh.normals;

            // reset values
            new_vertices.Clear();
			left_side.ClearAll();
			right_side.ClearAll();
            
			bool[] sides = new bool[3];
			int[] indices;
			int p1,p2,p3;

            int minCut = int.MaxValue, maxCut = int.MinValue;

            // go throught the submeshes
            indices = victim_mesh.GetIndices(0);

            left_side.indices = new List<int>();
            right_side.indices = new List<int>();

            int cutVerts = 0;
            for (int i = 0; i < indices.Length; i += 3)
            {
                if (RequiresCut(ref sides, indices[i], indices[i + 1], indices[i + 2]))
                {
                    cutVerts += 3;

                    if (Vector3.Distance(victim_verts[indices[i]], invAnchorPoint) < 0.1f)
                    {
                        minCut = MinInt(MinInt(MinInt(minCut, indices[i]), indices[i + 1]), indices[i + 2]);
                        maxCut = MaxInt(MaxInt(MaxInt(minCut, indices[i]), indices[i + 1]), indices[i + 2]);
                    }
                }
            }
            
            left_side.AddBlanks(cutVerts);

            for (int i = 0; i < indices.Length; i += 3)
            {
                p1 = indices[i];
                p2 = indices[i + 1];
                p3 = indices[i + 2];

                if (RequiresCut(ref sides, p1, p2, p3))
                {
                    if (p1 > maxCut)
                        left_side.AddTriangle(p1, p2, p3);
                    else if (p1 < minCut)
                        right_side.AddTriangle(p1, p2, p3);
                    else Cut_this_Face(sides, p1, p2, p3);
                }
                else
                {
                    if (sides[0]) // left side
                    {
                        if (p1 <= minCut)
                            right_side.AddTriangle(p1, p2, p3);
                        else left_side.AddTriangle(p1, p2, p3);
                    }
                    else
                    {
                        if (p1 >= maxCut)
                            left_side.AddTriangle(p1, p2, p3);
                        else right_side.AddTriangle(p1, p2, p3);
                    }
                }
            }

            Material mat = victim.GetComponent<MeshRenderer>().sharedMaterial;
            
			// cap the opennings
			Capping();
            
			// Left Mesh
			Mesh left_HalfMesh = new Mesh();
			left_HalfMesh.name =  "Split Mesh Left";
			left_HalfMesh.vertices  = left_side.vertices.ToArray();
			left_HalfMesh.triangles = left_side.triangles.ToArray();
			left_HalfMesh.normals   = left_side.normals.ToArray();

            left_HalfMesh.SetIndices(left_side.indices.ToArray(), MeshTopology.Triangles, 0);

            // Right Mesh
            Mesh right_HalfMesh = new Mesh();
			right_HalfMesh.name = "Split Mesh Right";
			right_HalfMesh.vertices  = right_side.vertices.ToArray();
			right_HalfMesh.triangles = right_side.triangles.ToArray();
			right_HalfMesh.normals   = right_side.normals.ToArray();

            right_HalfMesh.SetIndices(right_side.indices.ToArray(), MeshTopology.Triangles, 0);

            // assign the game objects
            victim.GetComponent<MeshFilter>().sharedMesh = left_HalfMesh;

			GameObject leftSideObj = victim;
            leftSideObj.GetComponent<MeshCollider>().sharedMesh = victim.GetComponent<MeshFilter>().sharedMesh;

			GameObject rightSideObj = new GameObject(victim.name, typeof(MeshFilter), typeof(MeshRenderer));
			rightSideObj.transform.position = victim.transform.position;
			rightSideObj.transform.rotation = victim.transform.rotation;
            rightSideObj.transform.localScale = victim.transform.localScale;
            rightSideObj.GetComponent<MeshFilter>().mesh = right_HalfMesh;

            CopyDeformComponents(leftSideObj, left_HalfMesh, rightSideObj, right_HalfMesh);

            // assign mats
            leftSideObj.GetComponent<MeshRenderer>().material = mat;
			rightSideObj.GetComponent<MeshRenderer>().material = mat;

            return new GameObject[]{ leftSideObj, rightSideObj };
		}

        private static void CopyDeformComponents(GameObject left, Mesh leftMesh, GameObject right, Mesh rightMesh)
        {
            Rigidbody r_r = right.AddComponent<Rigidbody>();
            r_r.isKinematic = left.GetComponent<Rigidbody>().isKinematic;

            DeformableMesh l_dm = left.GetComponent<DeformableMesh>();
            DeformableMesh r_dm = right.AddComponent<DeformableMesh>();
            r_dm.maxInfluence = l_dm.maxInfluence;
            r_dm.forceFactor = l_dm.forceFactor;
            r_dm.distanceLimiter = l_dm.distanceLimiter;

            CuttableMesh l_cm = left.GetComponent<CuttableMesh>();
            CuttableMesh r_cm = right.AddComponent<CuttableMesh>();
            r_cm.hitsToCut = l_cm.hitsToCut;

            MeshStateHandler r_msh = right.AddComponent<MeshStateHandler>();
        }

        private static int MinInt(int a, int b)
        {
            return (a < b) ? a : b;
        }

        private static int MaxInt(int a, int b)
        {
            return (a > b) ? a : b;
        }

        private static bool RequiresCut(ref bool[] sides, int p1, int p2, int p3)
        {
            sides[0] = blade.GetSide(victim_verts[p1]);
            sides[1] = blade.GetSide(victim_verts[p2]);
            sides[2] = blade.GetSide(victim_verts[p3]);

            if (sides[0] == sides[1] && sides[0] == sides[2])
                return false;
            else return true;
        }

        private static void Cut_this_Face (bool[] sides, int index1, int index2, int index3)
        {
			Vector3[] leftPoints = new Vector3[2];
			Vector3[] leftNormals = new Vector3[2];
			Vector3[] rightPoints = new Vector3[2];
			Vector3[] rightNormals = new Vector3[2];

			bool didset_left = false;
			bool didset_right = false;

			int p = index1;
			for (int side=0; side<3; side++) {
				switch (side) {
				    case 0: p = index1;
					    break;
				    case 1: p = index2;
					    break;
				    case 2: p = index3;
					    break;
				}

				if (sides[side]) {
					if (!didset_left) {
						didset_left = true;

						leftPoints[0] = victim_verts[p];
						leftPoints[1] = leftPoints[0];
						leftNormals[0] = victim_norms[p];
						leftNormals[1] = leftNormals[0];
					}
                    else {
						leftPoints[1] = victim_verts[p];
						leftNormals[1] = victim_norms[p];
					}
				}
                else {
					if (!didset_right) {
						didset_right = true;

						rightPoints[0] = victim_verts[p];
						rightPoints[1] = rightPoints[0];
						rightNormals[0] = victim_norms[p];
						rightNormals[1] = rightNormals[0];

					}
                    else {

						rightPoints[1] = victim_verts[p];
						rightNormals[1] = victim_norms[p];

					}
				}
			}


			float normalizedDistance = 0.0f;
			float distance = 0;
			blade.Raycast(new Ray(leftPoints[0], (rightPoints[0] - leftPoints[0]).normalized), out distance);

			normalizedDistance =  distance/(rightPoints[0] - leftPoints[0]).magnitude;
			Vector3 newVertex1 = Vector3.Lerp(leftPoints[0], rightPoints[0], normalizedDistance);
			Vector3 newNormal1 = Vector3.Lerp(leftNormals[0] , rightNormals[0], normalizedDistance);

			new_vertices.Add(newVertex1);

			blade.Raycast(new Ray(leftPoints[1], (rightPoints[1] - leftPoints[1]).normalized), out distance);

			normalizedDistance =  distance/(rightPoints[1] - leftPoints[1]).magnitude;
			Vector3 newVertex2 = Vector3.Lerp(leftPoints[1], rightPoints[1], normalizedDistance);
			Vector3 newNormal2 = Vector3.Lerp(leftNormals[1] , rightNormals[1], normalizedDistance);

			new_vertices.Add(newVertex2);
            
			left_side.AddTriangle(new Vector3[]{leftPoints[0], newVertex1, newVertex2},
				new Vector3[]{leftNormals[0], newNormal1, newNormal2 }, newNormal1);

			left_side.AddTriangle(new Vector3[]{leftPoints[0], leftPoints[1], newVertex2},
				new Vector3[]{leftNormals[0], leftNormals[1], newNormal2}, newNormal2);

			right_side.AddTriangle(new Vector3[]{rightPoints[0], newVertex1, newVertex2},
				new Vector3[]{rightNormals[0], newNormal1, newNormal2}, newNormal1);

			right_side.AddTriangle(new Vector3[]{rightPoints[0], rightPoints[1], newVertex2},
				new Vector3[]{rightNormals[0], rightNormals[1], newNormal2}, newNormal2);

		}

		private static List<Vector3> capVertTracker = new List<Vector3>();
		private static List<Vector3> capVertpolygon = new List<Vector3>();

		private static int Capping()
        {
			capVertTracker.Clear();

            for (int i = 0; i < new_vertices.Count; i++) {
                if (!capVertTracker.Contains(new_vertices[i])) {
                    capVertpolygon.Clear();
                    capVertpolygon.Add(new_vertices[i]);
                    capVertpolygon.Add(new_vertices[i + 1]);

                    capVertTracker.Add(new_vertices[i]);
                    capVertTracker.Add(new_vertices[i + 1]);

                    bool isDone = false;
                    while (!isDone) {
                        isDone = true;

                        for (int k = 0; k < new_vertices.Count; k += 2) { // go through the pairs
                            if (new_vertices[k] == capVertpolygon[capVertpolygon.Count - 1] && !capVertTracker.Contains(new_vertices[k + 1])) { // if so add the other
                                isDone = false;
                                capVertpolygon.Add(new_vertices[k + 1]);
                                capVertTracker.Add(new_vertices[k + 1]);
                            }
                            else if (new_vertices[k + 1] == capVertpolygon[capVertpolygon.Count - 1] && !capVertTracker.Contains(new_vertices[k])) {// if so add the other
                                isDone = false;
                                capVertpolygon.Add(new_vertices[k]);
                                capVertTracker.Add(new_vertices[k]);
                            }
                        }
                    }
                    
                    FillCap(capVertpolygon);
                }
            }

            return capVertpolygon.Count;
        }

		private static void FillCap(List<Vector3> vertices)
        {
            // center of the cap
            Vector3 center = Vector3.zero;
			foreach(Vector3 point in vertices)
				center += point;

			center = center/vertices.Count;

            for (int i=0; i<vertices.Count; i++) {
                left_side.AddTriangleAtIndex(new Vector3[] { vertices[i], vertices[(i+1) % vertices.Count], center },
                    new Vector3[] { -blade.normal, -blade.normal, -blade.normal }, -blade.normal, i*3);

                right_side.AddTriangle( new Vector3[] { vertices[i], vertices[(i+1) % vertices.Count], center },
                    new Vector3[] { blade.normal, blade.normal, blade.normal }, blade.normal);
			}
        }
	}
}