using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ParticleLib;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ParticleDraw;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    List<Particle> particles;

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
        // Placeholder, forgot how to do properly
        int width = 1600;
        int height = 900;
        particles = [];
        for (int i = 0; i < 15_000; i++)    // Convert to XNA vectors to avoid casting constantly. Likewise for System.Drawing.Color and Xna Color
            particles.Add(new Particle(i, 1f, 0f, 8f, 100f, new System.Numerics.Vector2(rng.Next(0, width), rng.Next(0, height)), new System.Numerics.Vector2(rng.NextSingle(), rng.NextSingle()), System.Drawing.Color.White));
    
        
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
