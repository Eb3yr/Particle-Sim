using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ParticlePort;

public class ParticleGame : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;

	public int Width { get => Window.ClientBounds.Width; }
	public int Height { get => Window.ClientBounds.Height; }
	public static readonly float GlobalTimeStep = 1f;	// Because gameTime will still vary even with a fixed target timestep if it can't keep up, and it'll limit fps if it can.
	List<Particle> particles;
	Texture2D sphereTex;
	Texture2D pixel;
	float DoubleInverseTexSize;

	public ParticleGame()
	{
		_graphics = new GraphicsDeviceManager(this);
		_graphics.SynchronizeWithVerticalRetrace = false;   //. Vsync disabled
		IsFixedTimeStep = false;    // Disabled fps cap. Can toggle for forced slowdown and use a slider for the target.
		IsFixedTimeStep = true;
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		Window.AllowUserResizing = true;

		//_graphics.PreferredBackBufferWidth = 1280;
		//_graphics.PreferredBackBufferHeight = 720;
	}

	protected override void Initialize()
	{
		base.Initialize();

		Random rng = new();
		particles = [];
		for (int i = 0; i < 25_000; i++)
		    particles.Add(new Particle(i, 1f, 0f, /*Radius:*/ 2f, 100f, new Vector2(rng.Next(0, Width), rng.Next(0, Height)), new Vector2(rng.NextSingle(), rng.NextSingle()), Color.White));

		backCol = Color.CornflowerBlue;
		backCol.A = 64;	// Higher alpha = shorter trail
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);
		sphereTex = Content.Load<Texture2D>("Sphere");
		DoubleInverseTexSize = 2f / sphereTex.Width;    // Matches up to radius
		pixel = new(GraphicsDevice, 1, 1);
		pixel.SetData<Color>([Color.White]);
		// Might like LoDs for sphere sizes. Hell, make a register of every radius and an associated texture to draw. Currently squashed textures look a bit jank.
	}

	DateTime cLastPressed = DateTime.Now;
	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();
		if (Keyboard.GetState().IsKeyDown(Keys.C) && DateTime.Now.Subtract(cLastPressed).TotalMilliseconds > 250)
		{
			IsFixedTimeStep ^= true;
			cLastPressed = DateTime.Now;
		}
		// Consider special form resize case where it's fullscreened, offset the particles by the opposite of the distance the form corner travels so that they stay in place. 

		Particle.UpdateCollection(GlobalTimeStep, particles, Width, Height);

		base.Update(gameTime);
	}

	byte fpsTicker = 0;
	DateTime prevDrawTime = DateTime.Now;
	Color backCol;
	protected override void Draw(GameTime gameTime)
	{
		fpsTicker++;
		if ((fpsTicker & 15) == 0)
			Window.Title = "fps: " + double.Round(1d / DateTime.Now.Subtract(prevDrawTime).TotalSeconds, 3);
		prevDrawTime = DateTime.Now;

		//GraphicsDevice.Clear(Color.CornflowerBlue);

		_spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
		_spriteBatch.Draw(pixel, new Rectangle(0, 0, Width, Height), backCol);
		_spriteBatch.End();


		_spriteBatch.Begin(samplerState: SamplerState.PointClamp /* Prevents anti-aliasing from scaling */);
		// Each particle's coordinate is at their centroid, so offset and draw as such.
		foreach (Particle p in particles)
			// Would like to do this without having to create a new vector every single time, maybe add it as a readonly field to each particle. Shame vectors have no scalar subtraction.
			_spriteBatch.Draw(sphereTex, p.Pos - new Vector2(p.Rad, p.Rad), null, p.Color, 0f, Vector2.Zero, p.Rad * DoubleInverseTexSize, SpriteEffects.None, 0f);
		
		_spriteBatch.End();

		base.Draw(gameTime);
	}
}
