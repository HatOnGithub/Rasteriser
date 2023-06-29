using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Template
{
    public class Node
    {
        public Node? Parent;
        public Mesh? Mesh;
        public Texture? texture;
        public List<Node> Children;
        public Box3 untranslatedBounds;
        public Box3? Bounds => untranslatedBounds;
        public Camera cam => MyApplication.cam;

        public Node( Mesh? mesh, string textureFilePath)
        {
            Mesh = mesh;
            if (textureFilePath != "") texture = new Texture(textureFilePath);
            Children = new List<Node>();
        }

        public Node(Mesh? mesh, Color color)
        {
            Mesh = mesh;

            //untranslatedBounds = new Box3(Vector3.Zero, mesh.MaxSize);
            
            texture = new Texture(color);

            Children = new List<Node>();
        }

        public virtual void Init()
        {

        }

        public virtual void Tick()
        {

        }

        public virtual void AddChild(Node child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public virtual void AddChild(ref Node child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public virtual void RemoveChild(Node child)
        {
            Children.Remove(child);
        }

        public virtual void RemoveSelf()
        {
            Parent?.RemoveChild(this);
        }

        public virtual void Render(Shader shader, Matrix4 ConcatParentMatrices, Matrix4 WorldToCamera, Matrix4 CameraToScreen, Vector3 AmbientLight, Light[] lights, Vector3 camPos, Texture environmentMap)
        {
            Matrix4 ObjectToWorld = ConcatParentMatrices;

            if (Mesh != null)
            {
                ObjectToWorld = Mesh.localTransform * ObjectToWorld;
                if (cam.FrustumCollisionCheck(Mesh.boundingSphere, ObjectToWorld))
                {
                    SceneGraph.nrOfObjectsRendered++;
                    Mesh?.Render(shader, ObjectToWorld * WorldToCamera * CameraToScreen, ObjectToWorld, texture, AmbientLight, lights, camPos, environmentMap);
                }
            }

            foreach(var child in Children)
            {
                child.Render(shader, ObjectToWorld, WorldToCamera, CameraToScreen, AmbientLight, lights, camPos, environmentMap);
            }
        }
    }

    public struct Sphere
    {
        public Vector3 Center;
        public float Radius;

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool OnOrInFrontOfPlane( Plane plane)
        {
            return plane.DistanceTo(Center) >= -Radius;
        }
    }
}
