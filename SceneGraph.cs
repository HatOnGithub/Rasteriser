using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    public class SceneGraph : Node
    {
        public Vector3 AmbientLight = new Vector3(0.1f, 0.1f, 0.1f);

        public SceneGraph() : 
            base( null, "")
        {

        }

        public void AddLight(Vector3 position, Vector3 color, float intensity)
        {
            Light light = new Light(position, color, intensity);
            Children.Add(light);
        }

        public void Render(Shader shader, Matrix4 WorldToCamera, Matrix4 CameraToScreen, Vector3 camPos)
        {
            Light[] lights = GetLights(this).ToArray();
            Vector3[] lightpos = new Vector3[lights.Length];
            Vector3[] colors = new Vector3[lights.Length];

            for (int i = 0; i < lights.Length; i++)
            {
                lightpos[i] = lights[i].Mesh.Pos;
                colors[i] = lights[i].color * lights[i].intensity;
            }

            foreach (var child in Children) child.Render(shader, Matrix4.Identity, WorldToCamera, CameraToScreen, AmbientLight ,lightpos, colors, camPos);
        }

        public List<Light> GetLights(Node node)
        {
            List<Light> result = new();

            foreach (Node child in node.Children) if (child is Light light)
            {
                result.Add(light);
                result.AddRange(GetLights(child));
            }

            return result;
        }
    }
}
