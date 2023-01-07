using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Inheritance.Geometry.Virtual
{
    public abstract class Body
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract bool ContainsPoint(Vector3 point);

        public abstract RectangularCuboid GetBoundingBox();
    }

    public class Ball : Body
    {
        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var vector = point - Position;
            var length2 = vector.GetLength2();
            return length2 <= Radius * Radius;
        }

        public override RectangularCuboid GetBoundingBox()
        {
            return new RectangularCuboid(Position, Radius*2, Radius*2, Radius*2);
        }
    }

    public class RectangularCuboid : Body
    {
        public double SizeX { get; }
        public double SizeY { get; }
        public double SizeZ { get; }

        public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var minPoint = new Vector3(
                Position.X - SizeX / 2,
                Position.Y - SizeY / 2,
                Position.Z - SizeZ / 2);
            var maxPoint = new Vector3(
                Position.X + SizeX / 2,
                Position.Y + SizeY / 2,
                Position.Z + SizeZ / 2);
            return point >= minPoint && point <= maxPoint;
        }

        public override RectangularCuboid GetBoundingBox()
        {
            return this;
        }
    }

    public class Cylinder : Body
    {
        public double SizeZ { get; }

        public double Radius { get; }

        public Cylinder(Vector3 position, double sizeZ, double radius) : base(position)
        {
            SizeZ = sizeZ;
            Radius = radius;
        }

        public override bool ContainsPoint(Vector3 point)
        {
            var vectorX = point.X - Position.X;
            var vectorY = point.Y - Position.Y;
            var length2 = vectorX * vectorX + vectorY * vectorY;
            var minZ = Position.Z - SizeZ / 2;
            var maxZ = minZ + SizeZ;
            return length2 <= Radius * Radius && point.Z >= minZ && point.Z <= maxZ;
        }

        public override RectangularCuboid GetBoundingBox()
        {
            return new RectangularCuboid(Position, Radius * 2, Radius * 2, SizeZ);
        }
    }

    public class CompoundBody : Body
    {
        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override bool ContainsPoint(Vector3 point)
        { 
            return Parts.Any(body => body.ContainsPoint(point));
        }

        public override RectangularCuboid GetBoundingBox()
        {
            var lowX = 0.0;
            var lowY = 0.0;
            var lowZ = 0.0;
            var highX = 0.0;
            var highY = 0.0;
            var highZ = 0.0;
            
            foreach (var body in Parts)
            {
                var currentBox = body.GetBoundingBox();

                if(currentBox.Position.X - currentBox.SizeX / 2 <= lowX)
                    lowX = currentBox.Position.X - currentBox.SizeX / 2;
                if(currentBox.Position.Y - currentBox.SizeY / 2 <= lowY)
                    lowY = currentBox.Position.Y - currentBox.SizeY / 2;
                if (currentBox.Position.Z - currentBox.SizeZ / 2 <= lowZ)
                    lowZ = currentBox.Position.Z - currentBox.SizeZ / 2;
                
                if(currentBox.Position.X + currentBox.SizeX / 2 >= highX)
                    highX = currentBox.Position.X + currentBox.SizeX / 2;
                if(currentBox.Position.Y + currentBox.SizeY / 2 >= highY)
                    highY = currentBox.Position.Y + currentBox.SizeY / 2;
                if (currentBox.Position.Z + currentBox.SizeZ / 2 >= highZ)
                    highZ = currentBox.Position.Z + currentBox.SizeZ / 2;
            }
            
            var sizeZ = highZ - lowZ;
            // if (lowZ == 0) sizeZ -= 1;
            return new RectangularCuboid(Position, highX - lowX, highY - lowY, sizeZ);
        }
    }
}