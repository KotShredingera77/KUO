using System;


public class Vec3
{
    public double x;
    public double y;
    public double z;

    public Vec3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public double Magnitude()
    {
        return Math.Sqrt(x * x + y * y + z * z);
    }

    public static Vec3 operator *(Vec3 v, double d)
    {
        return new Vec3(v.x * d, v.y * d, v.z * d);
    }

    public static Vec3 operator *(double d, Vec3 v)
    {
        return new Vec3(v.x * d, v.y * d, v.z * d);
    }

    public static Vec3 operator -(Vec3 v1, Vec3 v2)
    {
        return new Vec3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public static Vec3 operator /(Vec3 v, double d)
    {
        return new Vec3(v.x / d, v.y / d, v.z / d);
    }

    public static Vec3 operator -(Vec3 v)
    {
        return new Vec3(-v.x, -v.y, -v.z);
    }

    public static Vec3 operator +(Vec3 v1, Vec3 v2)
    {
        return new Vec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    override public string ToString()
    {
        return string.Format("Vec3({0}, {1}, {2})", x, y, z);
    }
}