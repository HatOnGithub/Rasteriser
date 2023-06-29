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
        public Texture bg = new Texture("../../../assets/hiigara.jpg");
        public static int nrOfObjectsRendered = 0;

        public SceneGraph() : 
            base( null, "")
        {

        }

        public void AddLight(Light light)
        {
            Children.Add(light);
        }

        public void Render(Shader shader, Matrix4 WorldToCamera, Matrix4 CameraToScreen, Vector3 camPos)
        {
            Light[] lights = GetLights(this).ToArray();

            nrOfObjectsRendered = 0;

            foreach (var child in Children) child.Render(shader, Matrix4.Identity, WorldToCamera, CameraToScreen, AmbientLight , lights, camPos, bg);

            Console.WriteLine("Objects Rendered = " + nrOfObjectsRendered);
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
