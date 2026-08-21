using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Quaternion3D
	{
		protected float x;
		protected float y;
		protected float z;
		protected float w;

        public Quaternion3D(float fx, float fy, float fz, float fw)
		{
			x = fx;
			y = fy;
			z = fz;
			w = fw;
		}

		public  float X
		{
			get { return x; }
			set { x = value; }
        }

		public float Y
		{
			get { return y; }
			set { y = value; }
		}

		public float Z
		{
			get { return z; }
			set { z = value; }
		}

		public float W
		{
			get { return w; }
			set { w = value; }
		}


		bool Equals(Quaternion3D pos)
		{
			if( pos.x != x)
				return false;
			if( pos.y != y)
				return false;
			if( pos.z != z)
				return false;
			if( pos.w != w)
				return false;
			return true;
		}
	}

	public class Position3D 
	{

		protected float x;
		protected float y;
		protected float z;



		public Position3D(float fx, float fy, float fz)
		{
			x = fx;
			y = fy;
			z = fz;
		}
		
		public float X
		{
			get { return x; }
			set { x = value; }
		}
		
		public float Y
		{
			get { return y; }
			set { y = value; }
		}

		public float Z
		{
			get { return z; }
			set { z = value; }
		}

		bool Equals(Position3D pos)
		{
			if( pos.x != x)
				return false;
			if( pos.y != y)
				return false;
			if( pos.z != z)
				return false;
			return true;
		}		
	}
}
