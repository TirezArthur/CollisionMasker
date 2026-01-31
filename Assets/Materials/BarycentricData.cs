using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BarycentricData : MonoBehaviour
{
	void Start()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		Color[] colors = new Color[vertices.Length];

		// Assign (1,0,0), (0,1,0), (0,0,1) to the corners of every triangle
		for (int i = 0; i < triangles.Length; i += 3)
		{
			colors[triangles[i]] = new Color(1, 0, 0);
			colors[triangles[i + 1]] = new Color(0, 1, 0);
			colors[triangles[i + 2]] = new Color(0, 0, 1);
		}

		mesh.colors = colors;
	}
}