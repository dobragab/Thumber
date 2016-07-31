using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Thumber
{
    class ConvertParams
    {
        public string filename;
        public string filepath;
        public string extension;

        public bool IsPng()
        {
            return extension.ToUpper() == ".PNG";
        }

        public static ConvertParams GetParams(string filepath)
        {
            ConvertParams result = new ConvertParams();

            result.filepath = filepath;
            result.extension = Path.GetExtension(filepath);
            result.filename = Path.GetFileName(filepath);

            return result;
        }

        public static ConvertParams GetConvertedParams(ConvertParams param, Thumb thumb)
        {
            ConvertParams result = new ConvertParams();

            result.extension = param.extension;
            result.filename = Path.ChangeExtension(String.Format("{0}_{1}", Path.GetFileNameWithoutExtension(param.filename), thumb.name), param.extension);
            result.filepath = Path.Combine(Path.GetDirectoryName(param.filepath), result.filename);

            return result;
        }

        public static bool ConvertFile(ConvertParams oldparams, ConvertParams newparams, Thumb thumb)
        {
            if (File.Exists(newparams.filepath))
                return false;

            string parameters = String.Format("-o \"{0}\" -overwrite -quiet -out {3} -ratio -resize {2} 0 \"{1}\"", 
                newparams.filepath, 
                oldparams.filepath, 
                thumb.width, 
                newparams.IsPng() ? "png" : "jpeg");

            ProcessStartInfo info = new ProcessStartInfo("nconvert.exe", parameters);
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            Process proc = Process.Start(info);
            proc.WaitForExit();

            return true;
        }
    }
}
