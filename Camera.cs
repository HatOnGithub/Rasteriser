using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Template
{
    public class Camera
    {
        public float movementSpeed = 5;

        public Vector3 e = new Vector3(0, 0, 10);
        public Vector3 t = new Vector3(0,1,0);
        public Vector3 g = new Vector3(0,0,-1);

        public Vector3 u { get { return Vector3.Normalize(Vector3.Cross(t, w)); } } // x
        public Vector3 v { get { return Vector3.Normalize(Vector3.Cross(w, u)); } } // y
        public Vector3 w { get { return -Vector3.Normalize(g); } } // z

        public Matrix4 WorldToCamera => Matrix4.LookAt(e, e + g * 1, t) ;


        public void MoveCamera(Vector3 positionChange, Vector3 gazeChange)
        {
            g = g *
                Matrix3.CreateFromAxisAngle(u, MathHelper.DegreesToRadians(-gazeChange.X)) *
                Matrix3.CreateFromAxisAngle(v, MathHelper.DegreesToRadians(-gazeChange.Y));
            
            t *= Matrix3.CreateFromAxisAngle(w, MathHelper.DegreesToRadians(gazeChange.Z));

            e += new Vector3(Vector3.Dot(positionChange, u), Vector3.Dot(positionChange, v), Vector3.Dot(positionChange, g));
        }
    }
}
