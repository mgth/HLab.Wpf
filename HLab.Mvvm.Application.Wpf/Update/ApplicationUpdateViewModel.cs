using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Base.ReactiveUI;
using HLab.Erp.Core.Update;
using HLab.Mvvm.Application.Updater;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;


namespace HLab.Mvvm.Application.Wpf.Update
{
    public class UpdaterWpf : ReactiveModel, IUpdater
    {
        public IApplicationInfoService Info { get; }


        public string Message
        {
            get => _message;
            set => SetAndRaise(ref _message,value);
        }
        string _message ;

        public string FileName
        {
            get => _fileName;
            set => SetAndRaise(ref _fileName,value);
        }
        string _fileName ;



        // http://www.chmp.org/sites/default/files/apps/sampling/
        public string Url
        {
            get => _url;
            set => SetAndRaise(ref _url,value);
        }

        string _url ;

        public Version NewVersion
        {
            get => _newVersion;
            set => SetAndRaise(ref _newVersion,value);
        }

        Version _newVersion ;

        public double Progress
        {
            get => _progress;
            set => SetAndRaise(ref _progress,value);
        }

        double _progress ;

        public bool Updated
        {
            get => _updated;
            set => SetAndRaise(ref _updated,value);
        }

        bool _updated = false;

        public void Update()
        {
            var filename = FileName.Replace("{version}", NewVersion.ToString());
            var path = Path.GetTempPath() + filename;

            var task = Task.Run(() => {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(Url + filename), path);
            });
        }

        void RunUpdate()
        {
            var filename = FileName.Replace("{version}", NewVersion.ToString());
            var path = Path.GetTempPath() + filename;
            var startInfo = new ProcessStartInfo(path) { Verb = "runas" };
            try
            {
                Process.Start(startInfo);
                Updated = true;
            }
            catch (Win32Exception)
            {
                Message = "L'execution a échouée";
            }
            catch (WebException)
            {
                Message = "Le téléchargement a échoué";
            }
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var bytesIn = double.Parse(e.BytesReceived.ToString());
            var totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            Progress = bytesIn / totalBytes * 100;
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            RunUpdate();
        }
        public void CheckVersion()
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(Url + "version");

                request.Method = "GET";

                var response = request.GetResponse() as HttpWebResponse;

                var streamResponse = response?.GetResponseStream();
                if (streamResponse == null) return;

                var streamRead = new StreamReader(streamResponse);

                var version = Version.Parse(streamRead.ReadToEnd());

                streamResponse.Close();
                streamRead.Close();
                response.Close();

                NewVersion = version;
            }
            catch (UriFormatException e)
            {
                Message = e.Message;
            }
            catch (WebException e)
            {
                Message = e.Message;
            }
            catch (ArgumentException e)
            {
                Message = e.Message;
            }
        }

        public bool NewVersionFound => _newVersionFound.Value;
        readonly ObservableAsPropertyHelper<bool> _newVersionFound;

        public Version CurrentVersion => _currentVersion.Value;
        readonly ObservableAsPropertyHelper<Version> _currentVersion;

        public UpdaterWpf(IApplicationInfoService info)
        {
            Info = info;

            _newVersionFound = this.WhenAnyValue(
                e => e.NewVersion, 
                e => e.CurrentVersion,
                selector : (newVersion,currentVersion) => newVersion > currentVersion
                )
                 .ToProperty(this, e => e.NewVersionFound);

            _currentVersion = this.WhenAnyValue(e => e.Info.Version)
                .ToProperty(this, e => e.CurrentVersion);
        }
    }

    public class ApplicationUpdateViewModel : ViewModel<UpdaterWpf>
    {
        public ApplicationUpdateViewModel()
        {
            UpdateCommand = ReactiveCommand.Create(Model.Update, this.WhenAnyValue(e => e.Model.NewVersionFound));
        }

        public void Show()
        {
            var view = new ApplicationUpdateView
            {
                DataContext = this
            };
            // TODO : view.ShowDialog();
        }


        public ICommand UpdateCommand { get; }
    }
}
