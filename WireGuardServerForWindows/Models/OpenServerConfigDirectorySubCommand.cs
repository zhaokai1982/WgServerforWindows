﻿using System.Diagnostics;
using System.Windows.Input;
using WireGuardServerForWindows.Properties;

namespace WireGuardServerForWindows.Models
{
    public class OpenServerConfigDirectorySubCommand : PrerequisiteItem
    {
        public OpenServerConfigDirectorySubCommand() : base
        (
            title: string.Empty,
            successMessage: string.Empty,
            errorMessage: string.Empty,
            resolveText: string.Empty,
            configureText: Resources.OpenServerConfigDirectoryConfigureText
        )
        {
            AppSettings.Instance.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(AppSettings.Instance.CustomServerConfigDirectory))
                {
                    RaisePropertyChanged(nameof(SuccessMessage));
                    RaisePropertyChanged(nameof(CanConfigure));
                }
            };
        }

        #region PrerequisiteItem members

        public override string SuccessMessage
        {
            get => string.Format(Resources.OpenServerConfigDirectorySuccessMessage, ServerConfigurationPrerequisite.ServerConfigDirectory);
            set { }
        }

        public override void Configure()
        {
            if (CanConfigure)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                Process.Start(new ProcessStartInfo
                {
                    FileName = ServerConfigurationPrerequisite.ServerConfigDirectory,
                    UseShellExecute = true
                });

                Mouse.OverrideCursor = null;
            }
        }

        #endregion
    }
}
