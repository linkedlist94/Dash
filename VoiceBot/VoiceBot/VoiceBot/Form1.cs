using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;
using MetroFramework.Forms;
using static MetroFramework.Drawing.MetroPaint.ForeColor;
using System.Xml;
using System.IO;

namespace VoiceBot
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {

        SpeechSynthesizer speech = new SpeechSynthesizer();
        Boolean wake = false; 
        Choices list = new Choices();


        String temp;
        String condition;
        String high;
        String low;

        
        
        public Form1()
        {


            SpeechRecognitionEngine rec = new SpeechRecognitionEngine();




            list.Add(File.ReadAllLines(@"C:\Users\Joseph\Desktop\VBot\commands.txt")); 
            

            Grammar grammar = new Grammar(new GrammarBuilder(list));

            try
            {
                rec.RequestRecognizerUpdate();
                rec.LoadGrammar(grammar);
                rec.SpeechRecognized += rec_SpeechRecognized;
                rec.SetInputToDefaultAudioDevice();
                rec.RecognizeAsync(RecognizeMode.Multiple);

            }

            catch(Exception)
            {
                throw;
            }

            speech.SelectVoiceByHints(VoiceGender.Male);


           

            InitializeComponent();

            }   


        public String GetWeather(String input)
        {
            String query = String.Format("https://query.yahooapis.com/v1/public/yql?q=select * from weather.forecast where woeid in (select woeid from geo.places(1) where text='toronto, ontario')&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            XmlDocument wData = new XmlDocument();

            try
            {
                wData.Load(query);
            }

            catch
            {
                MessageBox.Show("No internet connection!");
                return ("you are not connected to the internet");
            }

            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
            XmlNodeList nodes = wData.SelectNodes("query/results/channel");
            try
            {
                int rawTemp = int.Parse(channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value);
                temp = ((rawTemp - 32) * 5 / 9 + ""); 
                condition = ((rawTemp - 32) * 5 / 9 + "");
                int highTemp = int.Parse(channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["high"].Value);
                high = ((highTemp - 32) * 5 / 9 + "");
                int lowTemp = int.Parse(channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["low"].Value);
                low = ((lowTemp - 32) * 5 / 9 + "");                                                            
                if (input == "temp")
                {
                    return temp;
                }
                if (input == "high")
                {
                    return high;
                }
                if (input == "low")
                {
                    return low;
                }
                if (input == "cond")
                {
                    return condition;
                }
            }
            catch
            {
                return "Error Reciving data";
            }
            return "error";
        }

        public enum PromptBreak
        {

        }

        public static void killProg(String s)
        {
            System.Diagnostics.Process[] procs = null;

            try
            {
                procs = Process.GetProcessesByName(s);
                Process prog = procs[0]; 

                if(!prog.HasExited)
                {
                    prog.Kill();
                }

            }
            
            finally
            {
                if(procs != null)
                {
                    foreach(Process p in procs)
                    {
                        p.Dispose();
                    }
                }
            }
            procs = null;
        }


        String[] greetings = new String[4] {"hello","how may i assist you","hi", "yes?" };  

        public String greetings_action()
        {
            Random r = new Random();
            return greetings[r.Next(4)];
        }
        
        

        public void restart()
        {
            Process.Start(@"C:\Users\Joseph\Desktop\VBot\VoiceBot");
            Environment.Exit(0);
        }
        public void say(String h)
        {
            speech.Speak(h);
           
        }



        //Commands

        private void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String r = e.Result.Text;

            if (r == "hey dash")
            {
                wake = true;
                say("Hello Kevin, how may I assist you today?");
               // Label.Text = "Status: Awake";
            }
            if (r == "sleep")
            {
                
                wake = false;
               // Label.Text = "Status: Sleep";
                say("Goodbye Kevin");
                this.Close(); 
               
            }

            if (wake == true)
            {
                if (r == "last" || r == "last song")
                {
                    SendKeys.Send("^{LEFT}");
                }
                if (r == "next" || r == "next song")
                {
                    SendKeys.Send("^{RIGHT}");
                }
                if (r == "close itunes")
                {
                    say("okay, i hope you enjoyed your music");
                    killProg("itunes");
                }
                if (r == "open itunes")
                {
                    say("okay, im opening itunes");
                    Process.Start(@"C:\Program Files\iTunes\iTunes.exe");
                }
                if(r == "play" || r == "pause")
                {
                    SendKeys.Send(" ");
                }
                if (r == "minimize")
                {
                    this.WindowState = FormWindowState.Minimized; 
                }
                if (r == "maximize")
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                if (r == "normalize")
                {
                    this.WindowState = FormWindowState.Normal;
                }
                if (r == "what is your name")
                {
                    say("my name is Dash, Deluxe Artificial Speech Hybrid");
                }
                if(r == "open visual studio")
                {
                    say("okay, opening visual studio");
                    Process.Start(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\WDExpress.exe");
                }
                if(r == "close visual studio")
                {
                    say("not a problem, closing visual studio");
                    killProg("WDExpress");
                }
                if(r == "open curse voice")
                {
                    Process.Start(@"C:\Users\Joseph\AppData\Roaming\Curse Client\Bin\Curse.exe");
                }
                if(r == "close curse")
                {
                    killProg("Curse");
                }
                if(r == "thank you")
                {
                    say("you are welcome");
                }

                if(r == "what is the weather today")
                {
                    say("The weather forcast is " + GetWeather("cond") + " today.");
                }

                if (r == "what is the temperature")
                {
                    say("It is " + GetWeather("temp") + " degrees celcius");
                }

                if (r == "what is the high for today")
                {
                    say("The high for today is " + GetWeather("high") + " .");
                }

                if(r == "what is the low for today")
                {
                    say("The low for today is " + GetWeather("low") + " .");
                }

                if(r == "give me a detailed weather report" || r == "give me the full weather report")
                {
                    say("The forecast for today is " + GetWeather("cond") + " and it will be" + GetWeather("temp") +
                        "degrees fahrenheit, Also note the high for today is " + GetWeather("high") +
                        " and the low for today is " + GetWeather("low"));
                }

                if (r == "restart" || r == "update")
                {
                    restart();
                }

                if (r == "open word" )
                {
                    Process.Start(@"C:\Program Files (x86)\Microsoft Office\root\Office16\winword.exe");
                    say("okay, i am opening word now");
                }

                if (r == "close word")
                {
                    killProg("winword");
                    say("closing word, make sure you have saved your documents");

                }

                if (r == "open excel")
                {
                    Process.Start(@"C:\Program Files (x86)\Microsoft Office\root\Office16\excel.exe");
                    say("okay, opening excel now");
                }

                if (r == "close excel")
                {
                    killProg("excel");
                    say("closing excel, make sure you have saved your spreadsheets");
                }

                if (r == "open league of legends" || r == "open league")
                {
                    Process.Start(@"C:\Riot Games\League of Legends\LeagueClient.exe");
                    say("opening league of legends, prepare yourself summoner!");
                }

                if (r == "close league")
                {
                    killProg("LeagueClient");
                    say("okay closing league, i hope you did well");
                }

                if (r == "hi")
                {
                    say(greetings_action());
                }

                if (r == "tell me a joke")
                {
                    say("An electron and a neutron walk into a bar, the neutron orders a drink and the bartender says, for you, no charge!");
                }

                if (r == "how are you")
                {
                    say("Excellent, how are you?");
                }

                if (r == "what time is it")
                {
                    say("The time is" + DateTime.Now.ToString("h:mm tt"));
                }

                if (r == "what day is it" || r == "what is today")
                {
                    say("Today is " + DateTime.Now.ToString("M/d/yyyy"));
                }
                if (r == "open google" || r == "open chrome")
                {
                    Process.Start("http://google.com");
                }
                if (r == "close google" || r == "close chrome")
                {
                    killProg("google");
                }
            }
        }




        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            
        }
    }




      
    }


