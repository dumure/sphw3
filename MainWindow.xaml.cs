using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sphw3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string sourceText = string.Empty;
        string filePath = string.Empty;
        string encryptKey = string.Empty;
        CancellationTokenSource cts = new CancellationTokenSource();
        public MainWindow()
        {
            InitializeComponent();
            textBox.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                textBox.Text = fileDialog.FileName;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrEmpty(textBox.Text) || string.IsNullOrEmpty(passwordBox.Password)))
            {
                sourceText = File.ReadAllText(textBox.Text);
                filePath = textBox.Text;
                encryptKey = textBox.Text;
                progressBar.Value = 0;
                progressBar.Maximum = sourceText.Length;

                ThreadPool.QueueUserWorkItem(o => { Encrypt(cts.Token); });
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }

        private void Encrypt(CancellationToken token)
        {
            char[] encryptedText = sourceText.ToCharArray();
            for (int i = 0; i < sourceText.Length; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Dispatcher.Invoke(() =>
                    {
                        progressBar.Value = 0;
                    });
                    File.WriteAllText(filePath, sourceText);
                    return;
                }
                encryptedText[i] = (char)(sourceText[i] ^ encryptKey[i % encryptKey.Length]);
                string encryptedString = string.Empty;
                for (int j = 0; j < encryptedText.Length; j++)
                {
                    encryptedString += encryptedText[j];
                }
                File.WriteAllText(filePath, encryptedString);
                Dispatcher.Invoke(() =>
                {
                    progressBar.Value++;
                });
                Thread.Sleep(150);
            }
        }
    }
}