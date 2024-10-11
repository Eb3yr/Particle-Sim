using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ParticleDraw;

public class Game1 : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;

	public static int Width { get => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; }
	public static int Height { get => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; }
	List<Particle> particles;
	Texture2D sphereTex;
	float inverseTexSize;
	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		base.Initialize();

		Random rng = new();
		// Placeholder, forgot how to do properly. Must change later!
		int width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
		int height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
		particles = [];
		//for (int i = 0; i < 15_000; i++)
		//    particles.Add(new Particle(i, 1f, 0f, 8f, 100f, new Vector2(rng.Next(0, width), rng.Next(0, height)), new Vector2(rng.NextSingle(), rng.NextSingle()), Color.White));

		particles.Add(new Particle(0, 1f, 0f, 10f, 1000f, new Vector2(100, 100), new Vector2(0, 0), Color.White));
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);
		sphereTex = Content.Load<Texture2D>("Sphere");
		inverseTexSize = 1f / sphereTex.Width;
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		// check if form resizes, move particles if true
		// TODO: Add your update logic here

		Particle.UpdateCollection(0.00f, particles, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

		// Get width, height with GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/Height;

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);
		
		_spriteBatch.Begin(samplerState: SamplerState.PointClamp /* Prevents anti-aliasing from scaling */);

		foreach (Particle p in particles)
			_spriteBatch.Draw(sphereTex, p.Pos, null, p.Color, 0f, Vector2.Zero, p.Rad * inverseTexSize, SpriteEffects.None, 0f);
		
		_spriteBatch.End();

		base.Draw(gameTime);
	}
}
