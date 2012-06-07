using System;
using System.IO;
using System.Linq;
using Microsoft.Speech.Recognition;
using NuiDeviceFramework.Gestures;
using NuiDeviceFramework.reflection;
using Microsoft.Speech.AudioFormat;
using System.Collections.Generic;

namespace NuiDeviceFramework.gestures.implementations
{
    public class AudioGesture : Gesture
    {
        private SpeechRecognitionEngine sre;
        private const int WaveImageWidth = 500;
        private const int WaveImageHeight = 100;
        private EnergyCalculatingPassThroughStream stream;

        private Choices wordsToRecognize = new Choices();
        private List<string> words;

        public AudioGesture(object d) : base(d)
        {
            wordsToRecognize = new Choices();
            words = new List<string>();
        }

        public void AddWord(string word)
        {
            wordsToRecognize.Add(word);
            words.Add(word);
        }

        private SpeechRecognitionEngine CreateSpeechRecognizer()
        {
            RecognizerInfo ri = GetRecognizer();
            if (ri == null)
            {
                Console.WriteLine(
                    @"There was a problem initializing Speech Recognition.
Ensure you have the Microsoft Speech SDK installed.");
                return null;
            }

            SpeechRecognitionEngine sre;
            try
            {
                sre = new SpeechRecognitionEngine(ri.Id);
            }
            catch
            {
                Console.WriteLine(
                    @"There was a problem initializing Speech Recognition.
Ensure you have the Microsoft Speech SDK installed.");
                return null;
            }

            var gb = new GrammarBuilder{Culture = ri.Culture};
            gb.Append(wordsToRecognize);

            var g = new Grammar(gb);

            sre.LoadGrammar(g);
            sre.SpeechRecognized += this.SreSpeechRecognized;
            sre.SpeechHypothesized += this.SreSpeechHypothesized;
            sre.SpeechRecognitionRejected += this.SreSpeechRecognitionRejected;

            return sre;
        }

        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            this.RejectSpeech(e.Result);
        }

        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            this.ReportSpeechStatus("Hypothesized: " + e.Result.Text + " " + e.Result.Confidence);
        }

        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < 0.7)
            {
                this.RejectSpeech(e.Result);
                return;
            }

            string message = "";
            switch (e.Result.Text.ToUpperInvariant())
            {
                case "HELLO":
                    message += "Hello! I am your computer.";
                    break;
                case "COMPUTER":
                    message += "Computer here. What would you like me to do?";
                    break;
                case "ACTION":
                    message += "What action shall I perform?";
                    break;
                default:
                    message += "I recognized your speech but I'm not sure what you mean.";
                    break;
            }

            string status = "Recognized: " + e.Result.Text + " " + e.Result.Confidence + "\nResponse: " + message;
            this.ReportSpeechStatus(status);
            this.gestureDetected = true;
        }

        private void RejectSpeech(RecognitionResult result)
        {
            string status = "Rejected: " + (result == null ? string.Empty : result.Text + " " + result.Confidence);
            this.ReportSpeechStatus(status);
        }

        private void ReportSpeechStatus(string status)
        {
            Console.WriteLine(status);
        }

        private static RecognizerInfo GetRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                return "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        public override void Start()
        {
            sre = CreateSpeechRecognizer();
            object s = ReflectionUtilities.InvokeProperty(device, "AudioStream");
            if (s == null)
            {
                Console.WriteLine("Unable to generate audio stream.");
                return;
            }

            Console.WriteLine("Welcome to the Audio Gesture recognition module.");
            Console.WriteLine("I can recognize the following words:");
            foreach (string w in words)
            {
                Console.WriteLine("\t{0}", w);
            }

            this.stream = new EnergyCalculatingPassThroughStream((Stream)s);
            this.sre.SetInputToAudioStream(
                this.stream, new Microsoft.Speech.AudioFormat.SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            this.sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private class EnergyCalculatingPassThroughStream : Stream
        {
            private const int SamplesPerPixel = 10;

            private readonly double[] energy = new double[WaveImageWidth];
            private readonly object syncRoot = new object();
            private readonly Stream baseStream;

            private int index;
            private int sampleCount;
            private double avgSample;

            public EnergyCalculatingPassThroughStream(Stream stream)
            {
                this.baseStream = stream;
            }

            public override long Length
            {
                get { return this.baseStream.Length; }
            }

            public override long Position
            {
                get { return this.baseStream.Position; }
                set { this.baseStream.Position = value; }
            }

            public override bool CanRead
            {
                get { return this.baseStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return this.baseStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return this.baseStream.CanWrite; }
            }

            public override void Flush()
            {
                this.baseStream.Flush();
            }

            public void GetEnergy(double[] energyBuffer)
            {
                lock (this.syncRoot)
                {
                    int energyIndex = this.index;
                    for (int i = 0; i < this.energy.Length; i++)
                    {
                        energyBuffer[i] = this.energy[energyIndex];
                        energyIndex++;
                        if (energyIndex >= this.energy.Length)
                        {
                            energyIndex = 0;
                        }
                    }
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int retVal = this.baseStream.Read(buffer, offset, count);
                const double A = 0.3;
                lock (this.syncRoot)
                {
                    for (int i = 0; i < retVal; i += 2)
                    {
                        short sample = BitConverter.ToInt16(buffer, i + offset);
                        this.avgSample += sample * sample;
                        this.sampleCount++;

                        if (this.sampleCount == SamplesPerPixel)
                        {
                            this.avgSample /= SamplesPerPixel;

                            this.energy[this.index] = .2 + ((this.avgSample * 11) / (int.MaxValue / 2));
                            this.energy[this.index] = this.energy[this.index] > 10 ? 10 : this.energy[this.index];

                            if (this.index > 0)
                            {
                                this.energy[this.index] = (this.energy[this.index] * A) + ((1 - A) * this.energy[this.index - 1]);
                            }

                            this.index++;
                            if (this.index >= this.energy.Length)
                            {
                                this.index = 0;
                            }

                            this.avgSample = 0;
                            this.sampleCount = 0;
                        }
                    }
                }

                return retVal;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.baseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                this.baseStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.baseStream.Write(buffer, offset, count);
            }
        }
    }
}
