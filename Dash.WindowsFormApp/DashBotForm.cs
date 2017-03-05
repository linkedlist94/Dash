namespace VoiceBot
{
    public partial class DashBotForm : MetroFramework.Forms.MetroForm
    {
        public DashBotForm()
        {
            new DashBot(this);
            InitializeComponent();
        }
    }
}