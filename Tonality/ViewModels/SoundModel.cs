using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using Tonality.ViewModels;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Tonality.ViewModels
{
    public class SoundModel : BindableBase
    {
        public SoundGroup NewAdds { get; set; }
        public SoundGroup Software { get; set; }
        public SoundGroup Messengers { get; set; }
        public SoundGroup Entertainment { get; set; }
        public SoundGroup GamesMsc { get; set; }
        public SoundGroup Android { get; set; }
        public SoundGroup Groups { get; set; }

        public bool IsDataLoaded { get; set; }

        public const string CustomSoundKey = "CustomSound";

        public SoundModel()
        {
            this.LoadData();
        }

        public void LoadData()
        {
            Software = LoadFromXml("Software.xml");
            Messengers = LoadFromXml("Messengers.xml");
            Entertainment = LoadFromXml("Entertainment.xml");
            GamesMsc = LoadFromXml("GamesMsc.xml");
            Android = LoadFromXml("Android.xml");
            NewAdds = LoadFromXml("NewAdds.xml");



            IsDataLoaded = true;
        }

        private SoundGroup LoadFromXml(string xmlName)
        {
            SaveSoundGroup isolatedGroup = new SaveSoundGroup();
            SaveSoundGroup assetsGroup;
            XmlSerializer serializer = new XmlSerializer(typeof(SaveSoundGroup));

            using (Stream fileStream = Application.GetResourceStream(new Uri("Xmls/" + xmlName, UriKind.Relative)).Stream)
            {
                assetsGroup = (SaveSoundGroup)serializer.Deserialize(fileStream);
            }

            if (IsolatedStorageFile.GetUserStoreForApplication().FileExists(xmlName))
            {
                using (Stream fileStream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(xmlName, FileMode.Open, FileAccess.Read))
                {
                    isolatedGroup = (SaveSoundGroup)serializer.Deserialize(fileStream);
                }

                foreach (var entry in assetsGroup.Items)
                {
                    if (!isolatedGroup.Items.Contains(entry))
                    {
                        isolatedGroup.Items.Add(entry);
                    }
                }
                for (int i = 0; i < isolatedGroup.Items.Count; i++)
                {
                    if (!assetsGroup.Items.Contains(isolatedGroup.Items[i]))
                    {
                        isolatedGroup.Items.RemoveAt(i);
                        i--;
                    }
                }

            }
            else
            {
                isolatedGroup = assetsGroup;
            }

            return new SoundGroup(isolatedGroup);
        }

        private void SaveToXml(SoundGroup group, string xmlName)
        {
            if (group != null)
            {
                Stream fileStream;
                fileStream = IsolatedStorageFile.GetUserStoreForApplication().CreateFile(xmlName);

                XmlSerializer serializer = new XmlSerializer(typeof(SaveSoundGroup));
                serializer.Serialize(fileStream, new SaveSoundGroup(group));

                fileStream.Close();
                fileStream.Dispose();
            }
        }

        public void SaveData()
        {
            this.SaveToXml(Software, "Software.xml");
            this.SaveToXml(Messengers, "Messengers.xml");
            this.SaveToXml(Entertainment, "Entertainment.xml");
            this.SaveToXml(GamesMsc, "GamesMsc.xml");
            this.SaveToXml(Android, "Android.xml");
            this.SaveToXml(NewAdds, "NewAdds.xml");
        }

        private SoundGroup LoadCustomSounds()
        {
            SoundGroup data;
            string dataFromAppSettings;

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(CustomSoundKey, out dataFromAppSettings))
            {
                data = JsonConvert.DeserializeObject<SoundGroup>(dataFromAppSettings);
            }
            else
            {
                data = new SoundGroup();
                data.Title = "mine";
            }

            return data;
        }
    }
}

