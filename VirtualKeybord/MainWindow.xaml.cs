using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace VirtualKeybord {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private const int ButtonSize = 60;

        private const int WsExNoactivate = 0x08000000;
        private const int GwlExstyle = -20;
        public const int Enter = 0x0D; //Enter key code

        public const int KEYEVENTF_KEYDOWN = 0x0000; // New definition
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag

        public MainWindow() {
            InitializeComponent();

            for (var i = 0; i <= 9; i++) {
                wrapPanel.Children.Add(new Button {Content = i.ToString(), Width = ButtonSize, Height = ButtonSize});
            }
            wrapPanel.Children.Add(new Button { Content = "Enter", Width = ButtonSize, Height = ButtonSize });
            foreach (Button button in wrapPanel.Children) {
                button.Click += Button_Click;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            if (button == null) {
                return;
            }

            var chars = button.Content.ToString().ToCharArray();
            if (chars.Length == 0) {
                return;
            }
            if (button.Content.ToString() == "Enter")
            {
                keybd_event(Enter, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                keybd_event(Enter, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                return;
            }

            keybd_event((byte) chars[0], 0, 0, UIntPtr.Zero);
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);

            var interopHelper = new WindowInteropHelper(this);
            var exStyle = GetWindowLong(interopHelper.Handle, GwlExstyle);
            SetWindowLong(interopHelper.Handle, GwlExstyle, exStyle | WsExNoactivate);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    }
}