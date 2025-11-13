using Labb3_GUI.Models;
using Labb3_GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Labb3_GUI.Data
{
    internal class JsonPackService
    {

        private readonly string packPath;

        public JsonPackService()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            packPath = Path.Combine(path, "questionpack.json");

            if (!File.Exists(packPath))
                File.Create(packPath).Dispose();
        }

        public List<QuestionPack> LoadPacks()
        {
            if (!File.Exists(packPath))
                return new List<QuestionPack>();

            var json = File.ReadAllText(packPath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<QuestionPack>();

            var options = new JsonSerializerOptions
            {
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
            };

            return JsonSerializer.Deserialize<List<QuestionPack>>(json, options)
                   ?? new List<QuestionPack>();
        }

        public void SavePacks(IEnumerable<QuestionPack> packs)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(packs, options);
            File.WriteAllText(packPath, json);
        }


    }
}
