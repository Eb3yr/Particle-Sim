using System.Drawing;
using System.Numerics;
using static ParticleLib.ParticlePhysics;

namespace ParticleLib
{
	public class Particle	// While I'm very tempted to use a struct, this makes my life easier with operating upon the particles in Physics.cs
	{
		// Consider making them all fields for the sake of performance - not having to deal with getters cloning structs instead of passing a reference
		public int Num { get; set; }	// What purpose does this serve?
		public float Mass { get; set; }
		public float Charge { get; set; }
		public float Rad { get; set; }
		public float HalfLife { get; set; }
		public Vector2 Pos { get; set; }
		public Vector2 Vel { get; set; }
		public Color Color { get; set; }

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

		public static void UpdateCollection(float deltaTime, IList<Particle> particles, int width, int height)   // Arrays and Lists both implement IList
		{
			for (int i = 0; i < particles.Count; i++)
			{
				Particle left = particles[i];
				
				// Makes use of the sorted nature of the collection
				for (int j = i + 1; j < particles.Count; j++)
				{
					Particle right = particles[j];

					if (right.Pos.X - right.Rad > left.Pos.X + left.Rad)
						break;  // Not continue; due to sorting. Anything further than this point will also meet this condition

					if (DoesIntersect(left, right))
						Collide(left, right);

					// Bounce off window walls
					if (left.Pos.X <= left.Rad || left.Pos.X >= width - left.Rad)
						left.Vel = new Vector2(-left.Vel.X, left.Vel.Y);
					if (left.Pos.Y <= left.Rad || left.Pos.Y >= height - left.Rad)
						left.Vel = new Vector2(left.Vel.X, -left.Vel.Y);
				}
			}

			foreach (Particle p in particles)
				p.Update(deltaTime);
		}
	}
}
