using Autoblock.src;
using GBX.NET;
using GBX.NET.Inputs;
using GBX.NET.LZO;
using Photino.NET.Server;
using PhotinoNET;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace Autoblock
{
    class Message
    {
        public string Command { get; set; } = String.Empty;
        public string Data { get; set; } = String.Empty;

        public static string Error(string error)
        {
            return JsonSerializer.Serialize(new Message
            {
                Command = "error",
                Data = error
            });
        }

        public Message(string command = "", string data = "")
        {
            Command = command;
            Data = data;
        }
    };


    class Program
    {
        static JsonSerializerOptions json_options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

        };

        [STAThread]
        static void Main(string[] args)
        {

            Gbx.LZO = new Lzo();

            PhotinoServer
            .CreateStaticFileServer(args, out string baseUrl)
            .RunAsync();

            // Window title declared here for visibility
            string windowTitle = "Autoblock";

            System.Diagnostics.Debug.WriteLine($"Starting {windowTitle}...");
            // Creating a new PhotinoWindow instance with the fluent API
            var window = new PhotinoWindow()
                .SetTitle(windowTitle)
                // Resize to a percentage of the main monitor work area
                .SetUseOsDefaultSize(true)
                // Center window in the middle of the screen
                .Center()
                // Users can resize windows by default.
                // Let's make this one fixed instead.
                .SetResizable(true)
                // .RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
                // {
                //     contentType = "text/javascript";
                //     return new MemoryStream(Encoding.UTF8.GetBytes(@"
                //         (() =>{
                //             window.setTimeout(() => {
                //                 alert(`🎉 Dynamically inserted JavaScript.`);
                //             }, 1000);
                //         })();
                //     "));
                // })
                // Most event handlers can be registered after the
                // PhotinoWindow was instantiated by calling a registration 
                // method like the following RegisterWebMessageReceivedHandler.
                // This could be added in the PhotinoWindowOptions if preferred.
                .RegisterWebMessageReceivedHandler((object sender, string message) =>
                {
                    var window = (PhotinoWindow)sender;

                    // The message argument is coming in from sendMessage.
                    // "window.external.sendMessage(message: string)

                    // Handle the message
                    HandleMessage(window, message);
                    // Send a message back the to JavaScript event handler.
                    // "window.external.receiveMessage(callback: Function)"
                    //window.SendWebMessage(response);
                })
                .Load($"{baseUrl}/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.


            window.WaitForClose(); // Starts the application event loop
        }

        static void HandleMessage(PhotinoWindow window, string jsonMessage)
        {
            System.Diagnostics.Debug.WriteLine($"Received message:  {jsonMessage}");
            if (string.IsNullOrWhiteSpace(jsonMessage))
            {
                System.Diagnostics.Debug.WriteLine("Received empty message.");
                return;
            }
            Message message = JsonSerializer.Deserialize<Message>(jsonMessage, json_options);
            switch (message.Command)
            {
                case "exit":
                    Environment.Exit(0);
                    break;
                case "read_blocks_info":
                    ReadBlockInfo(window, message.Data);
                    break;
                case "convert":
                    Convert(window, message.Data);
                    break;  
                default:
                    System.Diagnostics.Debug.WriteLine($"Received unknown command: {message.Command}");
                    break;
            }
        }

        static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj, json_options);
        }

        static void SendMessage(PhotinoWindow window, string command, object data)
        {
            window.SendWebMessage(Serialize(new Message(command, Serialize(data))));
        }

        static void SendError(PhotinoWindow window, string error)
        {
            SendMessage(window, "error", error);
        }

        static void ReadBlockInfo(PhotinoWindow window, string url) {
            try
            {
                List<BlockModifier.BlockData> data = JsonSerializer.Deserialize<List<BlockModifier.BlockData>>(url, json_options);
                List<BlockModifier.BlockInfo> blocks_info = BlockModifier.GetBlocksInfo(data);
                SendMessage(window, "blocks_info", blocks_info);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading block info: {e.Message}");
                SendError(window, e.Message);
                return;
            }
        }


        static void Convert(PhotinoWindow window, string url)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Received messagio: {url}");
                BlockModifier.ConvertOptions convertOptions = JsonSerializer.Deserialize<BlockModifier.ConvertOptions>(url, json_options);
                List<BlockModifier.BlockData> convertedBlocks = BlockModifier.ConvertBlocks(convertOptions.BlockData, convertOptions.Conversions);
                SendMessage(window, "converted_blocks", convertedBlocks);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting blocks: {e.Message}");
                SendError(window, e.Message);

                return;
            }
        }
        /*
        static void OpenMap(PhotinoWindow window, string url)
        {
            try
            {
                url = JsonSerializer.Deserialize<string>(url, json_options);
                builder = new BlockBuilder(url);
                MapInfo mapInfo = builder.GetMapInfo();
                SendMessage(window, "map_info", mapInfo);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening map: {e.Message}");
                SendError(window, e.Message);
                return;
            }
        }*/

    }
}
