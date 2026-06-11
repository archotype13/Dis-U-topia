using System.Security.Cryptography.X509Certificates;

public class Level
{
    public struct Tile(int id)
    {
        public int Id = id;
    }

    public int Width;
    public int Height;
    private Tile[] Tiles;


    public Tile GetTileAt(int x, int y)
    {
        return Tiles[x + y*Width];
    }

    public Tile GetTileAt(Point point) { return GetTileAt(point.X, point.Y); }

    public void SetTileAt(int x, int y, Tile tile)
    {
        Tiles[x + y*Width] = tile;
    }

    public void SetTileAt(Point point, Tile tile) { SetTileAt(point.X, point.Y, tile); }

    public Level(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new Tile[width * height];
    }
}