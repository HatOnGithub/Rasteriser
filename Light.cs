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
        public Vector3 direction = Vector3.Zero;
        public float coneAngle = 360;

        public Light(Vector3 pos, Vector3 color, float intensity, Vector3 direction, float coneAngle) : base(new Mesh("", (Vector3?)pos, null, null, 0, 0, 0), "")
        {
            this.color = color;
            this.intensity = intensity;

            if (direction != Vector3.Zero)
            {
                direction = Vector3.Normalize(direction);
                this.coneAngle = MathHelper.DegreesToRadians( coneAngle);
            }
        }

        public override void Render(Shader shader, Matrix4 ConcatParentMatrices, Matrix4 WorldToCamera, Matrix4 CameraToScreen, Vector3 AmbientLight, Light[] lights, Vector3 camPos, Texture environmentMap)
        {
            return;
        }
    }
}
