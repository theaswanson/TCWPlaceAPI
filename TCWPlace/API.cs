namespace TCWPlace
{
    public class API : IAPI
    {
        public const int MAX_X = 320;
        public const int MAX_Y = 180;
        public const int MIN_X = 0;
        public const int MIN_Y = 0;
        private const string URL = "http://10.60.2.11:3000";

        /// <summary>
        /// Gets the current state of the board. TODO: return parsed response
        /// </summary>
        /// <returns>JSON response as a string.</returns>
        public async Task<string> GetBoard()
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{URL}/get");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Builds a request that will render a single pixel.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">Hex code (without #)</param>
        /// <returns>A Task that will send the request when fired.</returns>
        public Task BuildRenderRequest((int X, int Y) coordinate, string color)
        {
            var client = new HttpClient();
            return client.GetAsync($"{URL}/change?x={X(coordinate.X)}&y={Y(coordinate.Y)}&col={color}");
        }

        /// <summary>
        /// Renders the color over the entire board.
        /// </summary>
        /// <param name="color">Hex code (without #)</param>
        /// <returns></returns>
        public async Task Clear(string color)
        {
            var requests = new List<Task>();
            for (int i = MIN_X; i < MAX_X; i++)
            {
                for (int j = MIN_Y; j < MAX_Y; j++)
                {
                    requests.Add(BuildRenderRequest((i, j), color));
                }
            }
            await SendRequests(requests);
        }

        /// <summary>
        /// Draws a single pixel.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">Hex code (without #)</param>
        /// <returns></returns>
        public async Task RenderPixel((int X, int Y) coordinate, string color)
        {
            await BuildRenderRequest(coordinate, color);
        }

        /// <summary>
        /// Render a rectangle with the given start and end coordinates.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color">Hex code (without #)</param>
        /// <returns></returns>
        public async Task RenderRectangle((int X, int Y) start, (int X, int Y) end, string color)
        {
            if (end.X <= start.X)
                throw new Exception("The starting X coordinate must be less than the ending X coordinate.");
            if (end.Y <= start.Y)
                throw new Exception("The starting Y coordinate must be less than the ending Y coordinate.");

            var requests = new List<Task>();
            for (int i = X(start.X); i < X(end.X); i++)
            {
                for (int j = Y(start.Y); j < Y(end.Y); j++)
                {
                    requests.Add(BuildRenderRequest((i, j), color));
                }
            }
            await SendRequests(requests);
        }

        /// <summary>
        /// Renders text. No text wrapping.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="start"></param>
        /// <param name="font"></param>
        /// <param name="color">Text color. Hex code (without #)</param>
        /// <param name="backgroundColor">Background color. Hex code (without #)</param>
        /// <returns></returns>
        public async Task RenderText(string text, (int X, int Y) start, Figgle.FiggleFont font, string color, string? backgroundColor = null)
        {
            var renderAt = start;
            var asciiArt = font.Render(text);

            var requests = new List<Task>();
            foreach (var line in asciiArt.Split(Environment.NewLine))
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] != ' ')
                    {
                        requests.Add(BuildRenderRequest(renderAt, color));
                    }
                    else
                    {
                        if (backgroundColor != null)
                        {
                            requests.Add(BuildRenderRequest(renderAt, backgroundColor));
                        }
                    }
                    renderAt.X++;
                }
                renderAt.X = start.X;
                renderAt.Y++;
            }

            await SendRequests(requests);
        }

        /// <summary>
        /// Renders lines of text. No text wrapping.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="start"></param>
        /// <param name="font"></param>
        /// <param name="color">Text color. Hex code (without #)</param>
        /// <param name="backgroundColor">Background color. Hex code (without #)</param>
        /// <returns></returns>
        public async Task RenderLines(IEnumerable<string> lines, (int X, int Y) start, Figgle.FiggleFont font, string color, string? backgroundColor = null)
        {
            foreach (var line in lines)
            {
                await RenderText(line, start, font, color, backgroundColor);
                start.Y += font.Height + 1;
            }
        }

        /// <summary>
        /// Renders lines of text from a file. No text wrapping.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="start"></param>
        /// <param name="font"></param>
        /// <param name="color">Text color. Hex code (without #)</param>
        /// <param name="backgroundColor">Background color. Hex code (without #)</param>
        /// <returns></returns>
        public async Task RenderLinesFromFile(string filePath, (int X, int Y) start, Figgle.FiggleFont font, string color, string? backgroundColor = null)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            await RenderLines(lines, start, font, color, backgroundColor);
        }

        /// <summary>
        /// Runs the given action over and over. Delay and number of retries configurable.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delay"></param>
        /// <param name="retries"></param>
        /// <returns></returns>
        public static async Task Defend(Func<Task> action, int delay = 1000, int retries = int.MaxValue)
        {
            while (retries > 0)
            {
                await action();
                Thread.Sleep(delay);
                retries--;
            }
        }

        private static async Task SendRequests(IEnumerable<Task> requests) => await SendRequests(requests.ToArray());

        private static async Task SendRequests(Task[] requests) => await Task.WhenAll(requests);

        private static int X(int x) =>
            x switch
            {
                < MIN_X => MIN_X,
                > MAX_X => MAX_X,
                _ => x
            };

        private static int Y(int y) =>
            y switch
            {
                < MIN_Y => MIN_Y,
                > MAX_X => MAX_Y,
                _ => y
            };
    }
}
