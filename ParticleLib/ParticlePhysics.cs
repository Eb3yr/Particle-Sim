using System.Numerics;

namespace ParticleLib
{
	// Assume SI units used unless otherwise stated
	public static class ParticlePhysics
	{
		public static bool DoesIntersect(Particle left, Particle right)
		{
			Vector2 delta = right.Pos - left.Pos;
			float radSum = left.Rad + right.Rad;
			if (delta.LengthSquared() < radSum * radSum)
				return true;

			return false;
		}

		public static void Collide(Particle left, Particle right)
		{
			// Assume only called when DoesIntersect returns true
			Vector2 delta = right.Pos - left.Pos;
			// Considering vectorising below, but that induces overhead with struct creation, however small it may be. 
			// But all those float allocations... would get allocated behind the scenes anyway. Fuck it
			// But Vector2s can take advantage of SIMD...
			// Run the profiler


			float phi;
			if (delta.X == 0)
				phi = float.Pi * 0.5f;
			else
				phi = float.Atan(delta.Y / delta.X);

			float overlap = left.Rad + right.Rad - delta.Length();

			left.Pos -= new Vector2(0.5f * overlap * float.Cos(phi), 0.5f * overlap * float.Sin(phi));
			right.Pos += new Vector2(0.5f * overlap * float.Cos(phi), 0.5f * overlap * float.Sin(phi));

			float u1 = left.Vel.Length();
			float u2 = right.Vel.Length();
			float theta1 = float.Atan2(left.Vel.Y, left.Vel.X);
			if (theta1 < 0f)
				theta1 += float.Tau;

			float theta2 = float.Atan2(right.Vel.Y, right.Vel.X);
			if (theta2 < 0f)
				theta2 += float.Tau;

			float u1xr = u1 * float.Cos(theta1 - phi);
			float u1yr = u1 * float.Sin(theta1 - phi);
			float u2xr = u2 * float.Cos(theta2 - phi);
			float u2yr = u2 * float.Sin(theta2 - phi);

			float v1xr = ((left.Mass - right.Mass) * u1xr + (2f * right.Mass) * u2xr) / (left.Mass + right.Mass);
			float v2xr = ((2f * left.Mass) * u1xr + (right.Mass - left.Mass) * u2xr) / (left.Mass + right.Mass);
			float v1yr = u1yr;
			float v2yr = u2yr;

			float v1x = float.Cos(phi) * v1xr + float.Cos(phi + float.Pi * 0.5f) * v1yr;
			float v2x = float.Cos(phi) * v2xr + float.Cos(phi + float.Pi * 0.5f) * v2yr;
			float v1y = float.Sin(phi) * v1xr + float.Sin(phi + float.Pi * 0.5f) * v1yr;
			float v2y = float.Sin(phi) * v2xr + float.Sin(phi + float.Pi * 0.5f) * v2yr;

			left.Vel = new Vector2(v1x, v1y);	// Can't directly assign to fields as Vel is a property, not a field. Consider making them all fields for performance reasons
			right.Vel = new Vector2(v2x, v2y);
		}

		public static void CollisionWithVectors(Particle left, Particle right)
		{
			// Copy-paste and vectorise above, then profile
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the scalar magnitude of the graviational force between two particles. Multiply by the unit vector of the vector distance between the two for the vector force.
		/// </summary>
		public static float GravForceScalar(Particle left, Particle right)
		{
			return 6.6743015E-11f * left.Mass * right.Mass / (right.Pos - left.Pos).LengthSquared();
		}

		/// <summary>
		/// Returns the vector force exerted by the right particle on the left particle. Mutliply by -1 for vice versa.
		/// </summary>
		public static Vector2 GravForceVector(Particle left, Particle right)
		{
			Vector2 separation = right.Pos - left.Pos;
			float sepSquared = separation.LengthSquared();	// Avoiding unecessary sqrt
			separation = Vector2.Normalize(separation);
			return separation * (6.6743015E-11f * left.Mass * right.Mass / sepSquared);
		}
	}
}
