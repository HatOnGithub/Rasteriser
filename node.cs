using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    public class Node
    {
        public Node? Parent;
        public Mesh? Mesh;
        public Texture? texture;
        public List<Node> Children;

        public Node( Mesh? mesh, string textureFilePath)
        {
            Mesh = mesh;
            if (textureFilePath != "") texture = new Texture(textureFilePath);
            Children= new List<Node>();
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

        public virtual void RemoveChild(Node child)
        {
            Children.Remove(child);
        }

        public virtual void RemoveSelf()
        {
            Parent?.RemoveChild(this);
        }

        public virtual void Render(Shader shader, Matrix4 ConcatParentMatrices, Matrix4 WorldToCamera, Matrix4 CameraToScreen, Vector3 AmbientLight, Vector3[] lightPositions, Vector3[] colors, Vector3 camPos)
        {
            Matrix4 ObjectToWorld = ConcatParentMatrices;

            if (Mesh != null)
            {
                ObjectToWorld = Mesh.localTransform * ObjectToWorld;
                Mesh?.Render(shader, ObjectToWorld * WorldToCamera * CameraToScreen, ObjectToWorld, texture, AmbientLight, lightPositions, colors, camPos);
            }

            foreach(var child in Children)
            {
                child.Render(shader, ObjectToWorld, WorldToCamera, CameraToScreen, AmbientLight, lightPositions, colors, camPos);
            }
        }
    }
}
