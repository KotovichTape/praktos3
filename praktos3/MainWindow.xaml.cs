using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace praktos3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> ORIGINAL_PATHS = new List<string>();
        
        private bool ispause = true;
        
        private int index = 0;
        
        private bool isrepeating = false;
        
        private bool israndom = false;
        
        private List<string> _paths = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (ispause)
            {
                play.Content = "играть";
                media.Pause();
                ispause = false;
            }
            else
            {
                play.Content = "пауза";
                media.Play();
                ispause = true;
            }
        }
        private void Start()
        {
            if (ORIGINAL_PATHS.Count == 0)
            {
                MessageBox.Show("Не выбрана папка!");
            }
            else
            {
                song_name.Content = ORIGINAL_PATHS[index].Split("\\").Last().Split(".mp3")[0];
                media.Source = new Uri(ORIGINAL_PATHS[index]);
                media.Play();
                media.Volume = 0.7;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button original = (Button)e.OriginalSource;
            if (original.Name == "choose")
            {
                ORIGINAL_PATHS.Clear();
                var dialog = new CommonOpenFileDialog()
                {
                    IsFolderPicker = true
                };
                var res = dialog.ShowDialog();
                if (res == CommonFileDialogResult.Ok)
                {
                    string[] files = Directory.GetFiles(dialog.FileName);
                    List<string> sorted = new List<string>();
                    foreach (string file in files)
                    {
                        if (file.Contains("mp3"))
                        {
                            ORIGINAL_PATHS.Add(file);
                            var a = file.Split("\\");
                            sorted.Add(a[a.Length - 1]);
                        }
                    }
                    sorted.Sort();
                    songs.ItemsSource = sorted;
                    Start();
                }
                else
                {
                    MessageBox.Show("Вы выбрали неверную папку!");
                }
            }
            else if (original.Name == "prev")
            {
                if (ORIGINAL_PATHS.Count == 0)
                {
                    MessageBox.Show("Не выбрана папка!");
                    return;
                }
                if (index > 0)
                {
                    media.Stop();
                    index--;
                    Start();
                }
                else
                {
                    media.Stop();
                    index = ORIGINAL_PATHS.IndexOf(ORIGINAL_PATHS.Last());
                    Start();
                }
            }
            else if (original.Name == "next")
            {
                if (ORIGINAL_PATHS.Count == 0)
                {
                    MessageBox.Show("Не выбрана папка!");
                    return;
                }
                if (index < ORIGINAL_PATHS.Count - 1)
                {
                    media.Stop();
                    index++;
                    Start();
                }
                else if (index > ORIGINAL_PATHS.Count - 1)
                {
                    media.Stop();
                    index = 0;
                    Start();
                }
            }
            else if (original.Name == "repeat")
            {
                if (!isrepeating)
                {
                    isrepeating = true;
                }
                else
                {
                    isrepeating = false;
                }
            }
            else if (original.Name == "random")
            {
                if (ORIGINAL_PATHS.Count == 0)
                {
                    MessageBox.Show("Не выбрана папка!");
                    return;
                }
                if (!israndom)
                {
                    israndom = true;
                    Random RND = new Random();
                    foreach (string path in ORIGINAL_PATHS)
                    {
                        _paths.Add(path);
                    }
                    for (int i = 0; i < ORIGINAL_PATHS.Count; i++)
                    {
                        int j = RND.Next(i + 1);
                        var temp = ORIGINAL_PATHS[j];
                        ORIGINAL_PATHS[j] = ORIGINAL_PATHS[i];
                        ORIGINAL_PATHS[i] = temp;
                    }
                    index = 0;
                    media.Stop();
                    Start();
                }
                else
                {
                    israndom = false;
                    media.Stop();
                    ORIGINAL_PATHS.Clear();
                    foreach (string path in _paths)
                    {
                        ORIGINAL_PATHS.Add(path);
                    }
                    _paths.Clear();
                    index = 0;
                    Start();
                }
            }
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (!isrepeating)
            {
                index++;
                Start();
            }
            else
            {
                Start();
            }
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (media.NaturalDuration.TimeSpan.Seconds < 10)
            {
                total.Content = media.NaturalDuration.TimeSpan.Minutes + ":" + "0" + media.NaturalDuration.TimeSpan.Seconds;
            }
            else
            {
                total.Content = media.NaturalDuration.TimeSpan.Minutes + ":" + media.NaturalDuration.TimeSpan.Seconds;
            }
        }

        private void songs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(songs.SelectedItem != null)
            {
                int findIndex = 0;
                foreach (var item in ORIGINAL_PATHS)
                {
                    if (item.Contains(songs.SelectedItem.ToString()))
                    {
                        findIndex = ORIGINAL_PATHS.IndexOf(item);
                    }
                }
                songs.SelectedItem = null;
                index = findIndex;
                Start();
            }
        }
    }
}
