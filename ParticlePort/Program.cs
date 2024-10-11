using System.Numerics;
using ParticleLib;

bool exit = false;
int width = 1600;
int height = 900;

List<Particle> lp = [];
float deltaTime = 0.001f;   // 1ms
while (!exit)
{
	Particle.UpdateCollection(deltaTime, lp, width, height);
}

Console.WriteLine("Done");
Console.ReadLine();

// TODO: Get particles drawing somewhat efficiently in monogame.