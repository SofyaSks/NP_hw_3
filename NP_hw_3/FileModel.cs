using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace NP_hw_3
{
    internal class FileModel : BindableBase
    {
        private string userName = "";
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        private Stream fileName;

        public Stream FileName
        {
            get => fileName;
            set => SetProperty(ref fileName, value);

        }

        private IReadOnlyList<string> chat = Array.Empty<string>();
        public IReadOnlyList<string> Chat
        {
            get => chat;
            set => SetProperty(ref chat, value);
        }
    }
}
