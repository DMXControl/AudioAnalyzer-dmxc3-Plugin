using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using AudioAnalyzer.Engine;
using AudioAnalyzer.Wpf.ViewModels;
using AudioAnalyzer.Wpf.Views;
using Lumos.GUI;
using Lumos.GUI.BaseWindow;
using Lumos.GUI.Themes;
using LumosLIB.GUI.Windows;

namespace AudioAnalyzer.Wpf
{
    /// <summary>
    /// WinForms shell for the WPF surface. DMXControl still hosts tool windows as WinForms,
    /// so a migrated window is an ElementHost around a UserControl - same construction as
    /// LumosGUI's CommandLineView.
    /// </summary>
    public class AudioAnalyzerWpfView : ToolWindow
    {
        private readonly AudioAnalyzerControl control;
        private readonly AudioAnalyzerViewModel viewModel;

        public AudioAnalyzerWpfView(IAudioAnalyzer analyzer)
        {
            if (analyzer == null)
                throw new ArgumentNullException("analyzer");

            this.MainFormMenu = MenuType.Windows;
            this.MenuIconKey = "window_equalizer";
            this.Name = "audioAnalyzerWpfView";
            this.Text = LumosLIB.Tools.I18n.T._("Audio Analyzer");
            this.TabText = LumosLIB.Tools.I18n.T._("Audio Analyzer");

            var resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioAnalyzerWpfView));
            this.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");

            this.control = new AudioAnalyzerControl();
            this.viewModel = new AudioAnalyzerViewModel(analyzer);
            this.control.DataContext = this.viewModel;

            var host = new ElementHost();
            host.Dock = DockStyle.Fill;
            host.Child = this.control;
            this.Controls.Add(host);
        }

        public override EMenuGroups MenuGroup
        {
            get { return EMenuGroups.Control; }
        }

        public override void OnThemeChanged(IGUITheme theme)
        {
            base.OnThemeChanged(theme);

            if (control == null)
                return;

            control.UpdateDefaultStyle();
            control.InvalidateVisual();
            control.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { })).Wait();
            control.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => { })).Wait();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (viewModel != null)
                    viewModel.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
