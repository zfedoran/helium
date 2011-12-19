using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace helium.core
{
    public class Camera
    {
        public float fov, aspect, width, height, near, far;
        public Vector3 position, target, up;
        public Matrix view, projection, viewprojection;
        public BoundingFrustum boundingfrustum;

        public bool pauseBoundingFrustumUpdates;

        public Camera()
        {
            view = projection = viewprojection = Matrix.Identity;
            boundingfrustum = new BoundingFrustum(viewprojection);
        }

        public virtual void Update(double elapsed)
        {
            aspect = width / height;
            Matrix.CreateLookAt(ref position, ref target, ref up, out view);
            Matrix.CreatePerspectiveFieldOfView(fov, aspect, near, far, out projection);
            Matrix.Multiply(ref view, ref projection, out viewprojection);
            if (!pauseBoundingFrustumUpdates)
                boundingfrustum.Matrix = viewprojection;
        }

        public bool CullTest(ref BoundingBox aabb)
        {
            bool isVisible;
            boundingfrustum.Intersects(ref aabb, out isVisible);

            return !isVisible;
        }

        public Vector3 Project(Vector3 source)
        {
            Matrix matrix = Matrix.Multiply(view, projection);
            Vector3 vector = Vector3.Transform(source, matrix);
            float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;

            if (!WithinEpsilon(a, 1f))
                vector = (Vector3)(vector / a);

            vector.X = (((vector.X + 1f) * 0.5f) * width);
            vector.Y = (((-vector.Y + 1f) * 0.5f) * height);
            vector.Z = (vector.Z * (near - far)) + near;
            return vector;
        }

        private bool WithinEpsilon(float a, float b)
        {
            float num = a - b;
            return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
        }

        public static void CreateCameraMatrix(ref Vector3 rotation, ref Vector3 position, out Matrix matrix)
        {
            float a = (float)Math.Cos(rotation.X);
            float b = (float)Math.Sin(rotation.X);

            float c = (float)Math.Cos(rotation.Y);
            float d = (float)Math.Sin(rotation.Y);

            float e = (float)Math.Cos(rotation.Z);
            float f = (float)Math.Sin(rotation.Z);

            matrix = new Matrix();
            matrix.M11 = (c * e);
            matrix.M12 = (c * f);
            matrix.M13 = -d;
            matrix.M21 = (e * b * d - a * f);
            matrix.M22 = ((e * a) + (f * b * d));
            matrix.M23 = (b * c);
            matrix.M31 = (e * a * d + b * f);
            matrix.M32 = -(b * e - f * a * d);
            matrix.M33 = (a * c);
            matrix.M41 = position.X;
            matrix.M42 = position.Y;
            matrix.M43 = position.Z;
            matrix.M44 = 1;
        }

        public static void CreateCameraMatrix(ref Vector3 rotation, out Matrix matrix)
        {
            float a = (float)Math.Cos(rotation.X);
            float b = (float)Math.Sin(rotation.X);

            float c = (float)Math.Cos(rotation.Y);
            float d = (float)Math.Sin(rotation.Y);

            float e = (float)Math.Cos(rotation.Z);
            float f = (float)Math.Sin(rotation.Z);

            matrix = new Matrix();
            matrix.M11 = (c * e);
            matrix.M12 = (c * f);
            matrix.M13 = -d;
            matrix.M21 = (e * b * d - a * f);
            matrix.M22 = ((e * a) + (f * b * d));
            matrix.M23 = (b * c);
            matrix.M31 = (e * a * d + b * f);
            matrix.M32 = -(b * e - f * a * d);
            matrix.M33 = (a * c);
            matrix.M44 = 1;
        }
    }

}
