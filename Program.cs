using System;
using System.IO;
using System.Linq;
using Google.Cloud.Speech.V1;
using Google.Cloud.Storage.V1;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            //if(args.Any(x => x == "l"))
            //{
            //    SyncRecognizeModelSelection(args.Where(x => x.))
            //}
            AsyncRecognizeGcs("gs://okatec_audio/12.wav");
            //SyncRecognizeModelSelection(@"C:\Users\takashi-o\Videos\mwbv_J_202008_01_r240P.mp4", "default");
        }

        /// <summary>
        /// GCPストレージ上のファイルを変換
        /// </summary>
        /// <param name="storageUri">ストレージのURL</param>
        /// <returns></returns>
        static object AsyncRecognizeGcs(string storageUri)
        {
            var speech = SpeechClient.Create();
            var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
            {
                SampleRateHertz= 44100,
                LanguageCode = "ja",
            }, RecognitionAudio.FromStorageUri(storageUri));
            longOperation = longOperation.PollUntilCompleted();
            var response = longOperation.Result;
            string now = DateTime.Now.Ticks.ToString();
            StreamWriter streamw = File.CreateText($"{now}.txt");
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    streamw.WriteLine(alternative.Transcript);
                    Console.WriteLine(alternative.Transcript);
                }
            }
            streamw.Close();
            return 0;
        }

        /// <summary>
        /// ローカルのファイルを変換
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        static object SyncRecognizeModelSelection(string filePath, string model)
        {
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 16000,
                LanguageCode = "ja",
                // The `model` value must be one of the following:
                // "video", "phone_call", "command_and_search", "default"
                Model = model
            }, RecognitionAudio.FromFile(filePath));
            string now = DateTime.Now.Ticks.ToString();
            StreamWriter streamw = File.CreateText($"{now}.txt");
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    streamw.WriteLine(alternative.Transcript);
                    Console.WriteLine(alternative.Transcript);
                }
            }
            return 0;
        }
    }
}
