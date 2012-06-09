/**************************************************************
 This file is part of Kinect Sensor Architecture Development Project.

   Kinect Sensor Architecture Development Project is free software:
   you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   Kinect Sensor Architecture Development Project is distributed in
   the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Kinect Sensor Architecture Development Project.  If
   not, see <http://www.gnu.org/licenses/>.
**************************************************************/
/**************************************************************
The work was done in joint collaboration with Cisco Systems Inc.
Copyright © 2012, Cisco Systems, Inc. and UCLA
*************************************************************/

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
    public class MyAudioGesture : AudioGesture
    {

        const float CONFIDENCE_THRESHOLD = 0.7f;

        public MyAudioGesture(object d) : base(d)
        {
            List<string> myWords = new List<string> { "hello", "computer", "action" };
            foreach (string w in myWords)
            {
                this.AddWord(w);
            }
        }

        protected override void HypothesizedSpeech(RecognitionResult result)
        {
            this.ReportSpeechStatus("Hypothesized: " + result.Text + " " + result.Confidence);
        }
        protected override void RejectSpeech(RecognitionResult result)
        {
            string status = "Rejected: " + (result == null ? string.Empty : result.Text + " " + result.Confidence);
            this.ReportSpeechStatus(status);
        }
        protected override void RecognizedSpeech(RecognitionResult result)
        {
            if (result.Confidence < CONFIDENCE_THRESHOLD)
            {
                this.RejectSpeech(result);
                return;
            }

            string message = "";
            switch (result.Text.ToUpperInvariant())
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

            string status = "Recognized: " + result.Text + " " + result.Confidence + "\nResponse: " + message;
            this.ReportSpeechStatus(status);
            this.gestureDetected = true;
        }

        private void ReportSpeechStatus(string status)
        {
            Console.WriteLine(status);
        }

    }
}
