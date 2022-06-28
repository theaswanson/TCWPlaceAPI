# TCWPlaceAPI

A simple .NET API to interact with [tcw_place](https://github.com/upitroma/tcw_place).

## Usage

```csharp
// Create the API
var api = new API();

// Get the state of the board
var board = api.GetBoard();
Console.WriteLine(board);

// Build a request and fire it yourself
var request = api.BuildRenderRequest((0, 0), "ffffff");
await Task.WhenAll(request);

// Clear the board
await api.Clear("ffffff");

// Render a white pixel at 0,0 (top-left)
await api.RenderPixel((0, 0), "ffffff");

// Render a 50x50 blue square at bottom-right
await api.RenderRectangle((API.MAX_X - 50, API.MAX_Y - 50), (API.MAX_X, API.MAX_Y), "0000ff");

// Render red text on white background using the 3x5 ASCII font
await api.RenderText("Hello, world!", (0, 0), Figgle.FiggleFonts.ThreeByFive, "ff0000", "ffffff");

// Render multiple lines of black text using the Banner ASCII font
await api.RenderLines(new List<string> { "Hello", "there" }, (0, 0), Figgle.FiggleFonts.Banner, "000000");

// Render multiple lines of text from a file
await api.RenderLinesFromFile(Path.Combine(Environment.CurrentDirectory, "file.txt"), (0, 0), Figgle.FiggleFonts.Banner, "ffffff");

// Redraw your creation every second
await API.Defend(() => api.RenderRectangle((0, 0), (10, 10), "00ff00"));

// Redraw your creation every second 10 times
await API.Defend(() => api.RenderRectangle((0, 0), (10, 10), "00ff00"), retries: 10);

// Redraw your creation every 5 seconds 
await API.Defend(() => api.RenderRectangle((0, 0), (10, 10), "00ff00"), delay: 5000);
```