using Microsoft.Win32;
using SpaceImagesAI.Classes;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace SpaceImagesAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image file (*.png,*jpg,*.jpeg) | *.png;*jpg;*.jpeg";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(fileName));

                MakePredictionAsync(fileName);
            }
        }
        
        /*
         *Make prediction using customvision.ai
         */
        private async void MakePredictionAsync(string fileName)
        {
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/3ed549bd-6306-41b7-bd7b-5aef6f75787a/image?iterationId=77e09f85-86a8-4348-bc99-c8a120f7eb00";
            string prediction_key = "2a3f3ced7a5440b8addc5533a83cc76f";
            string content_type = "application/octet-stream";

            var file = File.ReadAllBytes(fileName);

            using (HttpClient client = new HttpClient())
            {
                //set the custom header requested by the prediction API
                client.DefaultRequestHeaders.Add("Prediction-Key", prediction_key);

                using (var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                    //optaining the response as HttpResponseMessage
                    var response = await client.PostAsync(url, content);
                    
                    var responseString = await response.Content.ReadAsStringAsync();

                    List<Prediction> predictions = (JsonConvert.DeserializeObject<CustomVision>(responseString)).predictions;

                    PredictionsListView.ItemsSource = predictions;
                }

            }
        }
    }
}
