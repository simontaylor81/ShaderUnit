namespace ShaderUnit.Maths
{
	public struct Vec2<T>
	{
		public T x;
		public T y;

		public Vec2(T x, T y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public struct Vec3<T>
	{
		public T x;
		public T y;
		public T z;

		public Vec3(T x, T y, T z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	public struct Vec4<T>
	{
		public T x;
		public T y;
		public T z;
		public T w;

		public Vec4(T x, T y, T z, T w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
	}

}

