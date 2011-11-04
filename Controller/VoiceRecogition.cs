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

        public VoiceRecogition(Controller.MainController.Mapping mapping, MainController controller)
        {
            this.controller = controller;
            this.recogEng = new SpeechRecognitionEngine();
            recogEng.SetInputToDefaultAudioDevice();

            recogEng.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(speechRecog_success);
            recogEng.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(
                    speechRecog_failure);

            recogEng.LoadGrammar(CreateSampleGrammar(mapping));
            this.start();
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

        public void stop()
        {
            recogEng.RecognizeAsyncStop();
        }

        private Grammar CreateSampleGrammar(Controller.MainController.Mapping mapping)
        {
            GrammarBuilder grammarBuilder = new GrammarBuilder(mapping.prefix);
            if (mapping.useDictation)
            {
                grammarBuilder.AppendDictation();
            }
            else
            {
                Choices choices = new Choices();
                foreach (string choice in mapping.list)
                {
                    choices.Add(choice);
                }
                grammarBuilder.Append(choices);
            }
            Grammar g = new Grammar(grammarBuilder);
            g.Name = "Commands";
            return g;
        }
    }
}
