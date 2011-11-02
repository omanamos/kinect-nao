using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;

namespace Controller
{
    class VoiceRecogition
    {
        private SpeechRecognitionEngine recogEng;
        private MainController controller;

        public VoiceRecogition(string prefix, List<string> list, MainController controller)
        {
            this.controller = controller;
            this.recogEng = new SpeechRecognitionEngine();
            recogEng.SetInputToDefaultAudioDevice();

            recogEng.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(speechRecog_success);
            recogEng.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(
                    speechRecog_failure);

            recogEng.LoadGrammar(CreateSampleGrammar(prefix, list));
            this.start();
            Console.WriteLine("Initialized...");
        }

        private void speechRecog_success(object sender, SpeechRecognizedEventArgs args)
        {
            controller.processCommand(args.Result.Text);
            Console.WriteLine("Recognized: {0} - {1}", args.Result.Text, args.Result.Confidence);
        }

        private void speechRecog_failure(object sender, SpeechRecognitionRejectedEventArgs args)
        {
            Console.WriteLine("Failed: {0} - {1}", args.Result.Text, args.Result.Confidence);
        }

        public void start()
        {
            recogEng.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void exit()
        {
            recogEng.RecognizeAsyncStop();
        }

        private Grammar CreateSampleGrammar(string prefix, List<string> list)
        {
            Choices choices = new Choices();
            foreach (string choice in list) {
                choices.Add(choice);
            }
            GrammarBuilder grammarBuilder = new GrammarBuilder(prefix);
            grammarBuilder.Append(choices);
            Grammar g = new Grammar(grammarBuilder);
            g.Name = "Start Commands";
            return g;
        }
    }
}