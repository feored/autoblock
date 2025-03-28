using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using GBX.NET;
using GBX.NET.Engines.GameData;
using GBX.NET.LZO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace blockswap;

public partial class MainWindow : Window
{
    List<CGameBlockItem> cur_blocks = new List<CGameBlockItem>();
    List<BlockData> cur_block_data = new List<BlockData> {
    new BlockData("Block1", 1),
    new BlockData("Block2", 2),
    };
    ObservableCollection<BlockData> CurrentBlocks;

    public MainWindow()
    {
        InitializeComponent();
        Gbx.LZO = new Lzo();
        var kako = new List<BlockData> {
    new BlockData("Block1", 1),
    new BlockData("Block2", 2),
    };
        CurrentBlocks = new ObservableCollection<BlockData>(kako);
    }

    private void bobo()
    {

    }

    private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        cur_blocks.Clear();
        cur_block_data.Clear();
        CurrentBlocks.Clear();

        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);
        var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var subFolderPath = Path.Combine(path, "Maniaplanet");
        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Pick Block (.gbx)",
            AllowMultiple = true,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("Blocks") { Patterns = new[] { "*.gbx" } },
            },
            SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync(subFolderPath)
        });

        foreach ( var file in files) {
            await using var stream = await file.OpenReadAsync();
            var block = Gbx.ParseNode<CGameItemModel>(stream);
            CGameBlockItem eME = (CGameBlockItem)block.EntityModelEdition!;
            cur_blocks.Add(eME);
            BlockData d = new BlockData(eME);
            CurrentBlocks.Add(d);
            System.Diagnostics.Debug.WriteLine(d.Archetype + ", " + d.CustomizedVariants);
        }
        System.Diagnostics.Debug.WriteLine(CurrentBlocks.Count);
        //CurrentBlocks = new ObservableCollection<BlockData>(cur_block_data);



        /* if (files.Count >= 1)
         {
             // Open reading stream from the first file.
             await using var stream = await files[0].OpenReadAsync();
             using var streamReader = new StreamReader(stream);
             // Reads all the content of file as a text.
             var fileContent = await streamReader.ReadToEndAsync();
         }*/
    }

}
