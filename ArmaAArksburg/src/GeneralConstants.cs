public static class GeneralConstants
{
    public enum DrawingOrders
    {
        DOORS,
        CORPSES,
        NPCS,
        PLAYER,
    }

    public static Color GrayscaleColor(Color color)
    {
        Color newColor = new(color.R * 0.2126f, color.G * 0.7152f, color.B * 0.0722f, color.A);
        System.Console.WriteLine($"Old color: {color.R}, {color.G}, {color.B}. New Color: {newColor.R}, {newColor.G}, {newColor.B}");
        return newColor;
    }
}