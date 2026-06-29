using System.Text;

public static class SaveManager
{
    private const string SAVE_FILE_PATH = "../../../Saves/save.sv";
    public static void SaveGame() // saves game state
    {
        BinaryWriter writer = new(File.Open(SAVE_FILE_PATH, FileMode.Create));
        Engine.Instance!.GameManager.Save(writer);
        writer.Close();
    }

    public static void LoadGame() // loads game
    {
        BinaryReader reader = new(File.Open(SAVE_FILE_PATH, FileMode.Open));
        Engine.Instance!.GameManager.Load(reader);
        reader.Close();
    }

    public static bool SaveExists()
    {
        return File.Exists(SAVE_FILE_PATH);
    }

    public static void SaveColoredGlyph(ColoredGlyph glyph, BinaryWriter writer)
    {
        writer.Write(glyph.Foreground.PackedValue);
        writer.Write(glyph.Background.PackedValue);
        writer.Write(glyph.Glyph);
        writer.Write((int)glyph.Mirror);
    }

    public static ColoredGlyph LoadColoredGlyph(BinaryReader reader)
    {
        ColoredGlyph glyph = new()
        {
            Foreground = new Color(reader.ReadUInt32()),
            Background = new Color(reader.ReadUInt32()),
            Glyph = reader.ReadInt32(),
            Mirror = (Mirror)reader.ReadInt32()
        };
        return glyph;
    }

    public static void SavePoint(Point point, BinaryWriter writer)
    {
        writer.Write(point.X);
        writer.Write(point.Y);
    }

    public static Point LoadPoint(BinaryReader reader)
    {
        Point point = new(reader.ReadInt32(), reader.ReadInt32());
        return point;
    }
}