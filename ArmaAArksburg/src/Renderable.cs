public interface IRenderable
{
    public int Priority {get; set;}
    public void Render(ICellSurface surface);
}