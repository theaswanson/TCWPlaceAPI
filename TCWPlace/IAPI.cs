using Figgle;

namespace TCWPlace
{
    public interface IAPI
    {
        Task<string> GetBoard();
        Task BuildRenderRequest((int X, int Y) coordinate, string color);
        Task Clear(string color);
        Task RenderPixel((int X, int Y) coordinate, string color);
        Task RenderRectangle((int X, int Y) start, (int X, int Y) end, string color);
        Task RenderText(string text, (int X, int Y) start, FiggleFont font, string color, string? backgroundColor = null);
        Task RenderLines(IEnumerable<string> lines, (int X, int Y) start, FiggleFont font, string color, string? backgroundColor = null);
        Task RenderLinesFromFile(string filePath, (int X, int Y) start, FiggleFont font, string color, string? backgroundColor = null);
        Task Defend(Func<Task> action, int delay = 1000, int retries = int.MaxValue);
    }
}