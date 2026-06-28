public sealed class MessageLog : Console
{
    public void LogMessage(string message) // logs a message into the log
    {
        Cursor.Print("- ");
        foreach (string line in message.Split('\n'))
        {
            ColoredString parsedString = ColoredString.Parser.Parse(line);
            Cursor.Print(parsedString);
            Cursor.NewLine();
        }
    }

    public MessageLog(int x, int y, int width, int height) : base(width, height)
    {
        Position = (x, y);
        Surface.DefaultForeground = Color.Green;
        Surface.DefaultBackground = Color.Black;
        Surface.Clear();

        // set up cursor
        Cursor.Move(0, 0);
    }
}