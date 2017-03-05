using System;
using System.Diagnostics;
using System.IO;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows.Forms;
using System.Xml;

namespace VoiceBot
{
    public class DashBot
    {
        private SpeechSynthesizer SpeechSynthesizer { get; }
        private SpeechRecognitionEngine SpeechRecognitionEngine { get; }
        private readonly DashBotForm _dashBotForm;
        private bool _status = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashBotForm"></param>
        public DashBot(DashBotForm dashBotForm)
        {
            SpeechSynthesizer = new SpeechSynthesizer();
            SpeechRecognitionEngine = new SpeechRecognitionEngine();
            _dashBotForm = dashBotForm;

            var choices = new Choices(File.ReadAllLines(@"..\..\commands.txt"));
            var grammarBuilder = new GrammarBuilder(choices);
            var grammar = new Grammar(grammarBuilder);

            SpeechSynthesizer.SelectVoiceByHints(VoiceGender.Male);

            SpeechRecognitionEngine.RequestRecognizerUpdate();
            SpeechRecognitionEngine.LoadGrammar(grammar);
            SpeechRecognitionEngine.SpeechRecognized += SpeechRecognized;
            SpeechRecognitionEngine.SetInputToDefaultAudioDevice();
            SpeechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void Say(string message)
        {
            SpeechSynthesizer.Speak(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Greet()
        {
            return new string[] { "hello", "how may i assist you", "hi", "yes?" }[new Random().Next(4)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetWeather(string input)
        {
            var query =
                string.Format(
                    "https://query.yahooapis.com/v1/public/yql?q=select * from weather.forecast where woeid in (select woeid from geo.places(1) where text='toronto, ontario')&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            var wData = new XmlDocument();

            try
            {
                wData.Load(query);
            }
            catch
            {
                return "you are not connected to the internet";
            }

            var manager = new XmlNamespaceManager(wData.NameTable);
            manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            var channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
            var nodes = wData.SelectNodes("query/results/channel");

            try
            {
                var rawTemp =
                    int.Parse(
                        channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes[
                            "temp"].Value);
                var temp = ((rawTemp - 32) * 5 / 9 + "");
                var condition = ((rawTemp - 32) * 5 / 9 + "");
                var highTemp =
                    int.Parse(
                        channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes[
                            "high"].Value);
                var high = ((highTemp - 32) * 5 / 9 + "");
                var lowTemp =
                    int.Parse(
                        channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["low"
                        ].Value);
                var low = ((lowTemp - 32) * 5 / 9 + "");

                switch (input)
                {
                    case "temp":
                        return temp;
                    case "high":
                        return high;
                    case "low":
                        return low;
                    case "cond":
                        return condition;
                }
            }
            catch
            {
                return "Error receiving data";
            }

            return "error";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        private void KillProg(string s)
        {
            System.Diagnostics.Process[] procs = null;

            try
            {
                procs = Process.GetProcessesByName(s);
                var prog = procs[0];

                if (!prog.HasExited)
                {
                    prog.Kill();
                }
            }
            finally
            {
                if (procs != null)
                {
                    foreach (var p in procs)
                    {
                        p.Dispose();
                    }
                }
            }

            procs = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Restart()
        {
            Process.Start(@"C:\Users\Joseph\Desktop\VBot\VoiceBot");
            Environment.Exit(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var speechText = e.Result.Text;

            switch (speechText)
            {
                case "hey dash":
                    _status = true;
                    Say("Hello Kevin, how may I assist you today?");
                    break;
                case "sleep":
                    _status = false;
                    Say("Goodbye Kevin");
                    _dashBotForm.Close();
                    break;
                default:
                    break;
            }

            if (_status != true) return;

            switch (speechText)
            {
                case "last":
                case "last song":
                    SendKeys.Send("^{LEFT}");
                    break;
                case "next":
                case "next song":
                    SendKeys.Send("^{RIGHT}");
                    break;
                case "close itunes":
                    Say("okay, i hope you enjoyed your music");
                    KillProg("itunes");
                    break;
                case "open itunes":
                    Say("okay, im opening itunes");
                    Process.Start(@"C:\Program Files\iTunes\iTunes.exe");
                    break;
                case "play":
                case "pause":
                    SendKeys.Send(" ");
                    break;
                case "minimize":
                    _dashBotForm.WindowState = FormWindowState.Minimized;
                    break;
                case "maximize":
                    _dashBotForm.WindowState = FormWindowState.Maximized;
                    break;
                case "normalize":
                    _dashBotForm.WindowState = FormWindowState.Normal;
                    break;
                case "what is your name":
                    Say("my name is Dash, Deluxe Artificial Speech Hybrid");
                    break;
                case "open visual studio":
                    Say("okay, opening visual studio");
                    Process.Start(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\WDExpress.exe");
                    break;
                case "close visual studio":
                    Say("not a problem, closing visual studio");
                    KillProg("WDExpress");
                    break;
                case "open curse voice":
                    Process.Start(@"C:\Users\Joseph\AppData\Roaming\Curse Client\Bin\Curse.exe");
                    break;
                case "close curse":
                    KillProg("Curse");
                    break;
                case "thank you":
                    Say("you are welcome");
                    break;
                case "what is the weather today":
                    Say("The weather forcast is " + GetWeather("cond") + " today.");
                    break;
                case "what is the temperature":
                    Say("It is " + GetWeather("temp") + " degrees celcius");
                    break;
                case "what is the high for today":
                    Say("The high for today is " + GetWeather("high") + " .");
                    break;
                case "what is the low for today":
                    Say("The low for today is " + GetWeather("low") + " .");
                    break;
                case "give me a detailed weather report":
                case "give me the full weather report":
                    Say("The forecast for today is " + GetWeather("cond") + " and it will be" + GetWeather("temp") +
                        "degrees fahrenheit, Also note the high for today is " + GetWeather("high") +
                        " and the low for today is " + GetWeather("low"));
                    break;
                case "restart":
                case "update":
                    Restart();
                    break;
                case "open word":
                    Process.Start(@"C:\Program Files (x86)\Microsoft Office\root\Office16\winword.exe");
                    Say("okay, i am opening word now");
                    break;
                case "close word":
                    KillProg("winword");
                    Say("closing word, make sure you have saved your documents");
                    break;
                case "open excel":
                    Process.Start(@"C:\Program Files (x86)\Microsoft Office\root\Office16\excel.exe");
                    Say("okay, opening excel now");
                    break;
                case "close excel":
                    KillProg("excel");
                    Say("closing excel, make sure you have saved your spreadsheets");
                    break;
                case "open league of legends":
                case "open league":
                    Process.Start(@"C:\Riot Games\League of Legends\LeagueClient.exe");
                    Say("opening league of legends, prepare yourself summoner!");
                    break;
                case "close league":
                    KillProg("LeagueClient");
                    Say("okay closing league, i hope you did well");
                    break;
                case "hi":
                    Say(Greet());
                    break;
                case "tell me a joke":
                    Say(
                        "An electron and a neutron walk into a bar, the neutron orders a drink and the bartender says, for you, no charge!");
                    break;
                case "how are you":
                    Say("Excellent, how are you?");
                    break;
                case "what time is it":
                    Say("The time is" + DateTime.Now.ToString("h:mm tt"));
                    break;
                case "what day is it":
                case "what is today":
                    Say("Today is " + DateTime.Now.ToString("M/d/yyyy"));
                    break;
                case "open google":
                case "open chrome":
                    Process.Start("http://google.com");
                    break;
                case "close google":
                case "close chrome":
                    KillProg("google");
                    break;
                default:
                    break;
            }
        }
    }
}
