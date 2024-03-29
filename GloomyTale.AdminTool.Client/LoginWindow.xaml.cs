﻿using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GloomyTale.AdminTool.Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            // initialize api
            if (CommunicationServiceClient.Instance.Authenticate(ConfigurationManager.AppSettings["MasterAuthKey"]))
            {
                Console.WriteLine();
                //Logger.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
            }
            string Sha512(string inputString)
            {
                using (SHA512 hash = SHA512.Create())
                {
                    return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(inputString)).Select(item => item.ToString("x2")));
                }
            }
            if (AdminToolServiceClient.Instance.AuthenticateAdmin(AccBox.Text, Sha512(PassBox.Password)))
            {
                Hide();
                MainWindow mw = new MainWindow();
                mw.Show();
            }
            else
            {
                MessageBox.Show("Credentials invalid or not permitted to use the Service.", "Login failed.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
