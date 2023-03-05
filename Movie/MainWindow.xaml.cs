using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Movie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ImagePath { get; set; }
        public string Minute { get; set; }
        public string Description { get; set; }

        public dynamic Data { get; set; }
        public dynamic SingleData { get; set; }

        HttpClient httpClient = new HttpClient();

        public ObservableCollection<Film> Films { get; set; } = new ObservableCollection<Film>
        {
            new Film
            {
                Id=1,
                FilmName="FRIENDS",
                ImagePath="images/friends.jpg"
            },
            new Film
            {
                Id=2,
                FilmName="FOREVER",
                ImagePath="images/forever.jpg"
            },
            new Film
            {
                Id=3,
                FilmName="GIFTED",
                ImagePath="images/gifted.jpg"
            },
            new Film
            {
                Id=4,
                FilmName="IRON MAN",
                ImagePath="images/ironman.jpg"
            },
            new Film
            {
                Id=5,
                FilmName="STRANGER THINGS",
                ImagePath="images/strangerthings.jpg"
            },
            new Film
            {
                Id=6,
                FilmName="SHERLOCK HOLMES",
                ImagePath="images/sherlock.jpg"
            },
            new Film
            {
                Id=7,
                FilmName="CHERNOBYL",
                ImagePath="images/chernobyl.jpg"
            },
            new Film
            {
                Id=8,
                FilmName="THE GREEN MILE",
                ImagePath="images/greenmile.jpg"
            },
            new Film
            {
                Id=9,
                FilmName="11.22.63",
                ImagePath="images/112263.jpg"
            },
            new Film
            {
                Id=10,
                FilmName="WONDER",
                ImagePath="images/wonder.jpg"
            }
        };
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void searchTextbox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            searchTextbox.Text = String.Empty;
        }


        BlurEffect blur = new BlurEffect();
        int counter = 0;
        private void searchfilmBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Films)
            {
                counter++;
                if (item.FilmName.ToLower() == searchTextbox.Text.ToLower())
                {
                    blur.KernelType = KernelType.Gaussian;
                    blur.Radius = 100;
                    blur.RenderingBias = RenderingBias.Quality;
                    secondGrid.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    secondGridtxtBox.Text = item.FilmName;
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(item.ImagePath, UriKind.Relative);
                    bitmap.EndInit();
                    secondGridImage.Stretch = Stretch.Fill;
                    secondGridImage.Source = bitmap;
                    secondGrid.Visibility = Visibility.Visible;
                    mainGrid.Effect = blur;
                    mainGrid.IsEnabled = false;
                }
                else
                {
                    if (counter == Films.Count)
                    {
                        addBtn.Visibility = Visibility.Visible;
                        changeBtn.Visibility = Visibility.Hidden;
                        cancelBtn.FontWeight = FontWeights.DemiBold;
                        cancelBtn.FontSize = 15;
                        cancelBtn.Width = 80;
                        cancelBtn.Margin = new Thickness(248, 445, 62, 25);
                        blur.KernelType = KernelType.Gaussian;
                        blur.Radius = 100;
                        blur.RenderingBias = RenderingBias.Quality;
                        secondGrid.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        var name = searchTextbox.Text;
                        try
                        {
                            HttpResponseMessage response = new HttpResponseMessage();
                            response = httpClient.GetAsync($@"http://www.omdbapi.com/?apikey=3eb9dfa5&s={name}&plot=full").Result;
                            var str = response.Content.ReadAsStringAsync().Result;
                            Data = JsonConvert.DeserializeObject(str);
                            response = httpClient.GetAsync($@"http://www.omdbapi.com/?apikey=3eb9dfa5&t={Data.Search[0].Title}&plot=full").Result;
                            str = response.Content.ReadAsStringAsync().Result;
                            SingleData = JsonConvert.DeserializeObject(str);
                            secondGridImage.Stretch = Stretch.Fill;
                            secondGridImage.Source = SingleData.Poster;
                            secondGridImage.Source = SingleData.Poster;
                            Minute = SingleData.RunTime;
                            Title = SingleData.Title;
                            secondGridtxtBox.Text = Minute + "  " + Title;
                            secondGrid.Visibility = Visibility.Visible;
                            mainGrid.Effect = blur;
                            mainGrid.IsEnabled = false;
                            response = httpClient.GetAsync($@"http://www.omdbapi.com/?apikey=3eb9dfa5&t={Data.Search[1].Title}&plot=full").Result;
                            str = response.Content.ReadAsStringAsync().Result;
                            SingleData = JsonConvert.DeserializeObject(str);
                        }
                        catch (Exception)
                        {
                            searchTextbox.Text = "NOT FOUND";
                        }
                        counter = 0;
                    }
                }
            }
        }
        bool IsChecked = false;

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = listbox.SelectedItem as Film;
            changeBtn.Visibility = Visibility.Visible;
            blur.KernelType = KernelType.Gaussian;
            secondGridtxtBox.Text = item.FilmName;
            blur.Radius = 100;
            blur.RenderingBias = RenderingBias.Quality;
            secondGrid.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(item.ImagePath, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();
            secondGridImage.Stretch = Stretch.Fill;
            secondGridImage.Source = bitmap;
            secondGrid.Visibility = Visibility.Visible;
            mainGrid.Effect = blur;
            mainGrid.IsEnabled = false;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            secondGrid.Visibility = Visibility.Hidden;
            mainGrid.Effect = null;
            mainGrid.IsEnabled = true;
        }

        private void searchTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchfilmBtn_Click(sender, e);
            }
        }

        private void ItemOnPreviewMouseDown(
        object sender, MouseButtonEventArgs e)
        {
            ((ListBoxItem)sender).IsSelected = true;
        }
        private void changeBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = listbox.SelectedItem as Film;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == true)
            {
                string selectedFileName = dlg.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                secondGridImage.Source = bitmap;
                item.ImagePath = Path.GetFullPath(selectedFileName);
            }
        }

        private void addBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Film newFilm = new Film();
            var item1 = secondGridtxtBox.Text;

            newFilm.FilmName = secondGridtxtBox.Text;
            newFilm.ImagePath = secondGridImage.Source.ToString();
            foreach (var item in listbox.Items)
            {
                if (item is Film film)
                {
                    if (film.FilmName != item1.ToString())
                    {
                        IsChecked = true;
                        newFilm.Id = Films.Count + 1;
                    }
                    else
                    {
                        IsChecked = false;
                        break;
                    }
                }
            }
            if (IsChecked)
            {
                Films.Add(newFilm);
                MessageBox.Show("Film is successfully added!");
                listbox.ItemsSource = null;
                listbox.ItemsSource = Films;
            }
           
        }
    }
}
