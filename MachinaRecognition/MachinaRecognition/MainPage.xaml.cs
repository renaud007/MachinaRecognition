using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MachinaRecognition
{
    public partial class MainPage : ContentPage
    {
        public ICommand ClickedCommand { get; set; }
        public MainPage()
        {
            ClickedCommand = new Command(() => { StartButtonClickedAsync(); });
            this.BindingContext = this; 
            InitializeComponent();
           
            NavigationPage.SetHasNavigationBar(this, false);
          
           
        }

         
 

        private void Button_Clicked(object sender, EventArgs e)
        {
           _= StartButtonClickedAsync();
        }

        private async Task StartButtonClickedAsync()
        {
            await CrossMedia.Current.Initialize();

            if ( !CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported )
            {
              await  DisplayAlert("erreur", ":( la camera n'est pas disponible", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = PhotoSize.Small
            }) ;

            if (file == null)
            {
                
                return;
            }

           await  Navigation.PushAsync(new scannerPage(file),false);
            
        }
    }
}
