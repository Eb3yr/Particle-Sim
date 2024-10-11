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
		int width = 1600;
		int height = 900;
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

		// TODO: Add your update logic here

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
