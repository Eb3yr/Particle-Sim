using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace ParticlePort
{
	public class Particle   // While I'm very tempted to use a struct, this makes my life easier with operating upon the particles in Physics.cs
	{
		// Ordinarily these would be properties, but switching to fields DOUBLED fps
		public int Num;    // What purpose does this serve? Atomic number?
		public float Mass;
		public float Charge;
		public float Rad;
		public float HalfLife;
		public Vector2 Pos;
		public Vector2 Vel;
		public Color Color;

		public static readonly Comparison<Particle> PosComparer = (x, y) =>
		{
			Vector2 delta = y.Pos - x.Pos;
			if (delta.X < 0f)
				return 1; // x > y

			if (delta.X == 0f)
			{
				if (delta.Y < 0f)
					return 1;
				else if (delta.Y == 0f)
					return 0;
				return -1;
			}
			return -1;

		};
		// Could have an alternate comparer that compares x and y distance as a sum, and tie that in to a slightly different optimisation for when to break. IDK how well that would work though.

		public Particle(int Num, float Mass, float Charge, float Radius, float HalfLife, Vector2 Position, Vector2 Velocity, Color Color)
		{
			this.Num = Num;
			this.Mass = Mass;
			this.Charge = Charge;
			this.Rad = Radius;
			this.HalfLife = HalfLife;
			this.Pos = Position;
			this.Vel = Velocity;
			this.Color = Color;
		}

		public void Update(float deltaTime)
		{
			Pos += Vel * deltaTime;
		}

		public Particle[] Decay()
		{
			throw new NotImplementedException();
		}

		public static void UpdateCollection(float deltaTime, List<Particle> particles, int width, int height)
		{
			// collision -> apply graviational forces -> move
			// Order doesn't really matter much, since this'll be repeating every frame. Might matter a bit, IDK.
			particles.Sort(PosComparer);    // Takes about 8-10% of the total method time

			// Collision
			Particle left, right;
			for (int i = 0; i < particles.Count; i++)
			{
				left = particles[i];

				// Makes use of the sorted nature of the collection

				for (int j = i + 1; j < particles.Count; j++)
				{
					right = particles[j];

					

					if (right.Pos.X - right.Rad > left.Pos.X + left.Rad)
						break;  // Not continue; due to sorting. Anything further than this point will also meet this condition

					if (DoesIntersect(left, right))
						Collide(left, right);
				}

				// Bounce off window walls
				// A bit off. Is this including the top bar?

				// IMPORTANT: my coordinates draw from top-left, not from centre!
				if (left.Pos.X < left.Rad)
				{
					if (left.Vel.X < 0)
						left.Vel.X *= -1f;
				}
				else if (left.Pos.X > width - left.Rad)
					if (left.Vel.X > 0)
						left.Vel.X *= -1f;

				if (left.Pos.Y < left.Rad)
				{
					if (left.Pos.Y < 0)
						left.Vel.Y *= -1f;
				}
				else if (left.Pos.Y > height - left.Rad)
					if (left.Pos.Y > 0)
						left.Vel.Y *= -1f;
			}

			// Gravity and other external forces
			for (int i = 0; i < particles.Count; i++)
			{

			}

			// Move with velocity * deltaTime
			foreach (Particle p in particles)
				p.Update(deltaTime);
		}

		#region Physics

		private static bool DoesIntersect(Particle left, Particle right)
		{
			float radSum = left.Rad + right.Rad;
			return Vector2.DistanceSquared(left.Pos, right.Pos) < radSum * radSum;	// DistanceSquared takes advantage of an intrinsic for a big speedup
		}

		private static void Collide(Particle left, Particle right)
		{
			// Assume only called when DoesIntersect returns true
			Vector2 delta = right.Pos - left.Pos;
			// Considering vectorising below, but that induces overhead with struct creation, however small it may be. 
			// But all those float allocations... would get allocated behind the scenes anyway. Fuck it
			// But Vector2s can take advantage of SIMD...

			// Would like to optimise this one more...
			// Surely SOMETHING I can do with SIMD
			//
			//

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

			left.Vel = new Vector2(v1x, v1y);   // Can't directly assign to fields as Vel is a property, not a field. Consider making them all fields for performance reasons
			right.Vel = new Vector2(v2x, v2y);
		}

		private static void CollisionWithVectors(Particle left, Particle right)
		{
			// Copy-paste and vectorise above, then profile
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the scalar magnitude of the graviational force between two particles. Multiply by the unit vector of the vector distance between the two for the vector force.
		/// </summary>
		private static float GravForceScalar(Particle left, Particle right)
		{
			return 6.6743015E-11f * left.Mass * right.Mass / (right.Pos - left.Pos).LengthSquared();
		}

		/// <summary>
		/// Returns the vector force exerted by the right particle on the left particle. Mutliply by -1 for vice versa.
		/// </summary>
		private static Vector2 GravForceVector(Particle left, Particle right)
		{
			Vector2 separation = right.Pos - left.Pos;
			float sepSquared = separation.LengthSquared();  // Avoiding unecessary sqrt
			separation = Vector2.Normalize(separation);
			return separation * (6.6743015E-11f * left.Mass * right.Mass / sepSquared);
		}
		#endregion
	}
}
