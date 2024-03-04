//Written for Subspace Discovery. https://store.steampowered.com/app/1717290/
using System.IO;

namespace Subspace_Discovery_stf_Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            using FileStream source = File.OpenRead(args[0]);
            BinaryReader br = new(source);
            br.BaseStream.Position = 16;
            int numSubfiles = br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            SUBFILE[] subfiles = new SUBFILE[numSubfiles];
            for (int i = 0; i < numSubfiles; i++)
            {
                br.ReadByte();//padding
                subfiles[i].name = new(br.ReadChars(br.ReadInt16()));
                subfiles[i].isDirectory = br.ReadByte();
                subfiles[i].size = br.ReadInt32();
                subfiles[i].offset = br.ReadInt32();
            }
            string extractTo = Path.GetDirectoryName(source.Name) + "//" + Path.GetFileNameWithoutExtension(source.Name);
            Directory.CreateDirectory(extractTo);
            string currentDirectory = extractTo;
            foreach (SUBFILE sub in subfiles)
            {
                br.BaseStream.Position = sub.offset;

                if (sub.isDirectory == 1)
                {
                    currentDirectory = extractTo + "//" + sub.name; 
                    Directory.CreateDirectory(currentDirectory);
                }
                else if (sub.isDirectory == 0)
                {
                    br.ReadByte();//padding
                    int size = br.ReadInt32();
                    if (size != sub.size)
                        throw new System.Exception("fuck.");

                    using FileStream FS = File.Create(currentDirectory + "//" + sub.name);
                    BinaryWriter bw = new(FS);
                    bw.Write(br.ReadBytes(sub.size));
                    bw.Close();
                }
            }
        }
    }
    struct SUBFILE
    {
        public int offset;
        public string name;
        public byte isDirectory;
        public int size;
    }
}
