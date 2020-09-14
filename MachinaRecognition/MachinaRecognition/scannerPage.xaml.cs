using MachinaRecognition.Models;
using MachinaRecognition.Services;
using Plugin.Media.Abstractions;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MachinaRecognition
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class scannerPage : ContentPage
    {
        FaceDetectResult faceDetectResult = null;
        bool processing = true;
        SpeechOptions speechOptions = null;
        public scannerPage(MediaFile file)
        {
            InitializeComponent();
            faceImage.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStreamWithImageRotatedForExternalStorage();
                return stream;
            });

            var data = file.GetStreamWithImageRotatedForExternalStorage();

            _ = starDetection(data);

            _ = LaserAnimationWithSoundAndDisplayResults();

            statusLabel.Text = "Analyse en cours...";

            _ = InitSpeak();
        }





        private async Task LaserAnimationWithSoundAndDisplayResults()
        {
            laserImage.Opacity = 0;
            await Task.Delay(500);
            await laserImage.FadeTo(1, 500);

            PlaySound("scan.wav");
            await laserImage.TranslateTo(0, 360, 1800);
            double y = 0;
            while (processing)
            {
                PlayCurrentSound();
                await laserImage.TranslateTo(0, y, 1800);
                y = (y == 0) ? 360 : 0;
            }

            laserImage.IsVisible = false;
            PlaySound("result.wav");
            if (faceDetectResult == null)
            {
                // Cas d'erreur
                await DisplayAlert("ERREUR", "l'analyse n'a pas fonctionnée ou absence de visage ", "ok");
                await Navigation.PopAsync(); 
            }
            else
            {
                 ResultsSpeech();
                DisplayResults();
               
            }
        }

        private void PlaySound(string soundName)
        {
            ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Load(GetStreamFromFile(soundName));
            player.Play();
        }

        private async Task starDetection(Stream data)
        {

            faceDetectResult = await CognitiveService.FaceDetect(data);
            processing = false;



        }

        private async void DisplayResults()
        {
            if (faceDetectResult == null)
            {
                return;
            }

            statusLabel.Text = "Analyse terminée";

            // On a récupéré les infos du visage
            ageLabel.Text = faceDetectResult.faceAttributes.age.ToString();
            genderLabel.Text = faceDetectResult.faceAttributes.gender.Substring(0, 1).ToUpper();
            infoLayout.IsVisible = true;
            continueButton.Opacity = 1;

        }
        private void PlayCurrentSound()
        {
            ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Stop();
            player.Play();
        }

        private Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;

            var names = assembly.GetManifestResourceNames();
            Console.WriteLine("RESOURCES NAMES : " + String.Join(", ", names)); //permet de connaitre les emb

            var stream = assembly.GetManifestResourceStream("MachinaRecognition." + filename);
            return stream;
        }

        private async Task Speak(string text)
        {
            if (speechOptions == null)
            {
                await InitSpeak();
            }
            await TextToSpeech.SpeakAsync(text, speechOptions);
        }

        private async Task InitSpeak()
        {
            var locales = await TextToSpeech.GetLocalesAsync();

            // Grab the first locale
            var locale = locales.Where(o => o.Language.ToLower() == "fr").FirstOrDefault();

            speechOptions = new SpeechOptions()
            {
                Volume = 1f,
                Pitch = 0.2f,
                Locale = locale
            };
        }

        private async Task ResultsSpeech()
        {
            if (faceDetectResult == null)
            {
                await Speak("Humain non détecté ");
                return;
            }


            if (faceDetectResult.faceAttributes.gender.ToLower() == "male")
            {
                await Speak("Humain détecté, Sexe masculin," + " âge " + faceDetectResult.faceAttributes.age.ToString() + " ans");
            }
            else
            {
                await Speak("Humain détecté, Sexe féminin," + " âge " + faceDetectResult.faceAttributes.age.ToString() + " ans");
            }

        }

        private void ContinueButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}