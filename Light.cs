using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;


namespace Template
{
    public class Light : Node
    {
        public Vector3 color;
        public float intensity;
        public Light(Vector3 pos, Vector3 color, float intensity) : base(new Mesh("", (Vector3?)pos, null, null, 0, 0, 0), "")
        {
            this.color = color;
            this.intensity = intensity;
        }

        public override void Render(Shader shader, Matrix4 ConcatParentMatrices, Matrix4 WorldToCamera, Matrix4 CameraToScreen, Vector3 AmbientLight, Vector3[] lightPositions, Vector3[] colors, Vector3 camPos)
        {
            return;
        }
    }
}
