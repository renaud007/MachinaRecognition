using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
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

            // Testons la connectivité
            var networkAccess = Connectivity.NetworkAccess;

            if (networkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("erreur", "Vous devez être connecté à internet", "OK");
                return;
            }



            //prise de photo
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
                //retourner à la page d'accueil
                return;
            }

           await  Navigation.PushAsync(new scannerPage(file),false);
            
        }
    }
}
