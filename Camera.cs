using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Template
{
    public class Camera
    {
        public float movementSpeed = 5;

        public Vector3 e = new Vector3(10, 15, 20);
        public Vector3 t = new Vector3(0,1,0);
        public Vector3 g = new Vector3(0,0,-1);

        public Vector3 u { get { return Vector3.Normalize(Vector3.Cross(t, w)); } } // x
        public Vector3 v { get { return Vector3.Normalize(Vector3.Cross(w, u)); } } // y
        public Vector3 w { get { return -Vector3.Normalize(g); } } // z

        public float FOV = 100;

        public float aspectratio;

        public Matrix4 WorldToCamera => Matrix4.LookAt(e, e + g * 1, t) ;

        Frustum frustum => new Frustum(this, aspectratio, MathHelper.DegreesToRadians(FOV), 1, 1000);


        public Camera(ref Surface screen)
        {
            aspectratio = screen.width/ screen.height;
        }
        public bool FrustumCollisionCheck(Sphere sphere, Matrix4 objectToWorld)
        {
            Vector3 scale = objectToWorld.ExtractScale();
            Vector3 center = new Vector3(new Vector4(sphere.Center, 1) * objectToWorld);

            float maxScale = MathF.Max(MathF.Max(scale.X, scale.Y), scale.Z);

            Sphere sphereInWorld = new Sphere(center, sphere.Radius * maxScale);

            return 
                sphereInWorld.OnOrInFrontOfPlane(frustum.near)   &&
                sphereInWorld.OnOrInFrontOfPlane(frustum.far)    &&
                sphereInWorld.OnOrInFrontOfPlane(frustum.top)    &&
                sphereInWorld.OnOrInFrontOfPlane(frustum.bottom) &&
                sphereInWorld.OnOrInFrontOfPlane(frustum.left)   &&
                sphereInWorld.OnOrInFrontOfPlane(frustum.right);
        }

        public void MoveCamera(Vector3 positionChange, Vector3 gazeChange)
        {
            g = g *
                Matrix3.CreateFromAxisAngle(u, MathHelper.DegreesToRadians(-gazeChange.X)) *
                Matrix3.CreateFromAxisAngle(v, MathHelper.DegreesToRadians(-gazeChange.Y));
            
            t *= Matrix3.CreateFromAxisAngle(w, MathHelper.DegreesToRadians(gazeChange.Z));

            e += new Vector3(Vector3.Dot(positionChange, u), Vector3.Dot(positionChange, v), Vector3.Dot(positionChange, g));
        }
    }

    public struct Frustum
    {
        public Plane top, bottom, left, right, far, near;

        public Frustum(Camera camera, float aspectratio, float fov, float znear, float zfar)
        {
            float halfHorizontal = zfar * MathF.Tan(fov / 2);
            float halfVertical = halfHorizontal / aspectratio;

            Vector3 toFarPlane = zfar * camera.g;

            near = new Plane( camera.e + (znear * camera.g), camera.g);
            far = new Plane(camera.e + toFarPlane, -camera.g);

            right = new Plane(camera.e, Vector3.Cross(toFarPlane - camera.u * halfHorizontal, camera.v));
            left = new Plane(camera.e, Vector3.Cross(camera.v, toFarPlane + camera.u * halfHorizontal));

            top = new Plane(camera.e, Vector3.Cross(camera.u, toFarPlane - camera.v * halfVertical));
            bottom = new Plane(camera.e, Vector3.Cross(toFarPlane + camera.v * halfVertical, camera.u));
        }
    }

    public struct Plane
    {
        public Vector3 normal, position;

        public float A, B, C, D;


        public Plane( Vector3 position, Vector3 normal)
        {
            this.normal = normal;
            this.position = position;

            A = normal.X;
            B = normal.Y;
            C = normal.Z;
            D = -(normal.X * position.X + normal.Y * position.Y + normal.Z * position.Z);
        }

        public float DistanceTo(Vector3 P)
        {
            return (A * P.X + B * P.Y + C * P.Z + D) / normal.Length;
        }
    }
}
