using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FicusSimulator.Core;

public static class Globals
{
    public static SpriteBatch SpriteBatch { get; private set; }
    public static GraphicsDevice GraphicsDevice { get; private set; }
    public static ContentManager Content { get; private set; }
    public static Random Random { get; private set; } = new Random();

    public static Texture2D WhiteTexture { get; private set; }
    public static Point VirtualGameSize => GraphicsDevice.Viewport.Bounds.Size;

    public static GameTime GameTime { get; set; }
    public static float DeltaTime => (float)GameTime.ElapsedGameTime.TotalSeconds;


    public static void Load(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager)
    {
        SpriteBatch = spriteBatch;
        GraphicsDevice = graphicsDevice;
        Content = contentManager;

        WhiteTexture = new Texture2D(graphicsDevice, 1, 1);
        WhiteTexture.SetData([Color.White]);
    }

    public static void Update(GameTime gameTime)
    {
        GameTime = gameTime;
    }

    public static T[,] ResizeArray<T>(T[,] original, int rows, int cols)
    {
        var newArray = new T[rows, cols];
        int minRows = Math.Min(rows, original.GetLength(0));
        int minCols = Math.Min(cols, original.GetLength(1));
        for (int i = 0; i < minRows; i++)
            for (int j = 0; j < minCols; j++)
                newArray[i, j] = original[i, j];
        return newArray;
    }
}